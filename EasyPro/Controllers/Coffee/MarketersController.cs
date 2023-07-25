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
    public class MarketersController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public MarketersController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Marketers
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var market = _context.Marketer.Where(n=>n.saccocode == sacco).ToList();
            return View(market.OrderByDescending(n=>n.Date).ToList());
        }

        public async Task<IActionResult> MarketerRegIndex()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var market = _context.MarketerReg.Where(n => n.saccocode == sacco).ToList();
            return View(market.OrderByDescending(n => n.AuditDateTime).ToList());
        }
        private void getdefaults()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var partchments = _context.Parchment.OrderBy(n => n.PName).ToList();
            var factories = _context.DBranch.Where(b=>b.Bcode== sacco).OrderBy(n => n.Bname).ToList();
            var Millers = _context.Millers.Where(b=>b.saccocode == sacco).OrderBy(n => n.Name).ToList();
            var Marketer = _context.MarketerReg.Where(b=>b.saccocode == sacco).OrderBy(n => n.Name).ToList();

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
        // GET: Marketers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketer = await _context.Marketer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marketer == null)
            {
                return NotFound();
            }

            return View(marketer);
        }

        // GET: Marketers/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            return View();
        }
        public IActionResult MarketerRegCreate()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            getdefaults();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarketerRegCreate(MarketerReg marketerReg) 
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            marketerReg.saccocode = sacco;
            marketerReg.AuditDateTime = DateTime.Now;
            getdefaults();
            if (ModelState.IsValid)
            {
                _context.Add(marketerReg);
                await _context.SaveChangesAsync();
                _notyf.Success("Records saved successfully");
                return RedirectToAction(nameof(MarketerRegIndex));
            }
            return View(marketerReg);
        }

        // POST: Marketers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,saccocode,MarketerName,Category,Grade,Class,Kgs,Date,Amount,AuditDateTime")] Marketer marketer)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            marketer.saccocode = sacco;
            marketer.AuditDateTime = DateTime.Now;
            getdefaults();
            if (ModelState.IsValid)
            {
                _context.Add(marketer);
                await _context.SaveChangesAsync();
                _notyf.Success("Records saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(marketer);
        }

        // GET: Marketers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketer = await _context.Marketer.FindAsync(id);
            if (marketer == null)
            {
                return NotFound();
            }
            return View(marketer);
        }

        // POST: Marketers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,saccocode,MarketerName,Category,Grade,Class,Kgs,Date,Amount,AuditDateTime")] Marketer marketer)
        {
            if (id != marketer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marketer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketerExists(marketer.Id))
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
            return View(marketer);
        }

        // GET: Marketers/Delete/5
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

            var marketer = await _context.Marketer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marketer == null)
            {
                return NotFound();
            }

            return View(marketer);
        }

        // POST: Marketers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var marketer = await _context.Marketer.FindAsync(id);
            _context.Marketer.Remove(marketer);
            await _context.SaveChangesAsync();
            _notyf.Success("Records Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool MarketerExists(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return _context.Marketer.Any(e => e.Id == id);
        }
    }
}
