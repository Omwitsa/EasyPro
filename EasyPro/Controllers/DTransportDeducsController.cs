using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.ViewModels.TransportersVM;

namespace EasyPro.Controllers
{
    public class DTransportDeducsController : Controller
    {
        private readonly MORINGAContext _context;

        public DTransportDeducsController(MORINGAContext context)
        {
            _context = context;
        }
        public TransportersVM Transportersobj { get; private set; }
        // GET: DTransportDeducs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DTransportDeducs.ToListAsync());
        }

        // GET: DTransportDeducs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransportDeduc = await _context.DTransportDeducs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransportDeduc == null)
            {
                return NotFound();
            }

            return View(dTransportDeduc);
        }

        // GET: DTransportDeducs/Create
        public IActionResult Create()
        {
            GetInitialValues();
            Transportersobj = new TransportersVM()
            {
                DTransporter = _context.DTransporters,
                DTransportDeduc = new Models.DTransportDeduc()
            };
            //return Json(new { data = Farmersobj });
            return View(Transportersobj);
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
        // POST: DTransportDeducs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,TdateDeduc,Description,Amount,Period,Startdate,Enddate,Auditid,Remarks,Auditdatetime,Yyear,Rate,Ai")] DTransportDeduc dTransportDeduc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dTransportDeduc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dTransportDeduc);
        }

        // GET: DTransportDeducs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            GetInitialValues();
            var dTransportDeduc = await _context.DTransportDeducs.FindAsync(id);
            if (dTransportDeduc == null)
            {
                return NotFound();
            }
            return View(dTransportDeduc);
        }

        // POST: DTransportDeducs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,TdateDeduc,Description,Amount,Period,Startdate,Enddate,Auditid,Remarks,Auditdatetime,Yyear,Rate,Ai")] DTransportDeduc dTransportDeduc)
        {
            if (id != dTransportDeduc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dTransportDeduc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransportDeducExists(dTransportDeduc.Id))
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
            return View(dTransportDeduc);
        }

        // GET: DTransportDeducs/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dTransportDeduc = await _context.DTransportDeducs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransportDeduc == null)
            {
                return NotFound();
            }

            return View(dTransportDeduc);
        }

        // POST: DTransportDeducs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dTransportDeduc = await _context.DTransportDeducs.FindAsync(id);
            _context.DTransportDeducs.Remove(dTransportDeduc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DTransportDeducExists(long id)
        {
            return _context.DTransportDeducs.Any(e => e.Id == id);
        }
    }
}
