using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace EasyPro.Controllers
{
    public class DPayrollsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DPayrollsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DPayrolls
        public async Task<IActionResult> Index()
        {
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(-1);
            var endDate = month.AddDays(-1);

            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var payroll = await _context.DPayrolls.Where(i=>i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.EndofPeriod == endDate).Join(
                _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
                p => p.Sno.ToString(),
                s => s.Sno.ToString(),
                (p, s) => new PayrollVm
                {
                    Sno = s.Sno,
                    Names = s.Names,
                    PhoneNo = s.PhoneNo,
                    IdNo = s.IdNo,
                    Bank = "",
                    AccNo = s.AccNo,
                    Branch = s.Branch,
                    Quantity = p.KgsSupplied,
                    GrossPay = p.Gpay,
                    Transport = p.Transport,
                    Registration = 0,
                    Advance = p.Advance,
                    Others = p.Others,
                    Netpay = p.Npay
                }).ToListAsync();
            return View(payroll);
        }

        // GET: DPayrolls/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dPayroll = await _context.DPayrolls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPayroll == null)
            {
                return NotFound();
            }

            return View(dPayroll);
        }

        // GET: DPayrolls/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DPayrolls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EndDate")] PayrollPeriod period)
        {
            utilities.SetUpPrivileges(this);
            var startDate = new DateTime(period.EndDate.Year, period.EndDate.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var payrolls = _context.DPayrolls
                .Where(p => p.Yyear == endDate.Year && p.Mmonth == endDate.Month 
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            if (payrolls.Any())
            {
                _context.DPayrolls.RemoveRange(payrolls);
                _context.SaveChanges();
            }
            var transportersPayRolls = _context.DTransportersPayRolls
                .Where(p => p.Yyear == endDate.Year && p.Mmonth == endDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            if (transportersPayRolls.Any())
            {
                _context.DTransportersPayRolls.RemoveRange(transportersPayRolls);
                _context.SaveChanges();
            }
            var transpoterIntakes = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate && i.TransDate <= endDate
                && i.ProductType.ToLower().Equals("transport"));
            if (transpoterIntakes.Any())
            {
                _context.ProductIntake.RemoveRange(transpoterIntakes);
                _context.SaveChanges();
            }

            var branchNames = _context.DBranch.Where(b => b.Bcode == sacco)
                .Select(b => b.Bname.ToUpper());

            foreach(var branchName in branchNames)
            {
                await ConsolidateTranspoterIntakes(startDate, endDate, sacco, branchName, loggedInUser);
                var supplierNos = _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper()) 
                && s.Branch.ToUpper().Equals(branchName)).Select(s => s.Sno.ToString());

                var productIntakes = _context.ProductIntake
                .Where(p => p.TransDate >= startDate && p.TransDate <= endDate
                && supplierNos.Contains(p.Sno) && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && p.Branch.ToUpper().Equals(branchName)).ToList();

                var intakes = productIntakes.GroupBy(p => p.Sno).ToList();
                intakes.ForEach(p =>
                {
                    var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                    var transport = p.Where(k => k.Description.ToLower().Contains("transport"));
                    var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet"));
                    var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                    var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                    var carryforward = p.Where(k => k.ProductType.ToLower().Contains("carry forward"));
                    var Others = p.Where(k => k.ProductType.ToLower().Contains("others"));
                    var clinical = p.Where(k => k.ProductType.ToLower().Contains("clinical"));
                    var ai = p.Where(k => k.ProductType.ToLower().Contains("ai"));
                    var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                    var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);

                    //var payroll = new DPayroll();
                    long.TryParse(p.Key, out long sno);
                    var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno 
                    && s.Scode.ToUpper().Equals(sacco.ToUpper()) && s.Branch.ToUpper().Equals(branchName));
                    if (supplier != null)
                    {
                        var debits = corrections.Sum(s => s.DR);
                        var Tot = advance.Sum(s => s.DR) + transport.Sum(s => s.DR)
                        + agrovet.Sum(s => s.DR) + bonus.Sum(s => s.DR) + shares.Sum(s => s.DR)
                        + Others.Sum(s => s.DR)+ clinical.Sum(s => s.DR)+ ai.Sum(s => s.DR)+ tractor.Sum(s => s.DR)
                        + carryforward.Sum(s => s.DR);
                        _context.DPayrolls.Add(new DPayroll
                        {
                            Sno = (int?)supplier.Sno,
                            Gpay = p.Sum(s => s.CR),
                            KgsSupplied = (double?)p.Sum(s => s.Qsupplied),
                            Advance = advance.Sum(s => s.DR),
                            CurryForward= carryforward.Sum(s => s.DR),
                            Others = Others.Sum(s => s.DR),
                            CLINICAL = clinical.Sum(s => s.DR),
                            AI = ai.Sum(s => s.DR),
                            Tractor=tractor.Sum(s => s.DR),
                            Transport = transport.Sum(s => s.DR),
                            Agrovet = agrovet.Sum(s => s.DR),
                            Bonus = bonus.Sum(s => s.DR),
                            Hshares = shares.Sum(s => s.DR),
                            Tdeductions = Tot,
                            Npay = p.Sum(s => s.CR) - debits - Tot,
                            Yyear = endDate.Year,
                            Mmonth = endDate.Month,
                            Bank = supplier.Bcode,
                            AccountNumber = supplier.AccNo,
                            Bbranch = supplier.Bbranch,
                            IdNo = supplier.IdNo,
                            EndofPeriod = endDate,
                            SaccoCode = sacco,
                            Auditid = loggedInUser,
                            Branch=branchName
                        });
                    }
                    
                });
                
                //Transporters
            }
            await _context.SaveChangesAsync();
            //Transporters
            var branchTransporters = _context.DBranch.Where(b => b.Bcode == sacco)
                .Select(b => b.Bname.ToUpper());
            foreach(var branchName in branchTransporters)
            {
                //Transporters
                var transpoterCodes = _context.DTransporters
               .Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper()) && s.Tbranch.ToUpper().Equals(branchName))
               .Select(s => s.TransCode.ToUpper());

                var productIntakes = _context.ProductIntake
                .Where(p => p.TransDate >= startDate && p.TransDate <= endDate
                && transpoterCodes.Contains(p.Sno.Trim().ToUpper())
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && p.Branch.ToUpper().Equals(branchName)).ToList();

                var intakes = productIntakes.GroupBy(p => p.Sno.Trim().ToUpper()).ToList();
                intakes.ForEach(p =>
                {
                    var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                    var transport = p.Where(k => k.ProductType.ToLower().Contains("transport"));
                    var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet"));
                    var variance = p.Where(k => k.ProductType.ToLower().Contains("variance"));
                    var Others = p.Where(k => k.ProductType.ToLower().Contains("others"));
                    var clinical = p.Where(k => k.ProductType.ToLower().Contains("clinical"));
                    var carryforward = p.Where(k => k.ProductType.ToLower().Contains("carry forward"));
                    var ai = p.Where(k => k.ProductType.ToLower().Contains("ai"));
                    var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                    var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                    var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                    var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);

                    //var payroll = new DTransportersPayRoll();
                    long.TryParse(p.Key, out long sno);

                    var transporter = _context.DTransporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(p.Key.ToUpper())
                    && s.ParentT.ToUpper().Equals(sacco.ToUpper()) && s.Tbranch.ToUpper().Equals(branchName));
                    if (transporter != null)
                    {
                        var debits = corrections.Sum(s => s.DR);
                        var amount = p.Sum(s => s.CR);
                        var Tot = advance.Sum(s => s.DR) + agrovet.Sum(s => s.DR) + shares.Sum(s => s.DR)
                        + Others.Sum(s => s.DR) + clinical.Sum(s => s.DR) + ai.Sum(s => s.DR)
                        + tractor.Sum(s => s.DR) + variance.Sum(s => s.DR) + carryforward.Sum(s => s.DR);
                        var subsidy = 0;
                        _context.DTransportersPayRolls.Add(new DTransportersPayRoll
                        {
                            Code = transporter.TransCode,
                            Amnt = amount,
                            Subsidy = subsidy,
                            GrossPay = amount + subsidy,
                            QntySup = (double?)p.Sum(s => s.Qsupplied),
                            Advance = advance.Sum(s => s.DR),
                            CurryForward = carryforward.Sum(s => s.DR),
                            Others = Others.Sum(s => s.DR),
                            AI = ai.Sum(s => s.DR),
                            CLINICAL = clinical.Sum(s => s.DR),
                            Tractor = tractor.Sum(s => s.DR),
                            VARIANCE = variance.Sum(s => s.DR),
                            Agrovet = agrovet.Sum(s => s.DR),
                            Hshares = shares.Sum(s => s.DR),
                            Totaldeductions = Tot,
                            NetPay = (amount + subsidy) - debits - Tot,
                            BankName = transporter.Bcode,
                            Yyear = endDate.Year,
                            Mmonth = endDate.Month,
                            AccNo = transporter.Accno,
                            BBranch = transporter.Bbranch,
                            EndPeriod = endDate,
                            Rate = 0,
                            SaccoCode = sacco,
                            AuditId = loggedInUser,
                            Branch = branchName
                        });
                    }

                });
            }
            _context.SaveChanges();
            //await _context.SaveChangesAsync();
            _notyf.Success("Payroll processed successfully");
            return RedirectToAction(nameof(Index));
        }

        private async Task ConsolidateTranspoterIntakes(DateTime startDate, DateTime endDate, string sacco, string branchName, string loggedInUser)
        {
            var transporterIntakes = _context.DTransports.Join(_context.ProductIntake,
                t => t.Sno.ToString().Trim(),
                i => i.Sno.Trim(),
                (t, i) => new
                {
                    i.Qsupplied,
                    t.Sno,
                    t.TransCode,
                    i.SaccoCode,
                    i.Remarks,
                    i.TransTime,
                    i.Ppu,
                    i.Branch,
                    i.ProductType,
                    i.CR,
                    i.DR,
                    i.DrAccNo,
                    i.CrAccNo,
                    i.TransDate,
                    t.Rate,
                    t.Startdate
                }).Where(i => i.TransDate >= startDate && i.TransDate <= endDate && i.SaccoCode == sacco 
                && i.Branch.ToUpper().Equals(branchName.ToUpper())).ToList();
            
            var intakes = transporterIntakes.GroupBy(i => i.TransCode.Trim()).ToList();
            intakes.ForEach(i =>
            {
                if (!string.IsNullOrEmpty(i.Key))
                {
                    // Debit supplier transport amount
                    var suppliers = i.GroupBy(s => s.Sno).ToList();
                    suppliers.ForEach(s =>
                    {
                        var intake = s.FirstOrDefault();
                        decimal? cr = 0;
                        var dr = s.Sum(t => t.Qsupplied) * intake.Rate;
                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = intake.Sno.ToString(),
                            TransDate = intake.TransDate,
                            TransTime = intake.TransTime,
                            ProductType = "Transport",
                            Qsupplied = s.Sum(t => t.Qsupplied),
                            Ppu = intake.Rate,
                            CR = cr,
                            DR = dr,
                            Description = "Transport",
                            TransactionType = TransactionType.Deduction,
                            Remarks = intake.Remarks,
                            AuditId = loggedInUser,
                            Auditdatetime = DateTime.Now,
                            Branch = intake.Branch,
                            SaccoCode = intake.SaccoCode,
                            DrAccNo = intake.DrAccNo,
                            CrAccNo = intake.CrAccNo

                        });

                        var product = intake?.ProductType ?? "";
                        var price = _context.DPrices.FirstOrDefault(p => p.Products.ToUpper().Equals(product.ToUpper()));
                        // Credit transpoter transport amount
                        cr = s.Sum(t => t.Qsupplied) * intake.Rate;
                        dr = 0;
                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = intake.TransCode.Trim().ToUpper(),
                            TransDate = intake.TransDate,
                            TransTime = intake.TransTime,
                            ProductType = "Transport",
                            Qsupplied = s.Sum(t => t.Qsupplied),
                            Ppu = intake.Rate,
                            CR = cr,
                            DR = dr,
                            Description = "Transport",
                            TransactionType = TransactionType.Deduction,
                            Remarks = intake.Remarks,
                            AuditId = loggedInUser,
                            Auditdatetime = DateTime.Now,
                            Branch = intake.Branch,
                            SaccoCode = intake.SaccoCode,
                            DrAccNo = price?.TransportDrAccNo ?? "",
                            CrAccNo = price?.TransportCrAccNo ?? ""
                        });
                    });
                }
            });
        }

        // GET: DPayrolls/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dPayroll = await _context.DPayrolls.FindAsync(id);
            if (dPayroll == null)
            {
                return NotFound();
            }
            return View(dPayroll);
        }

        // POST: DPayrolls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,Transport,Agrovet,Bonus,Tmshares,Fsa,Hshares,Advance,Others,Tdeductions,KgsSupplied,Gpay,Npay,Yyear,Mmonth,Bank,AccountNumber,Bbranch,Trader,Sbranch,EndofPeriod,Auditid,Auditdatetime,Mainaccno,Transportaccno,Agrovetaccno,Aiaccno,Tmsharesaccno,Fsaaccno,Hsharesaccno,Advanceaccno,Otheraccno,Netaccno,Subsidy,Cbo,IdNo,Tchp,Midmonth,Deduct12,Mpesa")] DPayroll dPayroll)
        {
            utilities.SetUpPrivileges(this);
            if (id != dPayroll.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dPayroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPayrollExists(dPayroll.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dPayroll);
        }

        // GET: DPayrolls/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dPayroll = await _context.DPayrolls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPayroll == null)
            {
                return NotFound();
            }

            return View(dPayroll);
        }

        // POST: DPayrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dPayroll = await _context.DPayrolls.FindAsync(id);
            _context.DPayrolls.Remove(dPayroll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPayrollExists(long id)
        {
            return _context.DPayrolls.Any(e => e.Id == id);
        }

        [HttpGet]
        public JsonResult CurryForward(DateTime period)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var startDate = new DateTime(period.Year, period.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var nextMonth = startDate.AddMonths(1);
            
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco).ToList();
            suppliers.ForEach(s =>
            {
                var payrolls = _context.DPayrolls.Where(p => p.SaccoCode == sacco 
                && p.EndofPeriod == endDate && p.Sno == s.Sno).ToList();

                var netPay = payrolls.Sum(p => p.Npay);
                var debited = _context.ProductIntake.Any(i => i.SaccoCode == sacco && i.Sno == s.Sno.ToString()
                && i.TransDate >= nextMonth && i.Description == "Carry Forward");
                if(netPay < 0 && !debited)
                {
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = s.Sno.ToString(),
                        TransDate = nextMonth,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = "Carry Forward",
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = 0,
                        DR = -netPay,
                        Balance = 0,
                        Description = "Carry Forward",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Carry Forward",
                        AuditId = auditId,
                        Auditdatetime = DateTime.Now,
                        Branch = "",
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                }
            });

            _context.SaveChanges();
            _notyf.Success("Curry Forward processed successfully");
            return Json("");
        }
    }
}
