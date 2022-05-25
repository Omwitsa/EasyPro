using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasyPro.Models;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;

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
            utilities.SetUpPrivileges(this);
            return View(await _context.DCompanies.ToListAsync());
        }

        // GET: DCompanies/Details/5
        public async Task<IActionResult> Details(long? id)
        {
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
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        // POST: DCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Adress,Town,Country,Province,District,Division,Location,FaxNo,PhoneNo,Email,Website,Fiscal,Auditid,Auditdatetime,Acc,Motto,SendTime,Smsno,Smscost,Smsport,Period")] DCompany dCompany)
        {
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

        // GET: DCompanies/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
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
            var counties = _context.County.Where(c => !c.Closed).ToList();
            ViewBag.counties = new SelectList(counties, "Name", "Name");
            var subCounties = _context.SubCounty.Where(c => !c.Closed).ToList();
            ViewBag.subCounties = new SelectList(subCounties, "Name", "Name");
            var wards = _context.Ward.Where(c => !c.Closed).ToList();
            ViewBag.wards = new SelectList(wards, "Name", "Name");
            var locations = _context.DLocations.ToList();
            ViewBag.locations = new SelectList(locations, "Lcode", "Lname");
        }



        // POST: DCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Adress,Town,Country,Province,District,Division,Location,FaxNo,PhoneNo,Email,Website,Fiscal,Auditid,Auditdatetime,Acc,Motto,SendTime,Smsno,Smscost,Smsport,Period")] DCompany dCompany)
        {
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
            utilities.SetUpPrivileges(this);
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
