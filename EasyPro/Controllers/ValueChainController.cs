using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.TranssupplyVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class ValueChainController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        private readonly IReportProvider _reportProvider;
        public ValueChainController(MORINGAContext context, INotyfService notyf, IReportProvider reportProvider)
        {
            _context = context;
            _notyf = notyf;
            _reportProvider = reportProvider;
            utilities = new Utilities(context);
        }

        // GET: AgReceipts
        public async Task<IActionResult> Index()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var receipts = _context.ValueChain
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()));

            return View(await receipts.OrderByDescending(s => s.Id).ToListAsync());
        }
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productCat = await _context.ValueChain
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCat == null)
            {
                return NotFound();
            }

            return View(productCat);
        }

        // GET: ProductCats/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: ProductCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ValueChain cIGs)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var extist = _context.ValueChain.Any(c => c.Name.ToUpper().Equals(cIGs.Name.ToUpper())
            && c.saccocode == sacco);
            if (extist)
            {
                _notyf.Error("Sorry, Category already exist");
                return View(cIGs);
            }

            if (ModelState.IsValid)
            {
                cIGs.saccocode = sacco;
                _context.Add(cIGs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cIGs);
        }

        // GET: ProductCats/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productCat = await _context.ValueChain.FindAsync(id);
            if (productCat == null)
            {
                return NotFound();
            }
            return View(productCat);
        }

        // POST: ProductCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] ValueChain productCat)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != productCat.Id)
            {
                return NotFound();
            }

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var extist = _context.ValueChain.Any(c => c.Name.ToUpper().Equals(productCat.Name.ToUpper())
            && c.saccocode == sacco && c.Id != productCat.Id);
            if (extist)
            {
                _notyf.Error("Sorry, ValueChain already exist");
                return View(productCat);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productCat.saccocode = sacco;
                    _context.Update(productCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCatExists(productCat.Id))
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
            return View(productCat);
        }

        // GET: ProductCats/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productCat = await _context.ValueChain
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCat == null)
            {
                return NotFound();
            }

            return View(productCat);
        }

        // POST: ProductCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var productCat = await _context.ValueChain.FindAsync(id);
            _context.ValueChain.Remove(productCat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCatExists(long id)
        {
            return _context.ValueChain.Any(e => e.Id == id);
        }
    }
}
