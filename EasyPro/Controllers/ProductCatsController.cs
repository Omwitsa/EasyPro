using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class ProductCatsController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public ProductCatsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: ProductCats
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.d_ProductCat.Where(c => c.SaccoCode == sacco).ToListAsync());
        }

        // GET: ProductCats/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productCat = await _context.d_ProductCat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCat == null)
            {
                return NotFound();
            }

            return View(productCat);
        }

        // GET: ProductCats/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: ProductCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ProductCat productCat)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var extist = _context.d_ProductCat.Any(c => c.Name.ToUpper().Equals(productCat.Name.ToUpper())
            && c.SaccoCode == sacco);
            if (extist)
            {
                _notyf.Error("Sorry, Category already exist");
                return View(productCat);
            }

            if (ModelState.IsValid)
            {
                productCat.SaccoCode = sacco;
                _context.Add(productCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productCat);
        }

        // GET: ProductCats/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productCat = await _context.d_ProductCat.FindAsync(id);
            if (productCat == null)
            {
                return NotFound();
            }
            return View(productCat);
        }

        // POST: ProductCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] ProductCat productCat)
        {
            utilities.SetUpPrivileges(this);
            if (id != productCat.Id)
            {
                return NotFound();
            }

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var extist = _context.d_ProductCat.Any(c => c.Name.ToUpper().Equals(productCat.Name.ToUpper())
            && c.SaccoCode == sacco && c.Id != productCat.Id);
            if (extist)
            {
                _notyf.Error("Sorry, Category already exist");
                return View(productCat);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCatExists(productCat.Id))
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
            return View(productCat);
        }

        // GET: ProductCats/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productCat = await _context.d_ProductCat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productCat == null)
            {
                return NotFound();
            }

            return View(productCat);
        }

        // POST: ProductCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var productCat = await _context.d_ProductCat.FindAsync(id);
            _context.d_ProductCat.Remove(productCat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCatExists(long id)
        {
            return _context.d_ProductCat.Any(e => e.Id == id);
        }
    }
}
