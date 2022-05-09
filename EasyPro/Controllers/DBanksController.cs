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
    public class DBanksController : Controller
    {
        private readonly MORINGAContext _context;

        public DBanksController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DBanks
        public async Task<IActionResult> Index()
        {
            return View(await _context.DBanks.ToListAsync());
        }

        // GET: DBanks/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBank = await _context.DBanks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBank == null)
            {
                return NotFound();
            }

            return View(dBank);
        }

        // GET: DBanks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DBanks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BankCode,BankName,BranchName,Address,Telephone,AuditId,AuditTime,BankAccNo,Accno,AccType")] DBank dBank)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dBank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dBank);
        }

        // GET: DBanks/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBank = await _context.DBanks.FindAsync(id);
            if (dBank == null)
            {
                return NotFound();
            }
            return View(dBank);
        }

        // POST: DBanks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,BankCode,BankName,BranchName,Address,Telephone,AuditId,AuditTime,BankAccNo,Accno,AccType")] DBank dBank)
        {
            if (id != dBank.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dBank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBankExists(dBank.Id))
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
            return View(dBank);
        }

        // GET: DBanks/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBank = await _context.DBanks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBank == null)
            {
                return NotFound();
            }

            return View(dBank);
        }

        // POST: DBanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dBank = await _context.DBanks.FindAsync(id);
            _context.DBanks.Remove(dBank);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DBankExists(long id)
        {
            return _context.DBanks.Any(e => e.Id == id);
        }
    }
}
