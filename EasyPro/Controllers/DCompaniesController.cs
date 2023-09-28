using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasyPro.Models;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class DCompaniesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DCompaniesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DCompanies
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var companies = _context.DCompanies.OrderBy(K => K.Name).ToList();
            ViewBag.checkiftoenable = 1;
            if (!loggedInUser.ToLower().Equals("psigei"))
            {
                ViewBag.checkiftoenable = 0;
                companies = _context.DCompanies.Where(i=>i.Name.ToUpper().Equals(sacco.ToUpper())).ToList();
            }
            return View(companies);
        }

        // GET: DCompanies/Details/5
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        // POST: DCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Adress,Town,Country,Province,District,Division,Location,FaxNo,PhoneNo,Email,Website,Fiscal,Auditid,Auditdatetime,Acc,Motto,SendTime,Smsno,Smscost,Smsport,Period,SupStatementNote")] DCompany dCompany)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            try
            {
                if (ModelState.IsValid)
                {
                    dCompany.Name = dCompany?.Name ?? "";
                    if (_context.DCompanies.Any(c => c.Name.ToUpper().Equals(dCompany.Name.ToUpper())))
                    {
                        _notyf.Error("Sorry, Society already exist");
                        return View(dCompany);
                    }
                    _context.Add(dCompany);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Saciety saved successfully");
                    return RedirectToAction(nameof(Index));
                }
                return View(dCompany);
            }
            catch (Exception ex)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(dCompany);
            }
        }

        public IActionResult SocietyStandingOrder()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            utilities.SetUpPrivileges(this);
            return View(_context.SocietyStandingOrder.Where(o => o.SaccoCode == sacco).ToList());
        }

        public IActionResult CreateSctyStandingOrder()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            utilities.SetUpPrivileges(this);
            var glsetups = _context.Glsetups.Where(g => g.saccocode == sacco).ToList();
            ViewBag.glsetups = new SelectList(glsetups, "AccNo", "GlAccName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSctyStandingOrder([Bind("Id,Date,Name,GlAcc,ContraAcc,HasRate,Amount,SaccoCode,AuditId")] SocietyStandingOrder standingOrder)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(standingOrder.Name))
                {
                    _notyf.Error("Sorry, Kindly provide name");
                    return View(standingOrder);
                }
                if (string.IsNullOrEmpty(standingOrder.GlAcc))
                {
                    _notyf.Error("Sorry, Kindly provide Gl Account");
                    return View(standingOrder);
                }
                if (string.IsNullOrEmpty(standingOrder.ContraAcc))
                {
                    _notyf.Error("Sorry, Kindly provide Contra Account");
                    return View(standingOrder);
                }
                if (standingOrder.Amount < 0.01M)
                {
                    _notyf.Error("Sorry, Kindly provide amount");
                    return View(standingOrder);
                }
                if (_context.SocietyStandingOrder.Any(o => o.Name.ToUpper().Equals(standingOrder.Name.ToUpper())))
                {
                    _notyf.Error("Sorry, Standing order already exist");
                    return View(standingOrder);
                }

                standingOrder.AuditId = loggedInUser;
                standingOrder.SaccoCode = sacco;
                standingOrder.Date = DateTime.Today;
                _context.Add(standingOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SocietyStandingOrder));
            }
            return View(standingOrder);
        }

        // GET: DCompanies/Edit/5
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

            var dCompany = await _context.DCompanies.FindAsync(id);
            if (dCompany == null)
            {
                return NotFound();
            }
            return View(dCompany);
        }

        private void SetInitialValues()
        {
            var counties = _context.County.Where(c => !c.Closed).OrderBy(m => m.Name).ToList();
            ViewBag.counties = new SelectList(counties.OrderBy(n=>n.Name).ToList().Select(b => b.Name).ToList());
            var subCounties = _context.SubCounty.Where(c => !c.Closed).OrderBy(m => m.Name).ToList();
            ViewBag.subCounties = subCounties;
            ViewBag.SubCountyName = new SelectList(subCounties, "Name", "Name");
            var wards = _context.Ward.Where(c => !c.Closed).OrderBy(m => m.Name).ToList();
            ViewBag.wards = wards;
            ViewBag.WardSubCounty = new SelectList(wards, "Name", "Name");
            var locations = _context.DLocations.OrderBy(m => m.Lname).ToList();
            ViewBag.locations = new SelectList(locations, "Lcode", "Lname");
        }



        // POST: DCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Adress,Town,Country,Province,District,Division,Location,FaxNo,PhoneNo,Email,Website,Fiscal,Auditid,Auditdatetime,Acc,Motto,SendTime,Smsno,Smscost,Smsport,Period,SupStatementNote")] DCompany dCompany)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
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
                    _notyf.Error("Sorry, An error occurred");
                    if (!DCompanyExists(dCompany.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _notyf.Success("Society edited successfully");
                return RedirectToAction(nameof(Index));
            }
            _notyf.Error("Sorry, An error occurred");
            return View(dCompany);
        }

        // GET: DCompanies/Delete/5
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dCompany = await _context.DCompanies.FindAsync(id);
            _context.DCompanies.Remove(dCompany);
            await _context.SaveChangesAsync();
            _notyf.Success("Saciety deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DCompanyExists(long id)
        {
            return _context.DCompanies.Any(e => e.Id == id);
        }
    }
}
