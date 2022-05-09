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
    public class DTransportsController : Controller
    {
        private readonly MORINGAContext _context;

        public DTransportsController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DTransports
        public async Task<IActionResult> Index()
        {
            return View(await _context.DTransports.ToListAsync());
        }

        // GET: DTransports/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransport == null)
            {
                return NotFound();
            }

            return View(dTransport);
        }

        // GET: DTransports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DTransports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,Sno,Rate,Startdate,Active,DateInactivate,Auditid,Auditdatetime,Isfrate")] DTransport dTransport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dTransport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dTransport);
        }

        // GET: DTransports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports.FindAsync(id);
            if (dTransport == null)
            {
                return NotFound();
            }
            return View(dTransport);
        }

        // POST: DTransports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,Sno,Rate,Startdate,Active,DateInactivate,Auditid,Auditdatetime,Isfrate")] DTransport dTransport)
        {
            if (id != dTransport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dTransport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransportExists(dTransport.Id))
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
            return View(dTransport);
        }

        // GET: DTransports/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransport == null)
            {
                return NotFound();
            }

            return View(dTransport);
        }

        // POST: DTransports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dTransport = await _context.DTransports.FindAsync(id);
            _context.DTransports.Remove(dTransport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTransportExists(long id)
        {
            return _context.DTransports.Any(e => e.Id == id);
        }
    }
}
