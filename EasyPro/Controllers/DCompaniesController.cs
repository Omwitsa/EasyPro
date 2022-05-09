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
    public class DCompaniesController : Controller
    {
        private readonly MORINGAContext _context;

        public DCompaniesController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DCompanies
        public async Task<IActionResult> Index()
        {
            return View(await _context.DCompanies.ToListAsync());
        }

        // GET: DCompanies/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dCompany = await _context.DCompanies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dCompany == null)
            {
                return NotFound();
            }

            return View(dCompany);
        }

        // GET: DCompanies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Adress,Town,Country,Province,District,Division,Location,FaxNo,PhoneNo,Email,Website,Fiscal,Auditid,Auditdatetime,Acc,Motto,SendTime,Smsno,Smscost,Smsport,Period")] DCompany dCompany)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dCompany);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dCompany);
        }

        // GET: DCompanies/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dCompany = await _context.DCompanies.FindAsync(id);
            if (dCompany == null)
            {
                return NotFound();
            }
            return View(dCompany);
        }

        // POST: DCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Adress,Town,Country,Province,District,Division,Location,FaxNo,PhoneNo,Email,Website,Fiscal,Auditid,Auditdatetime,Acc,Motto,SendTime,Smsno,Smscost,Smsport,Period")] DCompany dCompany)
        {
            if (id != dCompany.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DCompanyExists(dCompany.Id))
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
            return View(dCompany);
        }

        // GET: DCompanies/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dCompany = await _context.DCompanies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dCompany == null)
            {
                return NotFound();
            }

            return View(dCompany);
        }

        // POST: DCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dCompany = await _context.DCompanies.FindAsync(id);
            _context.DCompanies.Remove(dCompany);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DCompanyExists(long id)
        {
            return _context.DCompanies.Any(e => e.Id == id);
        }
    }
}
