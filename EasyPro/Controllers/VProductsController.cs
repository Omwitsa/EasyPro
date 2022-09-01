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
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class VProductsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public VProductsController(MORINGAContext context, INotyfService notyf)
        {
            utilities = new Utilities(context);
            _context = context;
            _notyf = notyf;
        }

        // GET: VProducts
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.VProducts.Where(p => p.SaccoCode == sacco).ToListAsync());
        }

        // GET: VProducts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var vProduct = await _context.VProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vProduct == null)
            {
                return NotFound();
            }

            return View(vProduct);
        }

        // GET: VProducts/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var glsetups = _context.Glsetups.Where(c => c.saccocode == sacco).ToList();
            ViewBag.glsetups = new SelectList(glsetups, "AccNo", "GlAccName");
            var types = new string[] { "Consumable", "Service" };
            ViewBag.types = new SelectList(types);
            var taxes = _context.Taxes.Where(c => c.SaccoCode == sacco && c.Type == "Purchases").ToList();
            ViewBag.taxes = new SelectList(taxes, "Name", "Name");
        }

        // POST: VProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Category,Ref,BarCode,Price,Cost,Notes,VenderTax,ARGlAccount,APGlAccount,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] VProduct vProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (ModelState.IsValid)
            {
                vProduct.Id = Guid.NewGuid();
                vProduct.SaccoCode = sacco;
                _context.Add(vProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vProduct);
        }

        // GET: VProducts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var vProduct = await _context.VProducts.FindAsync(id);
            if (vProduct == null)
            {
                return NotFound();
            }
            return View(vProduct);
        }

        // POST: VProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Type,Category,Ref,BarCode,Price,Cost,Notes,VenderTax,ARGlAccount,APGlAccount,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] VProduct vProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != vProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VProductExists(vProduct.Id))
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
            return View(vProduct);
        }

        // GET: VProducts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var vProduct = await _context.VProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vProduct == null)
            {
                return NotFound();
            }

            return View(vProduct);
        }

        // POST: VProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            utilities.SetUpPrivileges(this);
            var vProduct = await _context.VProducts.FindAsync(id);
            _context.VProducts.Remove(vProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VProductExists(Guid id)
        {
            return _context.VProducts.Any(e => e.Id == id);
        }
    }
}
