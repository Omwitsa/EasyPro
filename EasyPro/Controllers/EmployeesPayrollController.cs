using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class EmployeesPayrollController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public EmployeesPayrollController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: EmpBenefits
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var productIntakes = await _context.EmployeesPayroll.Where(c => c.SaccoCode == sacco).ToListAsync();
            var intakes = new List<EmpEmployeePayrollVM>();
            foreach (var intake in productIntakes)
            {
                var emploeyepayroll = _context.Employees.FirstOrDefault(i => i.EmpNo == intake.EmpNo && i.SaccoCode == sacco);
                if (emploeyepayroll != null)
                {
                    intakes.Add(new EmpEmployeePayrollVM
                    {
                        Id = intake.Id,
                        EmpNo = intake.EmpNo,
                        Name = emploeyepayroll.Surname + " " + emploeyepayroll.Othernames,
                        Basic = intake.Basic,
                        Allowance = intake.Allowance,
                        Gross = intake.Gross,
                        NHIF = intake.NHIF,
                        NSSF = intake.NSSF,
                        PAYE = intake.PAYE,
                        OTHERS = intake.OTHERS,
                        STORE = intake.STORE,
                        TOTALDED = intake.TOTALDED,
                        NETPAY = intake.NETPAY,
                        ENDINGMONTH = intake.ENDINGMONTH,
                        User = intake.Audituser
                    });
                }
            }
            return View(intakes);
            //return View(await _context.EmpBenefits.Where(i=>i.SaccoCode==sacco).ToListAsync());
        }

        // GET: EmpBenefits/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            if (id == null)
            {
                return NotFound();
            }

            var empBenefit = await _context.EmpBenefits
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empBenefit == null)
            {
                return NotFound();
            }

            return View(empBenefit);
        }

        // GET: EmpBenefits/Create
        public async Task<IActionResult> Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetInitialValues();
            return View();
        }

        private async Task SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var deductions = await _context.DeductionType.Where(e => e.SaccoCode == sacco)
                .Select(e => e.Name).ToListAsync();
            ViewBag.deductions = new SelectList(deductions);
            var employees = await _context.Employees.Where(e => e.SaccoCode == sacco).ToListAsync();
            ViewBag.employees = new SelectList(employees, "EmpNo", "Othernames");
        }

        // POST: EmpBenefits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ENDINGMONTH")] EmployeesPayroll period)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetInitialValues();
            var startDate = new DateTime(period.ENDINGMONTH.Year, period.ENDINGMONTH.Month, 1);
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            var payrolls = await _context.EmployeesPayroll
               .Where(p => p.ENDINGMONTH == monthsLastDate).ToListAsync();
            if (payrolls.Any())
            {
                _context.EmployeesPayroll.RemoveRange(payrolls);
                _context.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                var employees = _context.Employees.Where(m => m.SaccoCode == sacco).ToList();
                var getbefits = _context.EmpBenefits.Where(n => n.SaccoCode == sacco).ToList();
                var employeesded = _context.EmployeesDed.Where(m => m.saccocode == sacco
                && m.Date >= startDate && m.Date <= monthsLastDate).ToList();
                var employeesdeductions = _context.EmpDeductions.Where(m => m.SaccoCode == sacco).ToList();
                var groupemployees = employees.GroupBy(n => n.EmpNo).ToList();
                groupemployees.ForEach(s => {
                    var getdefaultdeductions = employeesdeductions.Where(n => n.EmpNo == s.Key).ToList();
                    var getdeductamount = employeesded.Where(m => m.Empno == s.Key).ToList();

                    var Basic = getbefits.Where(b => b.EntType.Contains("basic")).Sum(v => v.Amount);
                    var Allowance = getbefits.Where(b => b.EntType.Contains("allowance")).Sum(v => v.Amount);
                    var NHIF = getdefaultdeductions.Where(b => b.DeductionType.Contains("NHIF")).Sum(v => v.Amount);
                    var NSSF = getdefaultdeductions.Where(b => b.DeductionType.Contains("NSSF")).Sum(v => v.Amount);
                    var PAYE = getdefaultdeductions.Where(b => b.DeductionType.Contains("PAYE")).Sum(v => v.Amount);
                    var STORE = getdeductamount.Where(X => X.Deduction == "Store").Sum(v => v.Amount);
                    var OTHERS = getdeductamount.Where(X => X.Deduction != "Store").Sum(v => v.Amount);
                    var dcodes = getdefaultdeductions.Where(c => c.DeductionType.ToLower().Contains("basic") || c.DeductionType.Contains("PAYE") ||
                    c.DeductionType.ToLower().Contains("allowance") || c.DeductionType.Contains("NHIF") || c.DeductionType.Contains("NSSF"))
                    .Select(c => c.DeductionType.ToLower()).ToList();

                    var OTHER = getdeductamount.Where(X => !dcodes.Contains(X.Deduction)).Sum(v => v.Amount);
                    var Gross = Basic + Allowance;
                    var TOTALDED = NHIF + NSSF + PAYE + STORE + OTHERS + OTHER;
                    _context.EmployeesPayroll.Add( new EmployeesPayroll {
                        EmpNo = s.Key,
                        Basic = Basic,
                        Allowance = Allowance,
                        Gross = Gross,
                        NHIF = NHIF,
                        NSSF = NSSF,
                        PAYE = PAYE,
                        STORE = STORE,
                        OTHERS = OTHERS,
                        OTHERDED = OTHER,
                        TOTALDED = TOTALDED,
                        NETPAY = Gross- TOTALDED,
                        ENDINGMONTH = monthsLastDate,
                        Audituser = loggedInUser,
                        auditdatetime = DateTime.Now,
                        SaccoCode = sacco
                    });

                });
                await _context.SaveChangesAsync();
                _notyf.Success("Processing completed successfuly");
                return RedirectToAction(nameof(Index));
            }
            _notyf.Error("Sorry, Kindly process again");
            return View();
        }

        // GET: EmpBenefits/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var empDeduction = await _context.EmpDeductions.FindAsync(id);
            if (empDeduction == null)
            {
                return NotFound();
            }
            return View(empDeduction);
        }

        // POST: EmpBenefits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,EmpNo,DeductionType,Amount,Auditdate,AuditId,SaccoCode")] EmpDeduction deduction)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id != deduction.Id)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(deduction.EmpNo))
            {
                _notyf.Error("Sorry, Kindly provide employee");
                return View(deduction);
            }
            if (string.IsNullOrEmpty(deduction.DeductionType))
            {
                _notyf.Error("Sorry, Kindly provide deduction");
                return View(deduction);
            }
            if (_context.EmpDeductions.Any(b => b.EmpNo == deduction.EmpNo
            && b.DeductionType == deduction.DeductionType && b.SaccoCode == sacco && b.Id != deduction.Id))
            {
                _notyf.Error("Sorry, Employee deduction already exist");
                return View(deduction);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deduction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpDeductionExists(deduction.Id))
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
            return View(deduction);
        }

        // GET: EmpBenefits/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var empBenefit = await _context.EmpDeductions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empBenefit == null)
            {
                return NotFound();
            }

            return View(empBenefit);
        }

        // POST: EmpBenefits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var deduction = await _context.EmpDeductions.FindAsync(id);
            _context.EmpDeductions.Remove(deduction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpDeductionExists(long id)
        {
            return _context.EmpDeductions.Any(e => e.Id == id);
        }
    }
}
