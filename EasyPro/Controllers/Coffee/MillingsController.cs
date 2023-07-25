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
    public class MillingsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public MillingsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Millers(Millers miller)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            miller.saccocode = sacco;
            miller.AuditDateTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(miller);
                await _context.SaveChangesAsync();
                _notyf.Success("Intake saved successfully");
                 return RedirectToAction(nameof(MillersIndex));
            }
            return View(miller);
        }
        public async Task<IActionResult> Millers()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return View();
        }
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            var millers = await _context.Milling.Where(b => b.saccocode == sacco).ToListAsync();
            return View(millers.OrderByDescending(n => n.Date));
        }
        // GET: Millings    
        public async Task<IActionResult> MillersIndex()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var millers = await _context.Millers.Where(b=>b.saccocode == sacco).ToListAsync();
            return View(millers.OrderByDescending(n=>n.Name));
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

        // GET: Millings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling = await _context.Milling
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milling == null)
            {
                return NotFound();
            }

            return View(milling);
        }

        // GET: Millings/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            return View();
        }

        // POST: Millings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Factory,saccocode,Category,Kgs,Miller,Date,AuditDateTime")] Milling milling)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            milling.saccocode = sacco;
            milling.AuditDateTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(milling);
                await _context.SaveChangesAsync();
                _notyf.Success("Record saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(milling);
        }

        // GET: Millings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling = await _context.Milling.FindAsync(id);
            if (milling == null)
            {
                return NotFound();
            }
            return View(milling);
        }

        // POST: Millings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Factory,saccocode,Category,Kgs,Miller,Date,AuditDateTime")] Milling milling)
        {
            if (id != milling.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(milling);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MillingExists(milling.Id))
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
            return View(milling);
        }

        // GET: Millings/Delete/5
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

            var milling = await _context.Milling
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milling == null)
            {
                _notyf.Error("Sorry, No record to delete");
                return NotFound();
            }

            return View(milling);
        }

        // POST: Millings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var milling = await _context.Milling.FindAsync(id);
            _context.Milling.Remove(milling);
            await _context.SaveChangesAsync();
            _notyf.Success("Record Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool MillingExists(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return _context.Milling.Any(e => e.Id == id);
        }
    }
}
