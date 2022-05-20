using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class SubCountiesController : Controller
    {
        private readonly MORINGAContext _context;

        public SubCountiesController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: SubCounties
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubCounty.ToListAsync());
        }

        // GET: SubCounties/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCounty = await _context.SubCounty
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCounty == null)
            {
                return NotFound();
            }

            return View(subCounty);
        }

        // GET: SubCounties/Create
        public IActionResult Create()
        {
            SetInitialValues();
            return View();
        }

        // POST: SubCounties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,County,Contact,Closed,CreatedOn,CreatedBy")] SubCounty subCounty)
        {
            if (ModelState.IsValid)
            {
                subCounty.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                subCounty.CreatedOn = DateTime.Today;
                _context.Add(subCounty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subCounty);
        }

        // GET: SubCounties/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var subCounty = await _context.SubCounty.FindAsync(id);
            if (subCounty == null)
            {
                return NotFound();
            }
            return View(subCounty);
        }

        private void SetInitialValues()
        {
            var counties = _context.County.Where(c => !c.Closed).ToList();
            ViewBag.counties = new SelectList(counties, "Name", "Name");
        }

        // POST: SubCounties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,County,Contact,Closed,CreatedOn,CreatedBy")] SubCounty subCounty)
        {
            if (id != subCounty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    subCounty.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    subCounty.CreatedOn = DateTime.Today;
                    _context.Update(subCounty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCountyExists(subCounty.Id))
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
            return View(subCounty);
        }

        // GET: SubCounties/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCounty = await _context.SubCounty
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCounty == null)
            {
                return NotFound();
            }

            return View(subCounty);
        }

        // POST: SubCounties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var subCounty = await _context.SubCounty.FindAsync(id);
            _context.SubCounty.Remove(subCounty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubCountyExists(long id)
        {
            return _context.SubCounty.Any(e => e.Id == id);
        }
    }
}
