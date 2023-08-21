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
    public class DeliveryNotesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public DeliveryNotesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DeliveryNotes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var delivery = _context.DeliveryNote.ToList();
            return View(delivery.OrderByDescending(n => n.Date).ToList());
        }
        private void getdefaults()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var factories = _context.DBranch.Where(b => b.Bcode == sacco).OrderBy(n => n.Bname).ToList();
            ViewBag.Branch = new SelectList(factories.OrderBy(n => n.Bname).Select(k => k.Bname).ToList());
            List<SelectListItem> Coffee = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "FIRSTS" },
                new SelectListItem { Text = "SECONDS" },
                new SelectListItem { Text = "THIRDS" },
                new SelectListItem { Text = "LIGHTS" },
                new SelectListItem { Text = "MBUNI" },
            };
             ViewBag.Coffee = Coffee;

            List<SelectListItem> comment = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "SAN" },
                new SelectListItem { Text = "UTZ" },
                new SelectListItem { Text = "4C" },
                new SelectListItem { Text = "CP" },
                new SelectListItem { Text = "FLO" },
            };
            ViewBag.Comment = comment;
        }
        // GET: DeliveryNotes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (id == null)
            {
                return NotFound();
            }

            var deliveryNote = await _context.DeliveryNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryNote == null)
            {
                return NotFound();
            }

            return View(deliveryNote);
        }

        // GET: DeliveryNotes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            getdefaults();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return View();
        }

        // POST: DeliveryNotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Name,NoteNo,GrowersCode,CoffeType,VehicleNo,NoBags,Driver,Comment,UserId,AuditDate,Branch,saccocode")] DeliveryNote deliveryNote)
        {
            utilities.SetUpPrivileges(this);
            getdefaults();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (ModelState.IsValid)
            {
                deliveryNote.AuditDate = DateTime.Now;
                deliveryNote.UserId = loggedInUser;
                deliveryNote.saccocode = sacco;
                _context.Add(deliveryNote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryNote);
        }

        // GET: DeliveryNotes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (id == null)
            {
                getdefaults();
                return NotFound();
            }

            var deliveryNote = await _context.DeliveryNote.FindAsync(id);
            if (deliveryNote == null)
            {
                getdefaults();
                return NotFound();
            }
            return View(deliveryNote);
        }

        // POST: DeliveryNotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Date,Name,NoteNo,GrowersCode,CoffeType,VehicleNo,NoBags,Driver,Comment,UserId,AuditDate,Branch,saccocode")] DeliveryNote deliveryNote)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (id != deliveryNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deliveryNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryNoteExists(deliveryNote.Id))
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
            return View(deliveryNote);
        }

        // GET: DeliveryNotes/Delete/5
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

            var deliveryNote = await _context.DeliveryNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryNote == null)
            {
                return NotFound();
            }

            return View(deliveryNote);
        }

        // POST: DeliveryNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var deliveryNote = await _context.DeliveryNote.FindAsync(id);
            _context.DeliveryNote.Remove(deliveryNote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryNoteExists(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            return _context.DeliveryNote.Any(e => e.Id == id);
        }
    }
}
