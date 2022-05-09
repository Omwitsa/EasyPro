using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;

namespace EasyPro.Controllers
{
    public class DTransportersController : Controller
    {
        private readonly MORINGAContext _context;

        public DTransportersController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DTransporters
        public async Task<IActionResult> Index()
        {
            return View(await _context.DTransporters.ToListAsync());
        }

        // GET: DTransporters/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransporter = await _context.DTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransporter == null)
            {
                return NotFound();
            }

            return View(dTransporter);
        }

        // GET: DTransporters/Create
        public IActionResult Create()
        {
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var banksname = _context.DBanks.Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            var brances = _context.DBranch.Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

            List<SelectListItem> gender = new()
            {
                new SelectListItem { Value = "1", Text = "Male" },
                new SelectListItem { Value = "2", Text = "Female" },
            };
            ViewBag.gender = gender;
            List<SelectListItem> payment = new()
            {
                new SelectListItem { Value = "1", Text = "Weekly" },
                new SelectListItem { Value = "2", Text = "Monthly" },
            };
            ViewBag.payment = payment;
        }
        // POST: DTransporters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,TransName,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Isfrate,Rate,Canno,Tt,ParentT,Ttrate,Br,Freezed,PaymenMode")] DTransporter dTransporter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dTransporter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dTransporter);
        }

        // GET: DTransporters/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            GetInitialValues();
            var dTransporter = await _context.DTransporters.FindAsync(id);
            if (dTransporter == null)
            {
                return NotFound();
            }
            return View(dTransporter);
        }

        // POST: DTransporters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,TransName,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Isfrate,Rate,Canno,Tt,ParentT,Ttrate,Br,Freezed,PaymenMode")] DTransporter dTransporter)
        {
            if (id != dTransporter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dTransporter.Br = "A";
                    dTransporter.Freezed = "0";
                    _context.Update(dTransporter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransporterExists(dTransporter.Id))
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
            return View(dTransporter);
        }

        // GET: DTransporters/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransporter = await _context.DTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransporter == null)
            {
                return NotFound();
            }

            return View(dTransporter);
        }

        // POST: DTransporters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dTransporter = await _context.DTransporters.FindAsync(id);
            _context.DTransporters.Remove(dTransporter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTransporterExists(long id)
        {
            return _context.DTransporters.Any(e => e.Id == id);
        }
    }
}
