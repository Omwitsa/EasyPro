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
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class ParchmentFactoryPricingsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public ParchmentFactoryPricingsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: ParchmentFactoryPricings
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var list = _context.ParchmentFactoryPricing.Where(n => n.saccocode == sacco).ToList();
            return View(list);
        }
        [HttpPost]
        public JsonResult getsuppliers()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var suppliers = _context.ParchmentFactoryPricing.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            
            suppliers = suppliers.OrderByDescending(i => i.AuditDateTime).Take(15).ToList();
            return Json(suppliers);
        }
        // GET: ParchmentFactoryPricings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parchmentFactoryPricing = await _context.ParchmentFactoryPricing
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parchmentFactoryPricing == null)
            {
                return NotFound();
            }

            return View(parchmentFactoryPricing);
        }

        // GET: ParchmentFactoryPricings/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            getinitials();
            return View();
        }
         private void getinitials()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var brances = _context.DBranch.Where(a => a.Bcode == sacco).OrderBy(K => K.Bname).ToList();
            var parchment = _context.Parchment.Where(a => a.saccocode == sacco).OrderBy(K => K.PName).ToList();
            var grading = _context.ParchmentGrading.Where(a => a.saccocode == sacco).OrderBy(K => K.PGrading).ToList();
            var pclass = _context.ParchmentClasses.Where(a => a.saccocode == sacco).OrderBy(K => K.PClasses).ToList();
            ViewBag.brances = new SelectList(brances.Select(b => b.Bname));

            ViewBag.parchment = new SelectList(parchment.Select(b => b.PName));
            ViewBag.grading = new SelectList(grading.Select(b => b.PGrading));
            ViewBag.pclass = new SelectList(pclass.Select(b => b.PClasses));
        }
        // POST: ParchmentFactoryPricings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Factory,Parchment,Grading,Class,Price,saccocode,Date,Year,AuditDateTime")] ParchmentFactoryPricing parchmentFactoryPricing)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            parchmentFactoryPricing.Year = parchmentFactoryPricing.Date.Year.ToString();// "2023";
            var dSupplierExists = _context.ParchmentFactoryPricing.Any(i => i.Factory == parchmentFactoryPricing.Factory
            && i.Grading == parchmentFactoryPricing.Grading && i.Parchment == parchmentFactoryPricing.Parchment
            && i.Class == parchmentFactoryPricing.Class && i.Year == parchmentFactoryPricing.Year.ToString());
            if (dSupplierExists)
            {
                getinitials();
                _notyf.Error("Sorry, The Product already exist");
                return View();
            }


            if (ModelState.IsValid)
            {
                parchmentFactoryPricing.saccocode = sacco;
                parchmentFactoryPricing.AuditDateTime = DateTime.Now;
                parchmentFactoryPricing.Year = parchmentFactoryPricing.Date.Year.ToString();
                _context.Add(parchmentFactoryPricing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parchmentFactoryPricing);
        }

        // GET: ParchmentFactoryPricings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            getinitials();
            if (id == null)
            {
                return NotFound();
            }

            var parchmentFactoryPricing = await _context.ParchmentFactoryPricing.FindAsync(id);
            if (parchmentFactoryPricing == null)
            {
                return NotFound();
            }
            return View(parchmentFactoryPricing);
        }

        // POST: ParchmentFactoryPricings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Factory,Parchment,Grading,Class,Price,saccocode,Date,Year,AuditDateTime")] ParchmentFactoryPricing parchmentFactoryPricing)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            getinitials();
            if (id != parchmentFactoryPricing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    parchmentFactoryPricing.saccocode = sacco;
                    parchmentFactoryPricing.AuditDateTime = DateTime.Now;
                    parchmentFactoryPricing.Year = parchmentFactoryPricing.Date.Year.ToString();

                    _context.Update(parchmentFactoryPricing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParchmentFactoryPricingExists(parchmentFactoryPricing.Id))
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
            return View(parchmentFactoryPricing);
        }

        // GET: ParchmentFactoryPricings/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parchmentFactoryPricing = await _context.ParchmentFactoryPricing
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parchmentFactoryPricing == null)
            {
                return NotFound();
            }

            return View(parchmentFactoryPricing);
        }

        // POST: ParchmentFactoryPricings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var parchmentFactoryPricing = await _context.ParchmentFactoryPricing.FindAsync(id);
            _context.ParchmentFactoryPricing.Remove(parchmentFactoryPricing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParchmentFactoryPricingExists(long id)
        {
            return _context.ParchmentFactoryPricing.Any(e => e.Id == id);
        }
    }
}
