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
    public class DRegistrationsController : Controller
    {
        private readonly MORINGAContext _context;

        public DRegistrationsController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DRegistrations
        public async Task<IActionResult> Index()
        {
            return View(await _context.DRegistrations.ToListAsync());
        }

        // GET: DRegistrations/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dRegistration = await _context.DRegistrations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dRegistration == null)
            {
                return NotFound();
            }

            return View(dRegistration);
        }

        // GET: DRegistrations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DRegistrations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,Transdate,Amount,Bal,Transdescription,Auditid,Auditdate,Mno,Toledgers,Datepostedtoledger,Userledger,LocalId,Run")] DRegistration dRegistration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dRegistration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dRegistration);
        }

        // GET: DRegistrations/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dRegistration = await _context.DRegistrations.FindAsync(id);
            if (dRegistration == null)
            {
                return NotFound();
            }
            return View(dRegistration);
        }

        // POST: DRegistrations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,Transdate,Amount,Bal,Transdescription,Auditid,Auditdate,Mno,Toledgers,Datepostedtoledger,Userledger,LocalId,Run")] DRegistration dRegistration)
        {
            if (id != dRegistration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dRegistration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DRegistrationExists(dRegistration.Id))
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
            return View(dRegistration);
        }

        // GET: DRegistrations/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dRegistration = await _context.DRegistrations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dRegistration == null)
            {
                return NotFound();
            }

            return View(dRegistration);
        }

        // POST: DRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dRegistration = await _context.DRegistrations.FindAsync(id);
            _context.DRegistrations.Remove(dRegistration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DRegistrationExists(long id)
        {
            return _context.DRegistrations.Any(e => e.Id == id);
        }
    }
}
