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
    public class CProductsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public CProductsController(MORINGAContext context, INotyfService notyf)
        {
            utilities = new Utilities(context);
            _context = context;
            _notyf = notyf;
        }

        // GET: CProducts
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.CProducts.Where(p => p.SaccoCode == sacco).ToListAsync());
        }

        // GET: CProducts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var cProduct = await _context.CProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cProduct == null)
            {
                return NotFound();
            }

            return View(cProduct);
        }

        // GET: CProducts/Create
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
            var taxes = _context.Taxes.Where(c => c.SaccoCode == sacco && c.Type == "Sales").ToList();
            ViewBag.taxes = new SelectList(taxes, "Name", "Name");
        }

        // POST: CProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Category,Ref,BarCode,Price,CustomerTax,Cost,Notes,ARGlAccount,APGlAccount,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] CProduct cProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (ModelState.IsValid)
            {
                cProduct.Id = Guid.NewGuid();
                cProduct.SaccoCode = sacco;
                _context.Add(cProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cProduct);
        }

        // GET: CProducts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            SetInitialValues();
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var cProduct = await _context.CProducts.FindAsync(id);
            if (cProduct == null)
            {
                return NotFound();
            }
            return View(cProduct);
        }

        // POST: CProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Type,Category,Ref,BarCode,Price,CustomerTax,Cost,Notes,ARGlAccount,APGlAccount,Closed,Personnel,CreatedDate,ModifiedDate,SaccoCode")] CProduct cProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != cProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CProductExists(cProduct.Id))
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
            return View(cProduct);
        }

        // GET: CProducts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var cProduct = await _context.CProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cProduct == null)
            {
                return NotFound();
            }

            return View(cProduct);
        }

        // POST: CProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            utilities.SetUpPrivileges(this);
            var cProduct = await _context.CProducts.FindAsync(id);
            _context.CProducts.Remove(cProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CProductExists(Guid id)
        {
            return _context.CProducts.Any(e => e.Id == id);
        }
    }
}
