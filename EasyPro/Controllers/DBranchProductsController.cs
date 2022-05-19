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
    public class DBranchProductsController : Controller
    {
        private readonly MORINGAContext _context;

        public DBranchProductsController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DBranchProducts
        public async Task<IActionResult> Index()
        {
            return View(await _context.DBranchProducts.ToListAsync());
        }

        // GET: DBranchProducts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBranchProduct = await _context.DBranchProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBranchProduct == null)
            {
                return NotFound();
            }

            return View(dBranchProduct);
        }

        // GET: DBranchProducts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DBranchProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Bcode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBranchProduct dBranchProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dBranchProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }           
            return View(dBranchProduct);
        }

        // GET: DBranchProducts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBranchProduct = await _context.DBranchProducts.FindAsync(id);
            if (dBranchProduct == null)
            {
                return NotFound();
            }
            return View(dBranchProduct);
        }

        // POST: DBranchProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Bcode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBranchProduct dBranchProduct)
        {
            if (id != dBranchProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dBranchProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBranchProductExists(dBranchProduct.Id))
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
            return View(dBranchProduct);
        }

        // GET: DBranchProducts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBranchProduct = await _context.DBranchProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBranchProduct == null)
            {
                return NotFound();
            }

            return View(dBranchProduct);
        }

        // POST: DBranchProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dBranchProduct = await _context.DBranchProducts.FindAsync(id);
            _context.DBranchProducts.Remove(dBranchProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DBranchProductExists(long id)
        {
            return _context.DBranchProducts.Any(e => e.Id == id);
        }
    }
}
