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
    public class DPriceController : Controller
    {
        private readonly MORINGAContext _context;

        public DPriceController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DPrice
        public async Task<IActionResult> Index()
        {
            return View(await _context.DPrices.ToListAsync());
        }

        // GET: DPrice/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Products == id);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // GET: DPrice/Create
        public IActionResult Create()
        {
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var products = _context.DBranchProducts.Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
        }
            // POST: DPrice/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Products,Edate,Price")] DPrice dPrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dPrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dPrice);
        }

        // GET: DPrice/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices.FindAsync(id);
            if (dPrice == null)
            {
                return NotFound();
            }
            return View(dPrice);
        }

        // POST: DPrice/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Products,Edate,Price")] DPrice dPrice)
        {
            if (id != dPrice.Products)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dPrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPriceExists(dPrice.Products))
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
            return View(dPrice);
        }

        // GET: DPrice/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Products == id);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // POST: DPrice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var dPrice = await _context.DPrices.FindAsync(id);
            _context.DPrices.Remove(dPrice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPriceExists(string id)
        {
            return _context.DPrices.Any(e => e.Products == id);
        }
    }
}
