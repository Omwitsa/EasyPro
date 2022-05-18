using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class DBankBranchesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;

        public DBankBranchesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: DBankBranches
        public async Task<IActionResult> Index()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            return View(await _context.DBankBranch
                .Where(i => i.BankCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DBankBranches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBankBranch = await _context.DBankBranch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBankBranch == null)
            {
                return NotFound();
            }

            return View(dBankBranch);
        }

        // GET: DBankBranches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DBankBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BankCode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBankBranch dBankBranch)
        {
            var dSupplier1 = _context.DBankBranch.Where(i => i.Bname == dBankBranch.Bname && i.BankCode == dBankBranch.BankCode).Count();
            if (dSupplier1 != 0)
            {
                _notyf.Error("Sorry, The Bank Branch Name already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                sacco = sacco ?? "";
                dBankBranch.BankCode = sacco;
                _context.Add(dBankBranch);
                await _context.SaveChangesAsync();
                _notyf.Success("Bank Branch saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dBankBranch);
        }

        // GET: DBankBranches/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBankBranch = await _context.DBankBranch.FindAsync(id);
            if (dBankBranch == null)
            {
                return NotFound();
            }
            return View(dBankBranch);
        }

        // POST: DBankBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,BankCode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBankBranch dBankBranch)
        {
            if (id != dBankBranch.Id)
            {
                _notyf.Error("Sorry, an error occured while eidting");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                    sacco = sacco ?? "";
                    dBankBranch.BankCode = sacco;
                    _context.Update(dBankBranch);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Bank Branch Edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBankBranchExists(dBankBranch.Id))
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
            return View(dBankBranch);
        }

        // GET: DBankBranches/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dBankBranch = await _context.DBankBranch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBankBranch == null)
            {
                return NotFound();
            }

            return View(dBankBranch);
        }

        // POST: DBankBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dBankBranch = await _context.DBankBranch.FindAsync(id);
            _context.DBankBranch.Remove(dBankBranch);
            await _context.SaveChangesAsync();
            _notyf.Success("Bank Branch Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DBankBranchExists(long id)
        {
            return _context.DBankBranch.Any(e => e.Id == id);
        }
    }
}
