using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Models.Coffee;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;

namespace EasyPro.Controllers.Coffee
{
    public class Pulping_DryingController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public Pulping_DryingController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Pulping_Drying
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            var pulping = _context.Pulping_Drying.Where(n=>n.saccocode == sacco).ToList();
            return View(pulping.OrderByDescending(v=>v.Date));
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

            ViewBag.Category = new SelectList(partchments.OrderBy(n => n.PName).Select(k => k.PName).ToList());
            ViewBag.Factory = new SelectList(factories.OrderBy(n => n.Bname).Select(k => k.Bname).ToList());
            ViewBag.Miller = new SelectList(Millers.OrderBy(n => n.Name).Select(k => k.Name).ToList());
            ViewBag.Marketer = new SelectList(Marketer.OrderBy(n => n.Name).Select(k => k.Name).ToList());
        }
        // GET: Pulping_Drying/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pulping_Drying = await _context.Pulping_Drying
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pulping_Drying == null)
            {
                return NotFound();
            }

            return View(pulping_Drying);
        }

        // GET: Pulping_Drying/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            getdefaults();
            return View();
        }

        // POST: Pulping_Drying/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Factory,saccocode,Category,Kgs,Date,AuditDateTime")] Pulping_Drying pulping_Drying)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            getdefaults();
            if (ModelState.IsValid)
            {
                pulping_Drying.saccocode = sacco;
                pulping_Drying.AuditDateTime = DateTime.Now;
                _context.Add(pulping_Drying);
                await _context.SaveChangesAsync();
                _notyf.Success("Records saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(pulping_Drying);
        }

        // GET: Pulping_Drying/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pulping_Drying = await _context.Pulping_Drying.FindAsync(id);
            if (pulping_Drying == null)
            {
                return NotFound();
            }
            return View(pulping_Drying);
        }

        // POST: Pulping_Drying/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Factory,saccocode,Category,Kgs,Date,AuditDateTime")] Pulping_Drying pulping_Drying)
        {
            if (id != pulping_Drying.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pulping_Drying);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Pulping_DryingExists(pulping_Drying.Id))
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
            return View(pulping_Drying);
        }

        // GET: Pulping_Drying/Delete/5
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

            var pulping_Drying = await _context.Pulping_Drying
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pulping_Drying == null)
            {
                return NotFound();
            }

            return View(pulping_Drying);
        }

        // POST: Pulping_Drying/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var pulping_Drying = await _context.Pulping_Drying.FindAsync(id);
            _context.Pulping_Drying.Remove(pulping_Drying);
            await _context.SaveChangesAsync();
            _notyf.Success("Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool Pulping_DryingExists(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return _context.Pulping_Drying.Any(e => e.Id == id);
        }
    }
}
