using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class AgSupplier1Controller : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public AgSupplier1Controller(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: AgSupplier1
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.AgSupplier1s
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: AgSupplier1/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agSupplier1 = await _context.AgSupplier1s
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (agSupplier1 == null)
            {
                return NotFound();
            }

            return View(agSupplier1);
        }

        // GET: AgSupplier1/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View();
        }

        // POST: AgSupplier1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,SupplierId,CompanyName,ContactPerson,ContactTitle,Address,Email,Phone,Fax,saccocode")] AgSupplier1 agSupplier1)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (_context.AgSupplier1s.Any(i => i.SupplierId == agSupplier1.SupplierId && i.saccocode.ToUpper().Equals(sacco.ToUpper())))
            {
                _notyf.Error("Sorry, Supplier Number code does not exist");
                //return Json(new { data = Farmersobj });
                return View(agSupplier1);
            }
            
            if (ModelState.IsValid)
            {
                agSupplier1.saccocode = sacco;
                _context.Add(agSupplier1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agSupplier1);
        }

        // GET: AgSupplier1/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id == null)
            {
                return NotFound();
            }

            var agSupplier1 = await _context.AgSupplier1s.FindAsync(id);
            if (agSupplier1 == null)
            {
                return NotFound();
            }
            return View(agSupplier1);
        }

        // POST: AgSupplier1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("id,SupplierId,CompanyName,ContactPerson,ContactTitle,Address,Email,Phone,Fax,saccocode")] AgSupplier1 agSupplier1)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != agSupplier1.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    agSupplier1.saccocode = sacco;
                    _context.Update(agSupplier1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgSupplier1Exists(agSupplier1.id))
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
            return View(agSupplier1);
        }

        // GET: AgSupplier1/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id == null)
            {
                return NotFound();
            }

            var agSupplier1 = await _context.AgSupplier1s
                .FirstOrDefaultAsync(m => m.id == id);
            if (agSupplier1 == null)
            {
                return NotFound();
            }

            return View(agSupplier1);
        }

        // POST: AgSupplier1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var agSupplier1 = await _context.AgSupplier1s.FindAsync(id);
            _context.AgSupplier1s.Remove(agSupplier1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgSupplier1Exists(long id)
        {
            return _context.AgSupplier1s.Any(e => e.id == id);
        }
    }
}
