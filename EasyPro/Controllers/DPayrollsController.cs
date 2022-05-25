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
            var payroll = await _context.DPayrolls.Join(
                _context.DSuppliers,
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
            var payrolls = _context.DPayrolls.Where(p => p.Yyear == endDate.Year && p.Mmonth == endDate.Month);
            if (payrolls.Any())
            {
                _context.DPayrolls.RemoveRange(payrolls);
                _context.SaveChanges();
            }

            var productIntakes = _context.ProductIntake
                .Where(p => p.TransDate >= startDate && p.TransDate <= endDate).ToList();
            var intakes = productIntakes.GroupBy(p => p.Sno).ToList();
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

                var payroll = new DPayroll();
                long.TryParse(p.Key, out long sno);
                if(sno > 0)
                {
                    var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno);
                    payroll.Sno = (int?)supplier.Sno;
                    payroll.Gpay = p.Sum(s => s.CR);
                    payroll.KgsSupplied = (double?)p.Sum(s => s.Qsupplied);
                    payroll.Advance = advance.Sum(s => s.DR);
                    payroll.Others = 0;
                    payroll.Transport = transport.Sum(s => s.DR);
                    payroll.Agrovet = agrovet.Sum(s => s.DR);
                    payroll.Bonus = bonus.Sum(s => s.DR);
                    payroll.Hshares = shares.Sum(s => s.DR);
                    payroll.Tdeductions = payroll.Advance + payroll.Transport + payroll.Agrovet + payroll.Bonus + payroll.Hshares;
                    var debits = corrections.Sum(s => s.DR);
                    payroll.Npay = payroll.Gpay - debits - payroll.Tdeductions;
                    payroll.Yyear = endDate.Year;
                    payroll.Mmonth = endDate.Month;
                    payroll.AccountNumber = supplier.AccNo;
                    payroll.Bbranch = supplier.Bbranch;
                    payroll.IdNo = supplier.IdNo;
                    _context.DPayrolls.Add(payroll);
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
