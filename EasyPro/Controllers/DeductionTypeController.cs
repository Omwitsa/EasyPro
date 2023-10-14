using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class DeductionTypeController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DeductionTypeController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Deductions
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.DeductionType
                .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = _context.Glsetups.Where(a => a.saccocode == sacco).ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");
        }
        // GET: Deductions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            if (id == null)
            {
                return NotFound();
            }

            var deduction = await _context.DeductionType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deduction == null)
            {
                return NotFound();
            }

            return View(deduction);
        }

        // GET: Deductions/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }

        // POST: Deductions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Accno,SaccoCode")] DeductionType deduction)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            deduction.SaccoCode = sacco;
            var dCodesExists = _context.DeductionType.Any(i => i.Name == deduction.Name && i.SaccoCode == sacco);
            if (dCodesExists)
            {
                _notyf.Error("Sorry, The Name already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                _context.Add(deduction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deduction);
        }

        // GET: Deductions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var deduction = await _context.DeductionType.FindAsync(id);
            if (deduction == null)
            {
                return NotFound();
            }
            return View(deduction);
        }

        // POST: Deductions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Accno,SaccoCode")] DeductionType deduction)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            deduction.SaccoCode = sacco;
            if (id != deduction.Id)
            {
                return NotFound();
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
                    if (!DeductionExists(deduction.Id))
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

        // GET: Deductions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var deduction = await _context.DeductionType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deduction == null)
            {
                return NotFound();
            }

            return View(deduction);
        }

        // POST: Deductions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var deduction = await _context.DeductionType.FindAsync(id);
            _context.DeductionType.Remove(deduction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeductionExists(long id)
        {
            return _context.DeductionType.Any(e => e.Id == id);
        }
    }
}
