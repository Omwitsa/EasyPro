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
    public class DMilkintakesController : Controller
    {
        private readonly MORINGAContext _context;

        public DMilkintakesController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DMilkintakes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DMilkintakes.ToListAsync());
        }

        // GET: DMilkintakes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var dMilkintake = await _context.DMilkintakes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dMilkintake == null)
            {
                return NotFound();
            }

            return View(dMilkintake);
        }

        // GET: DMilkintakes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DMilkintakes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,TransDate,Qsupplied,Ppu,TransTime,CR,DR,BAL,AuditId,Auditdatetime,Paid,Lr,Remark,Descript,Comment,Status1,Location,LocalId,Run,Type,Productprocess")] DMilkintake dMilkintake)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dMilkintake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dMilkintake);
        }

        // GET: DMilkintakes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dMilkintake = await _context.DMilkintakes.FindAsync(id);
            if (dMilkintake == null)
            {
                return NotFound();
            }
            return View(dMilkintake);
        }

        // POST: DMilkintakes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,TransDate,Qsupplied,Ppu,TransTime,CR,DR,BAL,AuditId,Auditdatetime,Paid,Lr,Remark,Descript,Comment,Status1,Location,LocalId,Run,Type,Productprocess")] DMilkintake dMilkintake)
        {
            if (id != dMilkintake.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dMilkintake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DMilkintakeExists(dMilkintake.Id))
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
            return View(dMilkintake);
        }

        // GET: DMilkintakes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dMilkintake = await _context.DMilkintakes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dMilkintake == null)
            {
                return NotFound();
            }

            return View(dMilkintake);
        }

        // POST: DMilkintakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dMilkintake = await _context.DMilkintakes.FindAsync(id);
            _context.DMilkintakes.Remove(dMilkintake);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DMilkintakeExists(long id)
        {
            return _context.DMilkintakes.Any(e => e.Id == id);
        }
    }
}
