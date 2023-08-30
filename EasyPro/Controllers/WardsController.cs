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
using EasyPro.ViewModels;

namespace EasyPro.Controllers
{
    public class WardsController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public WardsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Wards
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var myIp = utilities.GetLocalIPAddress();
            var ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();
            utilities.SetUpPrivileges(this);
            var saccouser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var county = _context.DCompanies.FirstOrDefault(a=>a.Name.ToUpper().Equals(sacco.ToUpper())).Province;
            var subcounty = _context.SubCounty.Where(k => k.County.ToUpper().Equals(county.ToUpper())).ToList();
            var Wards = await _context.Ward.ToListAsync();
            var WardList = new List<WardVM>();
            if (saccouser != "psigei")
            {
                var ward = subcounty.GroupBy(m=>m.Name).ToList();
                ward.ForEach( l => {
                var constituency = l.FirstOrDefault();
                Wards = _context.Ward.Where(k => k.SubCounty.ToUpper().Equals(constituency.Name.ToUpper())).ToList();
                Wards.ForEach(m=>{ 
                    WardList.Add(new WardVM { 
                      Name= m.Name,
                      CreatedBy=m.CreatedBy,
                      Closed=m.Closed,
                      Contact =m.Contact,
                      CreatedOn = m.CreatedOn,
                      SubCounty = m.SubCounty
                    });
                    _context.SaveChanges();
                });
            });
                
            }
                

            return View(WardList);
        }

        // GET: Wards/Details/5
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

            var ward = await _context.Ward
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // GET: Wards/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        // POST: Wards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SubCounty,Contact,Closed,CreatedOn,CreatedBy")] Ward ward)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ward.Name))
                {
                    _notyf.Error("Sorry, Kindly provide ward");
                    return View(ward);
                }
                if (string.IsNullOrEmpty(ward.SubCounty))
                {
                    _notyf.Error("Sorry, Kindly provide sub-county");
                    return View(ward);
                }
                var subCountyExist = _context.Ward.Any(g => g.Name.ToUpper().Equals(ward.Name.ToUpper())
                && g.SubCounty.ToUpper().Equals(ward.SubCounty.ToUpper()));
                if (subCountyExist)
                {
                    _notyf.Error("Sorry, Ward already exist");
                    return View(ward);
                }
                ward.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                ward.CreatedOn = DateTime.Today;
                _context.Add(ward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ward);
        }

        // GET: Wards/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Ward.FindAsync(id);
            if (ward == null)
            {
                return NotFound();
            }
            return View(ward);
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccouser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var subCounties = _context.SubCounty.Where(c => !c.Closed).ToList();
            var checkcounty = _context.DCompanies.FirstOrDefault(l => l.Name.ToUpper().Equals(sacco.ToUpper()));
            if (saccouser != "psigei")
                subCounties = _context.SubCounty.Where(k => k.County.ToUpper().Equals(checkcounty.Province.ToUpper())).OrderBy(m => m.Name).ToList();
            
            ViewBag.subCounties = new SelectList(subCounties, "Name", "Name");
        }

        // POST: Wards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,SubCounty,Contact,Closed,CreatedOn,CreatedBy")] Ward ward)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (string.IsNullOrEmpty(ward.Name))
            {
                _notyf.Error("Sorry, Kindly provide ward");
                return View(ward);
            }
            if (string.IsNullOrEmpty(ward.SubCounty))
            {
                _notyf.Error("Sorry, Kindly provide sub-county");
                return View(ward);
            }
            var wardExist = _context.Ward.Any(g => g.Name.ToUpper().Equals(ward.Name.ToUpper())
            && g.SubCounty.ToUpper().Equals(ward.SubCounty.ToUpper()) && g.Id != ward.Id);
            if (wardExist)
            {
                _notyf.Error("Sorry, Ward already exist");
                return View(ward);
            }
            if (id != ward.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ward.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    ward.CreatedOn = DateTime.Today;
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardExists(ward.Id))
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
            return View(ward);
        }

        // GET: Wards/Delete/5
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

            var ward = await _context.Ward
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // POST: Wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var ward = await _context.Ward.FindAsync(id);
            _context.Ward.Remove(ward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardExists(long id)
        {
            return _context.Ward.Any(e => e.Id == id);
        }
    }
}
