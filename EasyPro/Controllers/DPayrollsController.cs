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
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var payroll = await _context.DPayrolls.Where(i=>i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).Join(
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var payrolls = _context.DPayrolls
                .Where(p => p.Yyear == endDate.Year && p.Mmonth == endDate.Month 
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            if (payrolls.Any())
            {
                _context.DPayrolls.RemoveRange(payrolls);
                _context.SaveChanges();
            }
            var payrolls1 = _context.DTransportersPayRolls
                .Where(p => p.Yyear == endDate.Year && p.Mmonth == endDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            if (payrolls1.Any())
            {
                _context.DTransportersPayRolls.RemoveRange(payrolls1);
                _context.SaveChanges();
            }

            var supplierNos = _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper()))
                .Select(s => s.Sno.ToString());
            var productIntakes = _context.ProductIntake
                .Where(p => p.TransDate >= startDate && p.TransDate <= endDate 
                && supplierNos.Contains(p.Sno) && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var intakes = productIntakes.GroupBy(p => p.Sno).ToList();
            intakes.ForEach(p =>
            {
                p.ToList().ForEach(i =>
                {
                    i.Paid = true;
                });

                var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                var transport = p.Where(k => k.Description.ToLower().Contains("transport"));
                var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet"));
                var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);

                
                var payroll = new DPayroll();
                long.TryParse(p.Key, out long sno);
                var supplier = _context.DSuppliers
                    .FirstOrDefault(s => s.Sno == sno && s.Scode.ToUpper().Equals(sacco.ToUpper()));
                if (supplier != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    var Tot = advance.Sum(s => s.DR)+ transport.Sum(s => s.DR)+ agrovet.Sum(s => s.DR)+ bonus.Sum(s => s.DR) + shares.Sum(s => s.DR);
                    _context.DPayrolls.Add(new DPayroll { 
                        Sno = (int?)supplier.Sno,
                        Gpay = p.Sum(s => s.CR),
                        KgsSupplied = (double?)p.Sum(s => s.Qsupplied),
                        Advance = advance.Sum(s => s.DR),
                        Others = 0,
                        Transport = transport.Sum(s => s.DR),
                        Agrovet = agrovet.Sum(s => s.DR),
                        Bonus = bonus.Sum(s => s.DR),
                        Hshares = shares.Sum(s => s.DR),
                        Tdeductions = Tot,
                        Npay = p.Sum(s => s.CR) - debits - Tot,
                        Yyear = endDate.Year,
                        Mmonth = endDate.Month,
                        Bank= supplier.Bcode,
                        AccountNumber = supplier.AccNo,
                        Bbranch = supplier.Bbranch,
                        IdNo = supplier.IdNo,
                        EndofPeriod = endDate,
                        SaccoCode = sacco,
                        Auditid = loggedInUser
                    });
                }
            });

            var transpoterCodes = _context.DTransporters
                .Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper()))
                .Select(s => s.TransCode.ToUpper());
            productIntakes = _context.ProductIntake
                .Where(p => p.TransDate >= startDate && p.TransDate <= endDate
                && transpoterCodes.Contains(p.Sno.Trim().ToUpper()) && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            intakes = productIntakes.GroupBy(p => p.Sno).ToList();
            intakes.ForEach(p =>
            {
                p.ToList().ForEach(i =>
                {
                    i.Paid = true;
                });

                var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                var transport = p.Where(k => k.ProductType.ToLower().Contains("transport"));
                var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet"));
                var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);

                var payroll = new DTransportersPayRoll();
                long.TryParse(p.Key, out long sno);
                
                var transporter = _context.DTransporters
                   .FirstOrDefault(s => s.TransCode.ToUpper().Equals(p.Key.ToUpper()) && s.ParentT.ToUpper().Equals(sacco.ToUpper()));
                if (transporter != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    var amount = p.Sum(s => s.CR);
                    var Tot = advance.Sum(s => s.DR) + agrovet.Sum(s => s.DR) + shares.Sum(s => s.DR);
                    var subsidy = 0;
                    _context.DTransportersPayRolls.Add(new DTransportersPayRoll
                    {
                        Code = transporter.TransCode,
                        Amnt = amount,
                        Subsidy = subsidy,
                        GrossPay = amount + subsidy,
                        QntySup = (double?)p.Sum(s => s.Qsupplied),
                        Advance = advance.Sum(s => s.DR),
                        Others = 0,
                        Agrovet = agrovet.Sum(s => s.DR),
                        Hshares = shares.Sum(s => s.DR),
                        Totaldeductions= Tot,
                        NetPay = (amount + subsidy) - debits - Tot,
                        BankName = transporter.Bcode,
                        Yyear = endDate.Year,
                        Mmonth = endDate.Month,
                        AccNo = transporter.Accno,
                        Branch = transporter.Bbranch,
                        EndPeriod = endDate,
                        Rate = 0,
                        SaccoCode = sacco,
                        AuditId = loggedInUser
                    });
                }
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
    }
}
