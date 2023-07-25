using System;
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
    public class SubCountiesController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public SubCountiesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: SubCounties
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccouser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var County = _context.DCompanies.FirstOrDefault(p => p.Name.ToUpper().Equals(sacco.ToUpper())).Province;
            var subcounty = await _context.SubCounty.ToListAsync();
            if (saccouser!= "psigei")
                subcounty=await _context.SubCounty.Where(k => k.County.ToUpper().Equals(County.ToUpper())).ToListAsync();
            
                return View(subcounty);
        }

        // GET: SubCounties/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(subCounty.Name))
                {
                    _notyf.Error("Sorry, Kindly provide sub-county");
                    return View(subCounty);
                }
                if (string.IsNullOrEmpty(subCounty.County))
                {
                    _notyf.Error("Sorry, Kindly provide county");
                    return View(subCounty);
                }
                var subCountyExist = _context.SubCounty.Any(g => g.Name.ToUpper().Equals(subCounty.Name.ToUpper()) 
                && g.County.ToUpper().Equals(subCounty.County.ToUpper()));
                if (subCountyExist)
                {
                    _notyf.Error("Sorry, Sub-county already exist");
                    return View(subCounty);
                }
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
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
            if (id != subCounty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(subCounty.Name))
                    {
                        _notyf.Error("Sorry, Kindly provide sub-county");
                        return View(subCounty);
                    }
                    if (string.IsNullOrEmpty(subCounty.County))
                    {
                        _notyf.Error("Sorry, Kindly provide county");
                        return View(subCounty);
                    }
                    var subCountyExist = _context.SubCounty.Any(g => g.Name.ToUpper().Equals(subCounty.Name.ToUpper())
                    && g.County.ToUpper().Equals(subCounty.County.ToUpper()) && g.Id != subCounty.Id);
                    if (subCountyExist)
                    {
                        _notyf.Error("Sorry, Sub-county already exist");
                        return View(subCounty);
                    }
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
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
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
