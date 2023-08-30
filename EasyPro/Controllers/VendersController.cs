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
    public class VendersController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public VendersController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Venders
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.Venders.Where(v => v.SaccoCode == sacco).ToListAsync());
        }

        // GET: Venders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var vender = await _context.Venders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vender == null)
            {
                return NotFound();
            }

            return View(vender);
        }

        // GET: Venders/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glsetups = _context.Glsetups.Where(c => c.saccocode == sacco).ToList();
            ViewBag.glsetups = new SelectList(glsetups, "AccNo", "GlAccName");
        }

        // POST: Venders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Street1,Street2,City,Country,PhoneNo,Mobile,Email,WebSite,SalesPerson,PurchasePaymentTerms,SalesPaymentTerms,FiscalPosition,Ref,Industry,APGlAccount,Bank,BankAccount,Notes,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] Vender vender)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                vender.Id = Guid.NewGuid();
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                vender.SaccoCode = sacco;
                _context.Add(vender);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vender);
        }

        // GET: Venders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var vender = await _context.Venders.FindAsync(id);
            if (vender == null)
            {
                return NotFound();
            }
            return View(vender);
        }

        // POST: Venders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Street1,Street2,City,Country,PhoneNo,Mobile,Email,WebSite,SalesPerson,PurchasePaymentTerms,SalesPaymentTerms,FiscalPosition,Ref,Industry,APGlAccount,Bank,BankAccount,Notes,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] Vender vender)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != vender.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                    vender.SaccoCode = sacco;
                    _context.Update(vender);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenderExists(vender.Id))
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
            return View(vender);
        }

        // GET: Venders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var vender = await _context.Venders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vender == null)
            {
                return NotFound();
            }

            return View(vender);
        }

        // POST: Venders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var vender = await _context.Venders.FindAsync(id);
            _context.Venders.Remove(vender);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenderExists(Guid id)
        {
            return _context.Venders.Any(e => e.Id == id);
        }
    }
}
