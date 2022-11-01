﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using EasyPro.ViewModels;

namespace EasyPro.Controllers
{
    public class EmpBenefitsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public EmpBenefitsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: EmpBenefits
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var productIntakes = await _context.EmpBenefits.Where(c => c.SaccoCode == sacco).ToListAsync();
            var intakes = new List<EmpBenefitVM>();
            foreach (var intake in productIntakes)
            {
                var emploeyeename = _context.Employees.FirstOrDefault(i => i.EmpNo == intake.EmpNo && i.SaccoCode == sacco);
                if (emploeyeename != null)
                {
                    intakes.Add(new EmpBenefitVM
                    {
                        EmpNo = intake.EmpNo,
                        Name = emploeyeename.Surname + " " + emploeyeename.Othernames,
                        EntType = intake.EntType,
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
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            return View();
        }

        // POST: EmpBenefits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmpNo,EntType,Amount,Auditdate,AuditId,SaccoCode")] EmpBenefit empBenefit)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (ModelState.IsValid)
            {
                empBenefit.SaccoCode = sacco;
                _context.Add(empBenefit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(empBenefit);
        }

        // GET: EmpBenefits/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var empBenefit = await _context.EmpBenefits.FindAsync(id);
            if (empBenefit == null)
            {
                return NotFound();
            }
            return View(empBenefit);
        }

        // POST: EmpBenefits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,EmpNo,EntType,Amount,Auditdate,AuditId,SaccoCode")] EmpBenefit empBenefit)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id != empBenefit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empBenefit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpBenefitExists(empBenefit.Id))
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
            return View(empBenefit);
        }

        // GET: EmpBenefits/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
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

        // POST: EmpBenefits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var empBenefit = await _context.EmpBenefits.FindAsync(id);
            _context.EmpBenefits.Remove(empBenefit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpBenefitExists(long id)
        {
            return _context.EmpBenefits.Any(e => e.Id == id);
        }
    }
}