using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class DSupplierDeducsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;

        public DSupplierDeducsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;

        }

        // GET: DSupplierDeducs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DSupplierDeducs.ToListAsync());
        }

        // GET: DSupplierDeducs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dSupplierDeduc = await _context.DSupplierDeducs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplierDeduc == null)
            {
                return NotFound();
            }

            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Create
        public IActionResult Create()
        {
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var Descriptionname = _context.DDcodes.Select(b => b.Description).ToList();
            ViewBag.Description = new SelectList(Descriptionname);

            var brances = _context.DBranch.Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

            List<SelectListItem> gender = new()
            {
                new SelectListItem { Value = "1", Text = "Male" },
                new SelectListItem { Value = "2", Text = "Female" },
            };
            ViewBag.gender = gender;
            List<SelectListItem> payment = new()
            {
                new SelectListItem { Value = "1", Text = "Weekly" },
                new SelectListItem { Value = "2", Text = "Monthly" },
            };
            ViewBag.payment = payment;
        }
        // POST: DSupplierDeducs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,DateDeduc,Description,Amount,Period,StartDate,EndDate,Auditid,Auditdatetime,Yyear,Remarks,Branch,Bonus,Status1,Status2,Status3,Status4,Status5,Status6,Branchcode")] DSupplierDeduc dSupplierDeduc)
        {
            var dSupplier1 = _context.DSupplierDeducs.Where(i => i.Sno == dSupplierDeduc.Sno).Count();
            if (dSupplier1 != 0)
            {
                GetInitialValues();
                _notyf.Error("Transporter code does not exist");
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Add(dSupplierDeduc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            GetInitialValues();
            var dSupplierDeduc = await _context.DSupplierDeducs.FindAsync(id);
            if (dSupplierDeduc == null)
            {
                return NotFound();
            }
            return View(dSupplierDeduc);
        }

        // POST: DSupplierDeducs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,DateDeduc,Description,Amount,Period,StartDate,EndDate,Auditid,Auditdatetime,Yyear,Remarks,Branch,Bonus,Status1,Status2,Status3,Status4,Status5,Status6,Branchcode")] DSupplierDeduc dSupplierDeduc)
        {
            if (id != dSupplierDeduc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dSupplierDeduc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DSupplierDeducExists(dSupplierDeduc.Id))
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
            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dSupplierDeduc = await _context.DSupplierDeducs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplierDeduc == null)
            {
                return NotFound();
            }

            return View(dSupplierDeduc);
        }

        // POST: DSupplierDeducs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dSupplierDeduc = await _context.DSupplierDeducs.FindAsync(id);
            _context.DSupplierDeducs.Remove(dSupplierDeduc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DSupplierDeducExists(long id)
        {
            return _context.DSupplierDeducs.Any(e => e.Id == id);
        }
    }
}
