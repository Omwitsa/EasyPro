using System;
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

namespace EasyPro.Controllers
{
    public class EntitlementTypesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public EntitlementTypesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: EntitlementTypes
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            return View(await _context.EntitlementType.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: EntitlementTypes/Details/5
        public async Task<IActionResult> Details(long? id)
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

            var entitlementType = await _context.EntitlementType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entitlementType == null)
            {
                return NotFound();
            }

            return View(entitlementType);
        }

        // GET: EntitlementTypes/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            return View(new EntitlementType { 
            Taxable=true
            });
        }

        // POST: EntitlementTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Taxable,SaccoCode")] EntitlementType entitlementType)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var CheckifExist = _context.EntitlementType.Any(i => i.SaccoCode == sacco && i.Name == entitlementType.Name);
            if (CheckifExist == true)
            {
                _notyf.Error("Sorry, The Name Already Exist.");
                return View();
            }
            if (ModelState.IsValid)
            {
                entitlementType.SaccoCode = sacco;
                _context.Add(entitlementType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entitlementType);
        }

        // GET: EntitlementTypes/Edit/5
        public async Task<IActionResult> Edit(long? id)
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

            var entitlementType = await _context.EntitlementType.FindAsync(id);
            if (entitlementType == null)
            {
                return NotFound();
            }
            return View(entitlementType);
        }

        // POST: EntitlementTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Taxable,SaccoCode")] EntitlementType entitlementType)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id != entitlementType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    entitlementType.SaccoCode = sacco;
                    _context.Update(entitlementType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntitlementTypeExists(entitlementType.Id))
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
            return View(entitlementType);
        }

        // GET: EntitlementTypes/Delete/5
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

            var entitlementType = await _context.EntitlementType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entitlementType == null)
            {
                return NotFound();
            }

            return View(entitlementType);
        }

        // POST: EntitlementTypes/Delete/5
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
            var entitlementType = await _context.EntitlementType.FindAsync(id);
            _context.EntitlementType.Remove(entitlementType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntitlementTypeExists(long id)
        {
            return _context.EntitlementType.Any(e => e.Id == id);
        }
    }
}
