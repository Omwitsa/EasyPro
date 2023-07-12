using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Models.Coffee;

namespace EasyPro.Controllers.Coffee
{
    public class MarketersController : Controller
    {
        private readonly MORINGAContext _context;

        public MarketersController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: Marketers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Marketer.ToListAsync());
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
            return View();
        }

        // POST: Marketers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,saccocode,MarketerName,Category,Grade,Class,Kgs,Date,Amount,AuditDateTime")] Marketer marketer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marketer);
                await _context.SaveChangesAsync();
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
            var marketer = await _context.Marketer.FindAsync(id);
            _context.Marketer.Remove(marketer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarketerExists(long id)
        {
            return _context.Marketer.Any(e => e.Id == id);
        }
    }
}
