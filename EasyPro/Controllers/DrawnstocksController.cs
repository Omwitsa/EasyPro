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
    public class DrawnstocksController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DrawnstocksController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Drawnstocks
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            return View(await _context.Drawnstocks
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Date == DateTime.Today && i.Branch == saccobranch)
                .OrderByDescending(s => s.Id).ToListAsync());
        }
        private void GetInitialValues()
        {
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var banksname = _context.DBanks.Where(i => i.BankCode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            
            var agproducts = _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).Select(b => b.PName).ToList();
            ViewBag.agproductsall = new SelectList(agproducts, "");

            var brances = _context.DBranch.Where(i => i.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

        }
        // GET: Drawnstocks/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drawnstock = await _context.Drawnstocks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drawnstock == null)
            {
                return NotFound();
            }

            return View(drawnstock);
        }

        // GET: Drawnstocks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drawnstocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Description,Quantity,Totalamount,Productid,Productname,Username,Priceeach,Month,Year,Branch,Updated,Buying,Ai,Commission")] Drawnstock drawnstock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drawnstock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(drawnstock);
        }

        // GET: Drawnstocks/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drawnstock = await _context.Drawnstocks.FindAsync(id);
            if (drawnstock == null)
            {
                return NotFound();
            }
            return View(drawnstock);
        }

        // POST: Drawnstocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Date,Description,Quantity,Totalamount,Productid,Productname,Username,Priceeach,Month,Year,Branch,Updated,Buying,Ai,Commission")] Drawnstock drawnstock)
        {
            if (id != drawnstock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drawnstock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrawnstockExists(drawnstock.Id))
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
            return View(drawnstock);
        }

        // GET: Drawnstocks/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drawnstock = await _context.Drawnstocks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drawnstock == null)
            {
                return NotFound();
            }

            return View(drawnstock);
        }

        // POST: Drawnstocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var drawnstock = await _context.Drawnstocks.FindAsync(id);
            _context.Drawnstocks.Remove(drawnstock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrawnstockExists(long id)
        {
            return _context.Drawnstocks.Any(e => e.Id == id);
        }
    }
}
