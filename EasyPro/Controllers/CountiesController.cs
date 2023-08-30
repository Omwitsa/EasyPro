using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class CountiesController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public CountiesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Counties
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var saccouser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var County = await _context.County.ToListAsync();
            var selectedCounty = _context.DCompanies.FirstOrDefault(p => p.Name.ToUpper().Equals(sacco.ToUpper())).Province;
            if (saccouser != "psigei")
                County = await _context.County.Where(k => k.Name.ToUpper().Equals(selectedCounty.ToUpper())).ToListAsync();

            return View(County);
        }

        // GET: Counties/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var county = await _context.County
                .FirstOrDefaultAsync(m => m.Id == id);
            if (county == null)
            {
                return NotFound();
            }

            return View(county);
        }

        // GET: Counties/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: Counties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Contact,Closed,CreatedOn,CreatedBy")] County county)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(county.Name))
                {
                    _notyf.Error("Sorry, Kindly provide county");
                    return View(county);
                }
                if (_context.County.Any(g => g.Name.ToUpper().Equals(county.Name.ToUpper())))
                {
                    _notyf.Error("Sorry, County already exist");
                    return View(county);
                }
                county.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                county.CreatedOn = DateTime.Today;
                _context.Add(county);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(county);
        }

        // GET: Counties/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var county = await _context.County.FindAsync(id);
            if (county == null)
            {
                return NotFound();
            }
            return View(county);
        }

        // POST: Counties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Contact,Closed,CreatedOn,CreatedBy")] County county)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != county.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(county.Name))
                    {
                        _notyf.Error("Sorry, Kindly provide county");
                        return View(county);
                    }
                    if (_context.County.Any(g => g.Name.ToUpper().Equals(county.Name.ToUpper()) && g.Id != county.Id))
                    {
                        _notyf.Error("Sorry, County already exist");
                        return View(county);
                    }
                    county.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    county.CreatedOn = DateTime.Today;
                    _context.Update(county);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountyExists(county.Id))
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
            return View(county);
        }

        // GET: Counties/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var county = await _context.County
                .FirstOrDefaultAsync(m => m.Id == id);
            if (county == null)
            {
                return NotFound();
            }

            return View(county);
        }

        // POST: Counties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var county = await _context.County.FindAsync(id);
            _context.County.Remove(county);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountyExists(long id)
        {
            return _context.County.Any(e => e.Id == id);
        }
    }
}
