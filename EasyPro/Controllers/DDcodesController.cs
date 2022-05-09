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
    public class DDcodesController : Controller
    {
        private readonly MORINGAContext _context;

        public DDcodesController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DDcodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DDcodes.ToListAsync());
        }

        // GET: DDcodes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dDcode = await _context.DDcodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dDcode == null)
            {
                return NotFound();
            }

            return View(dDcode);
        }

        // GET: DDcodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DDcodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dcode,Description,Dedaccno,Contraacc,Auditid,Auditdatetime")] DDcode dDcode)
        {
            if (ModelState.IsValid)
            {
               
                _context.Add(dDcode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dDcode);
        }

        // GET: DDcodes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var dDcode = await _context.DDcodes.FindAsync(id);
            if (dDcode == null)
            {
                return NotFound();
            }
            return View(dDcode);
        }

        // POST: DDcodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Dcode,Description,Dedaccno,Contraacc,Auditid,Auditdatetime")] DDcode dDcode)
        {
            if (id != dDcode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dDcode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDcodeExists(dDcode.Id))
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
            return View(dDcode);
        }

        // GET: DDcodes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dDcode = await _context.DDcodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dDcode == null)
            {
                return NotFound();
            }

            return View(dDcode);
        }

        // POST: DDcodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dDcode = await _context.DDcodes.FindAsync(id);
            _context.DDcodes.Remove(dDcode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DDcodeExists(long id)
        {
            return _context.DDcodes.Any(e => e.Id == id);
        }
    }
}
