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
    public class Milling_ProductsController : Controller
    {
        private readonly MORINGAContext _context;

        public Milling_ProductsController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: Milling_Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Milling_Products.ToListAsync());
        }

        // GET: Milling_Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling_Products = await _context.Milling_Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milling_Products == null)
            {
                return NotFound();
            }

            return View(milling_Products);
        }

        // GET: Milling_Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Milling_Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Factory,saccocode,Miller,Category,Grade,Class,Kgs,Date,AuditDateTime")] Milling_Products milling_Products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(milling_Products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(milling_Products);
        }

        // GET: Milling_Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling_Products = await _context.Milling_Products.FindAsync(id);
            if (milling_Products == null)
            {
                return NotFound();
            }
            return View(milling_Products);
        }

        // POST: Milling_Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Factory,saccocode,Miller,Category,Grade,Class,Kgs,Date,AuditDateTime")] Milling_Products milling_Products)
        {
            if (id != milling_Products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(milling_Products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Milling_ProductsExists(milling_Products.Id))
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
            return View(milling_Products);
        }

        // GET: Milling_Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var milling_Products = await _context.Milling_Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milling_Products == null)
            {
                return NotFound();
            }

            return View(milling_Products);
        }

        // POST: Milling_Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var milling_Products = await _context.Milling_Products.FindAsync(id);
            _context.Milling_Products.Remove(milling_Products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Milling_ProductsExists(long id)
        {
            return _context.Milling_Products.Any(e => e.Id == id);
        }
    }
}
