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
    public class EmpDeductionController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public EmpDeductionController(MORINGAContext context, INotyfService notyf)
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
            var productIntakes = await _context.EmpDeductions.Where(c => c.SaccoCode == sacco).ToListAsync();
            var intakes = new List<EmpBenefitDedVM>();
            foreach (var intake in productIntakes)
            {
                var emploeyeename = _context.Employees.FirstOrDefault(i => i.EmpNo == intake.EmpNo && i.SaccoCode == sacco);
                if (emploeyeename != null)
                {
                    intakes.Add(new EmpBenefitDedVM
                    {
                        Id = intake.Id,
                        EmpNo = intake.EmpNo,
                        Name = emploeyeename.Surname + " " + emploeyeename.Othernames,
                        Type = intake.DeductionType,
                        Amount = intake.Amount
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
        public async Task<IActionResult> Create([Bind("Id,EmpNo,DeductionType,Amount,Auditdate,AuditId,SaccoCode")] EmpDeduction deduction)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            if (string.IsNullOrEmpty(deduction.EmpNo))
            {
                _notyf.Error("Sorry, Kindly provide employee");
                return View(deduction);
            }
            if (string.IsNullOrEmpty(deduction.DeductionType))
            {
                _notyf.Error("Sorry, Kindly provide duductions");
                return View(deduction);
            }
            if (_context.EmpDeductions.Any(b => b.EmpNo == deduction.EmpNo
            && b.DeductionType == deduction.DeductionType && b.SaccoCode == sacco))
            {
                _notyf.Error("Sorry, Employee deduction already exist");
                return View(deduction);
            }

            if (ModelState.IsValid)
            {
                deduction.Auditdate = DateTime.Now;
                deduction.AuditId = loggedInUser;
                deduction.SaccoCode = sacco;
                _context.Add(deduction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deduction);
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
