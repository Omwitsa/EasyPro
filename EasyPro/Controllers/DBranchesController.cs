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
    public class DBranchesController : Controller
    {
        private readonly MORINGAContext _context;

        public DBranchesController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DBranches
        public async Task<IActionResult> Index()
        {
            return View(await _context.DBranch.ToListAsync());
        }

        // GET: DBranches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBranch = await _context.DBranch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBranch == null)
            {
                return NotFound();
            }

            return View(dBranch);
        }

        // GET: DBranches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Bcode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBranch dBranch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dBranch);
        }

        // GET: DBranches/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBranch = await _context.DBranch.FindAsync(id);
            if (dBranch == null)
            {
                return NotFound();
            }
            return View(dBranch);
        }

        // POST: DBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Bcode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBranch dBranch)
        {
            if (id != dBranch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBranchExists(dBranch.Id))
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
            return View(dBranch);
        }

        // GET: DBranches/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBranch = await _context.DBranch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBranch == null)
            {
                return NotFound();
            }

            return View(dBranch);
        }

        // POST: DBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dBranch = await _context.DBranch.FindAsync(id);
            _context.DBranch.Remove(dBranch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DBranchExists(long id)
        {
            return _context.DBranch.Any(e => e.Id == id);
        }
    }
}
