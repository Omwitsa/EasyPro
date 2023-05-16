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
            var payroll = await _context.DPayrolls.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.EndofPeriod == endDate).Join(
                _context.DSuppliers.Where(d => d.Scode.ToUpper().Equals(sacco.ToUpper())),
                p => p.Sno.ToUpper(),
                s => s.Sno.ToUpper(),
                (p, s) => new PayrollVm
                {
                    Sno = s.Sno,
                    Names = s.Names,
                    PhoneNo = s.PhoneNo,
                    IdNo = s.IdNo,
                    Bank = s.Bcode,
                    AccNo = s.AccNo,
                    Branch = s.Branch,
                    Quantity = p.KgsSupplied,
                    GrossPay = p.Gpay,
                    Transport = p.Transport,
                    Advance = p.Advance,
                    Fsa = p.Fsa, // loan
                    Others = p.Others,
                    Netpay = p.Npay,
                    Agrovet = p.Agrovet,
                    Bonus = p.Bonus,
                    Hshares = p.Hshares,
                    CurryForward = p.CurryForward,
                    Clinical = p.CLINICAL,
                    AI = p.AI,
                    Tractor = p.Tractor,
                    Extension = p.extension,
                    SMS = p.SMS,
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
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var branchNames = _context.DBranch.Where(b => b.Bcode == sacco)
                .Select(b => b.Bname.ToUpper());
            
            var payrolls = _context.DPayrolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            if (payrolls.Any())
            {
                _context.DPayrolls.RemoveRange(payrolls);
                _context.SaveChanges();
            }
            var transportersPayRolls = _context.DTransportersPayRolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            if (transportersPayRolls.Any())
            {
                _context.DTransportersPayRolls.RemoveRange(transportersPayRolls);
                _context.SaveChanges();
            }

            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var advIntakes = productIntakeslist.Where(i => i.TransDate >= startDate && i.TransDate <= DateTime.Today && i.Remarks == StrValues.AdvancePayroll);
            if (advIntakes.Any() && period.EndDate != monthsLastDate)
            {
                _context.ProductIntake.RemoveRange(advIntakes);
                _context.SaveChanges();
            }
            
            if (sacco != "MBURUGU DAIRY F.C.S")
            {
                var transpoterIntakes = productIntakeslist.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate && i.TransDate <= period.EndDate
                && i.Description.ToLower().Equals("transport"));
                if (transpoterIntakes.Any())
                {
                    _context.ProductIntake.RemoveRange(transpoterIntakes);
                    _context.SaveChanges();
                }
            }

            var deletestandingorder = productIntakeslist.Where(i => i.TransDate >= startDate && i.TransDate <= period.EndDate
                 && i.Remarks == "Standing Order" && i.SaccoCode == sacco);
            if (deletestandingorder.Any())
            {
                _context.ProductIntake.RemoveRange(deletestandingorder);
                _context.SaveChanges();
            }
            calcstandingorder(startDate, period.EndDate, sacco, loggedInUser);

            var selectdistinctdedname = _context.d_PreSets.Where(b => b.saccocode == sacco)
            .Select(b => b.Deduction.ToUpper()).Distinct();
            foreach (var dedtype in selectdistinctdedname)
            {
                var deletesdefaultded = productIntakeslist.Where(i => i.TransDate >= startDate && i.TransDate <= period.EndDate
                && i.ProductType.ToUpper().Equals(dedtype.ToUpper()) && i.SaccoCode == sacco).ToList();
                if (deletesdefaultded.Any())
                {
                    _context.ProductIntake.RemoveRange(deletesdefaultded);
                }
            }

            var checkgls = _context.Gltransactions
               .Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= startDate
               && f.TransDate <= period.EndDate && (f.TransDescript.ToUpper().Equals("Bonus".ToUpper()) || f.TransDescript.ToUpper().Equals("Shares".ToUpper())));
            if (checkgls.Any())
            {
                _context.Gltransactions.RemoveRange(checkgls);
            }

            var checksharespaid = _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                            && f.TransDate >= startDate && f.TransDate <= period.EndDate && f.Type == "Checkoff"
                            && f.Pmode == "Checkoff");
            if (checksharespaid.Any())
            {
                _context.DShares.RemoveRange(checksharespaid);
            }
            _context.SaveChanges();

            foreach (var branchName in branchNames)
            {
                //update default deductions if any like bonus
                await calcDefaultdeductions(startDate, period.EndDate, sacco, branchName, loggedInUser);
                if (sacco != "MBURUGU DAIRY F.C.S")
                    await ConsolidateTranspoterIntakes(startDate, period.EndDate, sacco, branchName, loggedInUser);
            }
            _context.SaveChanges();

            var dcodes = _context.DDcodes.Where(c => c.Description.ToLower().Equals("advance") || c.Description.ToLower().Equals("transport")
               || c.Description.ToLower().Equals("agrovet") || c.Description.ToLower().Equals("bonus") || c.Description.ToLower().Equals("shares")
               || c.Description.ToLower().Equals("loan") || c.Description.ToLower().Equals("carry forward") || c.Description.ToLower().Equals("clinical")
               || c.Description.ToLower().Equals("a.i") || c.Description.ToLower().Equals("ai") || c.Description.ToLower().Equals("tractor")
               || c.Description.ToLower().Equals("sms") || c.Description.ToLower().Equals("extension work")).Select(c => c.Description.ToLower()).ToList();
            foreach (var branchName in branchNames)
            {
                var supplierNos = _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())
                && s.Branch.ToUpper().Equals(branchName.ToUpper())).Select(s => s.Sno);

                var productIntakes = await productIntakeslist
                .Where(p => p.TransDate >= startDate && p.TransDate <= period.EndDate
                && supplierNos.Contains(p.Sno) && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && p.Branch.ToUpper().Equals(branchName.ToUpper())).ToListAsync();

                var intakes = productIntakes.GroupBy(p => p.Sno.ToUpper()).ToList();
                intakes.ForEach(p =>
                {
                    var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                    var transport = p.Where(k => k.Description.ToLower().Contains("transport"));
                    var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet"));
                    var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                    var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                    var loan = p.Where(k => k.ProductType.ToLower().Contains("loan"));
                    var carryforward = p.Where(k => k.ProductType.ToLower().Contains("carry forward"));
                    var clinical = p.Where(k => k.ProductType.ToLower().Contains("clinical"));
                    var ai = p.Where(k => (k.ProductType.ToLower().Contains("ai") || k.ProductType.ToLower().Contains("a.i")));
                    var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                    var extension = p.Where(k => k.ProductType.ToLower().Contains("extension work"));
                    var SMS = p.Where(k => k.ProductType.ToLower().Contains("sms"));
                    var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                    var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                    var Others = p.Where(k => !dcodes.Contains(k.ProductType.ToLower()) && !k.ProductType.ToUpper().Equals("AGROVET")
                    && !k.ProductType.ToUpper().Equals("MILK") && k.TransactionType == TransactionType.Deduction);

                    var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(p.Key)
                    && s.Scode.ToUpper().Equals(sacco.ToUpper()) && s.Branch.ToUpper().Equals(branchName.ToUpper()));
                    if (supplier != null)
                    {

                        var debits = corrections.Sum(s => s.DR);
                        var credited = p.Sum(s => s.CR);
                        var Tot = advance.Sum(s => s.DR) + agrovet.Sum(s => s.DR) + bonus.Sum(s => s.DR) + shares.Sum(s => s.DR)
                        + Others.Sum(s => s.DR) + clinical.Sum(s => s.DR) + ai.Sum(s => s.DR) + tractor.Sum(s => s.DR) + transport.Sum(s => s.DR)
                        + carryforward.Sum(s => s.DR) + loan.Sum(s => s.DR) + extension.Sum(s => s.DR) + SMS.Sum(s => s.DR);

                        if(supplier.TransCode == "Monthly" && period.EndDate == monthsLastDate)
                            _context.DPayrolls.Add(new DPayroll
                            {
                                Sno = supplier.Sno,
                                Gpay = credited,
                                KgsSupplied = (double?)milk.Sum(s => s.Qsupplied),
                                Advance = advance.Sum(s => s.DR),
                                CurryForward = carryforward.Sum(s => s.DR),
                                Others = Others.Sum(s => s.DR),
                                CLINICAL = clinical.Sum(s => s.DR),
                                AI = ai.Sum(s => s.DR),
                                Tractor = tractor.Sum(s => s.DR),
                                Transport = transport.Sum(s => s.DR),
                                extension = extension.Sum(s => s.DR),
                                SMS = SMS.Sum(s => s.DR),
                                Agrovet = agrovet.Sum(s => s.DR),
                                Bonus = bonus.Sum(s => s.DR),
                                Fsa = loan.Sum(s => s.DR),
                                Hshares = shares.Sum(s => s.DR),
                                Tdeductions = Tot,
                                Npay = credited - (debits + Tot),
                                Yyear = period.EndDate.Year,
                                Mmonth = period.EndDate.Month,
                                Bank = supplier.Bcode,
                                AccountNumber = supplier.AccNo,
                                Bbranch = supplier.Bbranch,
                                IdNo = supplier.IdNo,
                                EndofPeriod = period.EndDate,
                                SaccoCode = sacco,
                                Auditid = loggedInUser,
                                Branch = supplier.Branch
                            });

                        if (supplier.TransCode == "Weekly" && period.EndDate != monthsLastDate)
                        {
                            _context.DPayrolls.Add(new DPayroll
                            {
                                Sno = supplier.Sno,
                                Gpay = credited,
                                KgsSupplied = (double?)milk.Sum(s => s.Qsupplied),
                                Advance = advance.Sum(s => s.DR),
                                CurryForward = carryforward.Sum(s => s.DR),
                                Others = Others.Sum(s => s.DR),
                                CLINICAL = clinical.Sum(s => s.DR),
                                AI = ai.Sum(s => s.DR),
                                Tractor = tractor.Sum(s => s.DR),
                                Transport = transport.Sum(s => s.DR),
                                extension = extension.Sum(s => s.DR),
                                SMS = SMS.Sum(s => s.DR),
                                Agrovet = agrovet.Sum(s => s.DR),
                                Bonus = bonus.Sum(s => s.DR),
                                Fsa = loan.Sum(s => s.DR),
                                Hshares = shares.Sum(s => s.DR),
                                Tdeductions = Tot,
                                Npay = credited - (debits + Tot),
                                Yyear = period.EndDate.Year,
                                Mmonth = period.EndDate.Month,
                                Bank = supplier.Bcode,
                                AccountNumber = supplier.AccNo,
                                Bbranch = supplier.Bbranch,
                                IdNo = supplier.IdNo,
                                EndofPeriod = period.EndDate,
                                SaccoCode = sacco,
                                Auditid = loggedInUser,
                                Branch = supplier.Branch
                            });

                            _context.ProductIntake.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                TransDate = DateTime.Today,
                                TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                                ProductType = "",
                                Qsupplied = 0,
                                Ppu = 0,
                                CR = 0,
                                DR = credited,
                                Balance = 0,
                                Description = "Advance",
                                TransactionType = TransactionType.Deduction,
                                Remarks = StrValues.AdvancePayroll,
                                AuditId = loggedInUser,
                                Auditdatetime = DateTime.Now,
                                Branch = supplier.Branch,
                                SaccoCode = sacco,
                                DrAccNo = "0",
                                CrAccNo = "0",
                                Zone = "",
                                MornEvening = ""
                            });
                        }
                    }
                });
            }

            await _context.SaveChangesAsync();
            //Transporters
            var branchTransporters = _context.DBranch.Where(b => b.Bcode == sacco)
                .Select(b => b.Bname.ToUpper());
            foreach (var branchName in branchTransporters)
            {
                //Transporters
                var transpoterCodes = _context.DTransporters
               .Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper()) && s.Tbranch.ToUpper().Equals(branchName))
               .Select(s => s.TransCode.Trim().ToUpper());

                var productIntakes = productIntakeslist
                .Where(p => p.TransDate >= startDate && p.TransDate <= period.EndDate
                && transpoterCodes.Contains(p.Sno.Trim().ToUpper())
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && p.Branch.ToUpper().Equals(branchName.ToUpper())).ToList();

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
                    var ai = p.Where(k => (k.ProductType.ToLower().Contains("ai") || k.ProductType.ToLower().Contains("a.i")));
                    var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                    var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                    var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                    var extension = p.Where(k => k.ProductType.ToLower().Contains("extension work"));
                    var SMS = p.Where(k => k.ProductType.ToLower().Contains("sms"));
                    var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                    var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                    //var payroll = new DTransportersPayRoll();
                    long.TryParse(p.Key, out long sno);

                    var transporter = _context.DTransporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(p.Key.ToUpper())
                    && s.ParentT.ToUpper().Equals(sacco.ToUpper()) && s.Tbranch.ToUpper().Equals(branchName.ToUpper()));
                    if (transporter != null)
                    {
                        var debits = corrections.Sum(s => s.DR);
                        var amount = p.Sum(s => s.CR);
                        var Tot = advance.Sum(s => s.DR) + agrovet.Sum(s => s.DR) + shares.Sum(s => s.DR)
                        + Others.Sum(s => s.DR) + clinical.Sum(s => s.DR) + ai.Sum(s => s.DR)
                        + tractor.Sum(s => s.DR) + variance.Sum(s => s.DR) + carryforward.Sum(s => s.DR) + extension.Sum(s => s.DR) + SMS.Sum(s => s.DR);
                        var subsidy = 0;
                        _context.DTransportersPayRolls.Add(new DTransportersPayRoll
                        {
                            Code = transporter.TransCode,
                            Amnt = amount,
                            Subsidy = subsidy,
                            GrossPay = amount + subsidy,
                            QntySup = (double?)p.Sum(s => s.Qsupplied),
                            Advance = advance.Sum(s => s.DR),
                            extension = extension.Sum(s => s.DR),
                            SMS = SMS.Sum(s => s.DR),
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
                            Yyear = period.EndDate.Year,
                            Mmonth = period.EndDate.Month,
                            AccNo = transporter.Accno,
                            BBranch = transporter.Bbranch,
                            EndPeriod = period.EndDate,
                            Rate = 0,
                            SaccoCode = sacco,
                            AuditId = loggedInUser,
                            Branch = transporter.Tbranch
                        });
                    }
                });
            }
            _context.SaveChanges();
            //await _context.SaveChangesAsync();
            _notyf.Success("Payroll processed successfully");
            return RedirectToAction(nameof(Index));
        }

        private void calcstandingorder(DateTime startDate, DateTime endDate, string sacco, string loggedInUser)
        {
            var activeorders = _context.StandingOrder.Where(o => o.StartDate <= endDate && o.SaccoCode == sacco && !o.Status).ToList();
            activeorders.ForEach(o =>
            {
                var exists = _context.ProductIntake.Any(i => i.Sno == o.Sno && i.TransDate >= startDate && i.TransDate <= endDate
                && i.Remarks == "Standing Order"
                && i.Description == o.Description && i.SaccoCode == sacco);
                if (!exists)
                {
                    var check = _context.ProductIntake.Where(m => m.Sno.ToUpper().Equals(o.Sno.ToUpper()) && m.Description.ToUpper().Equals(o.Zone.ToUpper())
                    && m.SaccoCode == sacco && m.Remarks == "Standing Order").Sum(g => g.DR);
                    if (o.Duration > check)
                    {
                        decimal deductamount = (decimal)(o.Duration - check);
                        if (deductamount < o.Amount)
                            deductamount = deductamount;
                        else
                            deductamount = (decimal)o.Amount;

                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = o.Sno.ToUpper(),
                            TransDate = endDate,
                            TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                            ProductType = o?.Zone ?? "",
                            Qsupplied = 0,
                            Ppu = 0,
                            CR = 0,
                            DR = deductamount,
                            Balance = 0,
                            Description = o?.Description ?? "",
                            TransactionType = TransactionType.Deduction,
                            Paid = false,
                            Remarks = "Standing Order",
                            AuditId = loggedInUser,
                            Auditdatetime = DateTime.Now,
                            Branch = "",
                            SaccoCode = sacco,
                            DrAccNo = "",
                            CrAccNo = "",
                            Posted = false
                        });
                    }
                }
            });

            _context.SaveChanges();
        }

        private async Task calcDefaultdeductions(DateTime startDate, DateTime endDate, string sacco, string branchName, string loggedInUser)
        {
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var productIntakes = await _context.ProductIntake
                .Where(p => p.TransDate >= startDate && p.TransDate <= endDate &&
                p.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && p.Branch.ToUpper().Equals(branchName.ToUpper())).ToListAsync();
            var intakes = productIntakes.GroupBy(p => p.Sno.Trim().ToUpper()).ToList();
            intakes.ForEach(g => {
                var totalkgs = g.Where(l => l.TransactionType == TransactionType.Intake || l.TransactionType == TransactionType.Correction).Sum(w => w.Qsupplied);
                if (totalkgs > 0)
                {
                    var eachdeductions = _context.d_PreSets.Where(l => l.Sno == g.Key && !l.Stopped
                    && l.saccocode.ToUpper().Equals(sacco.ToUpper()) && l.BranchCode.ToUpper().Equals(branchName.ToUpper())).ToList();
                    if (eachdeductions.Any())
                    {
                        var eachdeductionsselect = eachdeductions.GroupBy(p => p.Deduction.Trim().ToUpper()).Distinct().ToList();
                        eachdeductionsselect.ForEach(n => {
                            var Checkanydefaultdeduction = n.FirstOrDefault();
                            var bonus = eachdeductions.FirstOrDefault(m => m.Deduction == Checkanydefaultdeduction.Deduction).Rate;
                            if (Checkanydefaultdeduction.Rated == true)
                            {
                                totalkgs = (decimal)(totalkgs);
                                bonus = totalkgs * Checkanydefaultdeduction.Rate;
                            }
                            var glbonus = bonus;
                            if (Checkanydefaultdeduction.Deduction.ToUpper().Equals("Shares".ToUpper()))
                            {
                                var sharespaid = _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                                && f.TransDate >= startDate && f.TransDate <= endDate && f.Type != "Checkoff" && f.Pmode != "Checkoff"
                                && f.Sno.ToUpper().Equals(g.Key.ToUpper())).Sum(m => m.Amount);
                                if (sharespaid > 0)
                                {
                                    if (sharespaid < bonus)
                                    {
                                        bonus = bonus - sharespaid;
                                        glbonus = bonus;

                                        _context.DShares.Add(new DShare
                                        {
                                            Sno = g.Key.Trim().ToUpper(),
                                            Type = "Checkoff",
                                            Pmode = "Checkoff",
                                            Amount = (decimal)bonus,
                                            Period = DateTime.Today.Month.ToString(),
                                            TransDate = endDate,
                                            AuditDateTime = DateTime.Now,
                                            SaccoCode = sacco,
                                            zone = Checkanydefaultdeduction.BranchCode,
                                            AuditId = loggedInUser,
                                        });
                                    }
                                    else
                                    {
                                        bonus = 0;
                                        glbonus = sharespaid;
                                    }

                                }

                            }

                            if (bonus > 0)
                            {
                                _context.ProductIntake.Add(new ProductIntake
                                {
                                    Sno = g.Key.Trim().ToUpper(),
                                    TransDate = endDate,
                                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                                    ProductType = Checkanydefaultdeduction.Deduction,
                                    Qsupplied = 0,
                                    Ppu = 0,
                                    CR = 0,
                                    DR = bonus,
                                    Balance = 0,
                                    Description = Checkanydefaultdeduction.Remark,
                                    TransactionType = TransactionType.Deduction,
                                    Remarks = Checkanydefaultdeduction.Remark,
                                    AuditId = loggedInUser,
                                    Auditdatetime = DateTime.Now,
                                    Branch = branchName,
                                    SaccoCode = sacco,
                                    DrAccNo = "0",
                                    CrAccNo = "0"
                                });
                            }
                            var glsforbonus = _context.DDcodes.FirstOrDefault(m => m.Description.ToUpper().Equals(Checkanydefaultdeduction.Deduction.ToUpper()));

                            _context.Gltransactions.Add(new Gltransaction
                            {
                                AuditId = loggedInUser,
                                TransDate = endDate,
                                Amount = (decimal)glbonus,
                                AuditTime = DateTime.Now,
                                Source = g.Key.Trim().ToUpper(),
                                TransDescript = Checkanydefaultdeduction.Deduction,
                                Transactionno = $"{loggedInUser}{DateTime.Now}",
                                SaccoCode = sacco,
                                DrAccNo = glsforbonus.Dedaccno,
                                CrAccNo = glsforbonus.Contraacc,
                            });
                        });
                    };
                }
            });
        }

        private async Task ConsolidateTranspoterIntakes(DateTime startDate, DateTime endDate, string sacco, string branchName, string loggedInUser)
        {
            var transporterIntakes = _context.DTransports.Join(_context.ProductIntake,
                t => t.Sno.Trim().ToUpper(),
                i => i.Sno.Trim().ToUpper(),
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
                    t.Startdate,
                    t.DateInactivate,
                    i.TransactionType
                }).Where(i => i.TransDate >= startDate && i.TransDate <= endDate && i.SaccoCode == sacco
                && i.Branch.ToUpper().Equals(branchName.ToUpper())
                && (i.TransactionType == TransactionType.Correction || i.TransactionType == TransactionType.Intake)).ToList();

            var intakes = transporterIntakes.GroupBy(i => i.TransCode.Trim().ToUpper()).ToList();
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
                        if (intake.Rate > 0)
                        {
                            _context.ProductIntake.Add(new ProductIntake
                            {
                                Sno = intake.Sno,
                                TransDate = endDate,
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
                        }

                        var product = intake?.ProductType ?? "";
                        var price = _context.DPrices.FirstOrDefault(p => p.Products.ToUpper().Equals(product.ToUpper()));
                        // Credit transpoter transport amount

                        var TBprice = _context.DTransporters.FirstOrDefault(j => j.ParentT == sacco
                         && j.Tbranch.ToUpper().Equals(branchName.ToUpper())
                         && j.TransCode.ToUpper().Equals(intake.TransCode.Trim().ToUpper()));

                        decimal? TPrice = 0;
                        if (intake.Rate == 0)
                        {
                            TPrice = s.Sum(t => t.Qsupplied) * (decimal)TBprice.Rate;
                        }
                        else
                        {
                            TPrice = s.Sum(t => t.Qsupplied) * intake.Rate;
                        }

                        cr = TPrice;
                        dr = 0;
                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = intake.TransCode.Trim().ToUpper(),
                            TransDate = endDate,
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
                && p.EndofPeriod == endDate && p.Sno.ToUpper().Equals(s.Sno.ToUpper())).ToList();

                var netPay = payrolls.Sum(p => p.Npay);
                var debited = _context.ProductIntake.Any(i => i.SaccoCode == sacco && i.Sno.ToUpper().Equals(s.Sno.ToUpper())
                && i.TransDate >= nextMonth && i.Description == "Carry Forward");
                if (netPay < 0 && !debited)
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
                        Branch = s.Branch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                }
            });

            var Transporters = _context.DTransporters.Where(s => s.ParentT == sacco).ToList();
            Transporters.ForEach(s =>
            {
                var payrolls = _context.DTransportersPayRolls.Where(p => p.SaccoCode == sacco
                && p.EndPeriod == endDate && p.Code.ToUpper().Equals(s.TransCode.ToUpper())).ToList();

                var netPay = payrolls.Sum(p => p.NetPay);
                var debited = _context.ProductIntake.Any(i => i.SaccoCode == sacco && i.Sno.ToUpper().Equals(s.TransCode.ToUpper())
                && i.TransDate >= nextMonth && i.Description == "Carry Forward");
                if (netPay < 0 && !debited)
                {
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = s.TransCode.ToString(),
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
                        Branch = s.Tbranch,
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
