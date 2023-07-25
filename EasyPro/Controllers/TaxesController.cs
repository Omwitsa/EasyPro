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
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class TaxesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public TaxesController(MORINGAContext context, INotyfService notyf)
        {
            utilities = new Utilities(context);
            _context = context;
            _notyf = notyf;
        }

        // GET: Taxes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.Taxes.Where(t => t.SaccoCode == sacco).ToListAsync());
        }

        // GET: Taxes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var tax = await _context.Taxes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tax == null)
            {
                return NotFound();
            }

            return View(tax);
        }

        // GET: Taxes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var glsetups = _context.Glsetups.Where(c => c.saccocode == sacco).ToList();
            ViewBag.glsetups = new SelectList(glsetups, "AccNo", "GlAccName");
            var types = new string[] { "Sales", "Purchases" };
            ViewBag.types = new SelectList(types);
        }

        // POST: Taxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Computation,GlAccount,Rate,Scope,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] Tax tax)
        {
            utilities.SetUpPrivileges(this);
            tax.Rate = tax?.Rate ?? 0;
            if (tax.Rate < 1)
            {
                SetInitialValues();
                _notyf.Error("Sorry, Kindly provide tax rate");
                return View(tax);
            }
            
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (ModelState.IsValid)
            {
                tax.Id = Guid.NewGuid();
                tax.SaccoCode = sacco;
                _context.Add(tax);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tax);
        }

        // GET: Taxes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            if (id == null)
            {
                SetInitialValues();
                return NotFound();
            }

            var tax = await _context.Taxes.FindAsync(id);
            if (tax == null)
            {
                SetInitialValues();
                return NotFound();
            }
            return View(tax);
        }

        // POST: Taxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Type,Computation,GlAccount,Rate,Scope,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] Tax tax)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != tax.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tax.Rate = tax?.Rate ?? 0;
                    if (tax.Rate < 1)
                    {
                        SetInitialValues();
                        _notyf.Error("Sorry, Kindly provide tax rate");
                        return View(tax);
                    }
                    _context.Update(tax);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxExists(tax.Id))
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
            return View(tax);
        }

        // GET: Taxes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var tax = await _context.Taxes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tax == null)
            {
                return NotFound();
            }

            return View(tax);
        }

        // POST: Taxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            utilities.SetUpPrivileges(this);
            var tax = await _context.Taxes.FindAsync(id);
            _context.Taxes.Remove(tax);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaxExists(Guid id)
        {
            return _context.Taxes.Any(e => e.Id == id);
        }
    }
}
