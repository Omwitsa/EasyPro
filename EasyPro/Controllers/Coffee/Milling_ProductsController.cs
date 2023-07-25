using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Models.Coffee;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers.Coffee
{
    public class Milling_ProductsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public Milling_ProductsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Milling_Products
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            var market = _context.Milling_Products.Where(n => n.saccocode == sacco).ToList();
            return View(market.OrderByDescending(n=>n.Date).ToList());
        }

        private void getdefaults()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var partchments = _context.Parchment.OrderBy(n => n.PName).ToList();
            var factories = _context.DBranch.Where(b => b.Bcode == sacco).OrderBy(n => n.Bname).ToList();
            var Millers = _context.Millers.Where(b => b.saccocode == sacco).OrderBy(n => n.Name).ToList();
            var Marketer = _context.MarketerReg.Where(b => b.saccocode == sacco).OrderBy(n => n.Name).ToList();

            var parchment = _context.Parchment.OrderBy(K => K.PName).ToList();
            var grading = _context.ParchmentGrading.OrderBy(K => K.PGrading).ToList();
            var pclass = _context.ParchmentClasses.OrderBy(K => K.PClasses).ToList();

            ViewBag.Category = new SelectList(partchments.OrderBy(n => n.PName).Select(k => k.PName).ToList());
            ViewBag.Factory = new SelectList(factories.OrderBy(n => n.Bname).Select(k => k.Bname).ToList());
            ViewBag.Miller = new SelectList(Millers.OrderBy(n => n.Name).Select(k => k.Name).ToList());
            ViewBag.Marketer = new SelectList(Marketer.OrderBy(n => n.Name).Select(k => k.Name).ToList());

            ViewBag.parchment = new SelectList(parchment.Select(b => b.PName));
            ViewBag.grading = new SelectList(grading.Select(b => b.PGrading));
            ViewBag.pclass = new SelectList(pclass.Select(b => b.PClasses));

        }

        // GET: Milling_Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling_Products = await _context.Milling_Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milling_Products == null)
            {
                return NotFound();
            }

            return View(milling_Products);
        }

        // GET: Milling_Products/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            return View();
        }

        // POST: Milling_Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Factory,saccocode,Miller,Category,Grade,Class,Kgs,Date,AuditDateTime")] Milling_Products milling_Products)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            milling_Products.saccocode = sacco;
            milling_Products.AuditDateTime = DateTime.Now;
            getdefaults();
            if (ModelState.IsValid)
            {
                _context.Add(milling_Products);
                await _context.SaveChangesAsync();
                _notyf.Success("Record saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(milling_Products);
        }

        // GET: Milling_Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling_Products = await _context.Milling_Products.FindAsync(id);
            if (milling_Products == null)
            {
                return NotFound();
            }
            return View(milling_Products);
        }

        // POST: Milling_Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Factory,saccocode,Miller,Category,Grade,Class,Kgs,Date,AuditDateTime")] Milling_Products milling_Products)
        {
            if (id != milling_Products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(milling_Products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Milling_ProductsExists(milling_Products.Id))
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
            return View(milling_Products);
        }

        // GET: Milling_Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (id == null)
            {
                return NotFound();
            }

            var milling_Products = await _context.Milling_Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milling_Products == null)
            {
                return NotFound();
            }

            return View(milling_Products);
        }

        // POST: Milling_Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var milling_Products = await _context.Milling_Products.FindAsync(id);
            _context.Milling_Products.Remove(milling_Products);
            await _context.SaveChangesAsync();
            _notyf.Success("Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool Milling_ProductsExists(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return _context.Milling_Products.Any(e => e.Id == id);
        }
    }
}
