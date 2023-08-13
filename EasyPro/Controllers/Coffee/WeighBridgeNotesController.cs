using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Models.Coffee;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers.Coffee
{
    public class WeighBridgeNotesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public WeighBridgeNotesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: WeighBridgeNotes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            //getdefaults();
            var weighing = _context.WeighBridgeNote.Where(n => n.SaccoCode == sacco).ToList();
            return View(weighing.OrderByDescending(v => v.Date));
            //return View(await _context.WeighBridgeNotes.ToListAsync());
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

            List<SelectListItem> millerSelect = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "SAN" },
                new SelectListItem { Text = "UTZ" },
                new SelectListItem { Text = "4C" },
                new SelectListItem { Text = "CP" },
                new SelectListItem { Text = "FLO" },
            };
            ViewBag.millerSelect = millerSelect;
            ViewBag.Category = new SelectList(partchments.OrderBy(n => n.PName).Select(k => k.PName).ToList());
            ViewBag.Factory = new SelectList(factories.OrderBy(n => n.Bname).Select(k => k.Bname).ToList());
            ViewBag.Miller = new SelectList(Millers.OrderBy(n => n.Name).Select(k => k.Name).ToList());
            
        }

        // GET: WeighBridgeNotes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weighBridgeNote = await _context.WeighBridgeNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weighBridgeNote == null)
            {
                return NotFound();
            }

            return View(weighBridgeNote);
        }

        // GET: WeighBridgeNotes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            getdefaults();
            return View();
        }

        // POST: WeighBridgeNotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Miller,Factory,VehicleNo,CbkCode,OutreturnNo,FirstWeight,SecondWeight,NetWeight,Category,NoBags,Storage,RecipientName,RecipientIdno,Selection")] WeighBridgeNote weighBridgeNote)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            getdefaults();
            if (ModelState.IsValid)
            {
                weighBridgeNote.SaccoCode = sacco;
                weighBridgeNote.AuditDateTime = DateTime.Now;
                _context.Add(weighBridgeNote);
                await _context.SaveChangesAsync();
                _notyf.Success("Records saved successfully");
                return RedirectToAction(nameof(Index));
                
            }
            return View(weighBridgeNote);
        }

        // GET: WeighBridgeNotes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weighBridgeNote = await _context.WeighBridgeNote.FindAsync(id);
            if (weighBridgeNote == null)
            {
                return NotFound();
            }
            return View(weighBridgeNote);
        }

        // POST: WeighBridgeNotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Date,Miller,Factory,VehicleNo,CbkCode,OutreturnNo,FirstWeight,SecondWeight,NetWeight,Category,NoBags,Storage,RecipientName,RecipientIdno,Selection")] WeighBridgeNote weighBridgeNote)
        {
            if (id != weighBridgeNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weighBridgeNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeighBridgeNoteExists(weighBridgeNote.Id))
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
            return View(weighBridgeNote);
        }

        // GET: WeighBridgeNotes/Delete/5
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

            var weighBridgeNote = await _context.WeighBridgeNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weighBridgeNote == null)
            {
                return NotFound();
            }

            return View(weighBridgeNote);
        }

        // POST: WeighBridgeNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var weighBridgeNote = await _context.WeighBridgeNote.FindAsync(id);
            _context.WeighBridgeNote.Remove(weighBridgeNote);
            await _context.SaveChangesAsync();
            _notyf.Success("Records Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool WeighBridgeNoteExists(long id)
        {
            return _context.WeighBridgeNote.Any(e => e.Id == id);
        }
    }
}
