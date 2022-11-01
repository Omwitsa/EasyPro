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
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.ViewModels;

namespace EasyPro.Controllers
{
    public class DSuppliersController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DSuppliersController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        // GET: DSuppliers
        
        [HttpGet]
        public async Task<IActionResult> Index(string Search,int? pageNumber, int? pageSize)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";

            ViewData["Getsuppliers"]= Search;
            var suppliers = from x in _context.DSuppliers
                .Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()) && i.Approval) select x;
            if (!string.IsNullOrEmpty(Search))
                suppliers = suppliers.Where(x => x.IdNo.Contains(Search) || x.Names.ToUpper().Contains(Search.ToUpper()) || x.PhoneNo.Contains(Search) || x.Sno.ToString().Contains(Search));
            return View(await PaginatedList<DSupplier>.CreateAsync(suppliers.OrderByDescending(s => s.Id).AsNoTracking(), pageNumber ?? 1, pageSize ?? 20));
        }//return Json(shares);
        [HttpGet]
        public async Task<IActionResult> UnApprovedList(string Search)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);

            ViewData["Getsuppliers"] = Search;
            var suppliers = from x in _context.DSuppliers
                .Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()) && !i.Approval) select x;
            if (!string.IsNullOrEmpty(Search))
                suppliers = suppliers.Where(x => x.IdNo.Contains(Search) || x.Names.ToUpper().Contains(Search.ToUpper()) || x.PhoneNo.Contains(Search) || x.Sno.ToString().Contains(Search));
            return View(await suppliers.AsNoTracking().ToListAsync());
        }
        public async Task<IActionResult> SaccoSupplierSummery()
        {
            utilities.SetUpPrivileges(this);
            var counties = _context.County.Select(c => c.Name).ToList();
            ViewBag.counties = new SelectList(counties);
            var countySuppliers = _context.DSuppliers.ToList().GroupBy(s => s.County).ToList();
            var countySaccos = new List<CountySupplierSummery>();
            var totalSuppliers = 0;
            var totalMale = 0;
            var totalFemale = 0;

            countySuppliers.ForEach(c =>
            {
                var saccoSuppliers = new List<SaccoSupplierSummery>();
                var totalCountySuppliers = 0;
                var totalCountyMale = 0;
                var totalCountyFemale = 0;
               
                var groupedSuppliers = c.ToList().GroupBy(s => s.Scode).ToList();
                groupedSuppliers.ForEach(s =>
                {
                    var total = s.Count();
                    var male = s.Where(g => g.Type.ToLower().Equals("male")).Count();
                    var female = s.Where(g => g.Type.ToLower().Equals("female")).Count();
                    totalCountySuppliers += total;
                    totalCountyMale += male;
                    totalCountyFemale += female;
                    saccoSuppliers.Add(new SaccoSupplierSummery
                    {
                        Sacco = s.Key,
                        Suppliers = total,
                        Male = male,
                        Female = female
                    });
                });

                countySaccos.Add(new CountySupplierSummery
                {
                    County = c.Key,
                    suppliers = saccoSuppliers,
                    TotalSupplies = totalCountySuppliers,
                    TotalMale = totalCountyMale,
                    TotalFemale = totalCountyFemale
                });

                totalSuppliers += totalCountySuppliers;
                totalMale += totalCountyMale;
                totalFemale += totalCountyFemale;
            });

            var supplierSummery = new SupplierSummeryVm
            {
                countySuppliers = countySaccos,
                TotalSupplies = totalSuppliers,
                TotalMale = totalMale,
                TotalFemale = totalFemale
            };
            return View(supplierSummery);
        }

        public async Task<IActionResult> SaccoSuppliers(string id)
        {

            utilities.SetUpPrivileges(this);
            ViewBag.sacco = id;
            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(id.ToUpper()));
            return View(suppliers);
        }

        // GET: DSuppliers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dSupplier = await _context.DSuppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplier == null)
            {
                return NotFound();
            }

            return View(dSupplier);
        }

        // GET: DSuppliers/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();

            return View(new DSupplier { 
                Active = true,
                Regdate=DateTime.Today,
                Dob = DateTime.Today
            });
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";
            ViewBag.sacco = sacco;
            var counties = _context.County.Select(b => b.Name).ToList();
            ViewBag.counties = new SelectList(counties);
            var SubCountyName = _context.SubCounty.ToList();
            ViewBag.SubCountyName = SubCountyName;
            ViewBag.subCounties = new SelectList(SubCountyName, "Name", "Name");
            var WardSubCounty = _context.Ward.ToList();
            ViewBag.WardSubCounty = WardSubCounty;
            ViewBag.wards = new SelectList(WardSubCounty, "Name", "Name");
            var locations = _context.DLocations.Where(l => l.Lcode == sacco && l.Branch == saccoBranch).Select(b => b.Lname).ToList();
            ViewBag.locations = new SelectList(locations);

            var banksname = _context.DBanks.Where(a=>a.BankCode == sacco).Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            var brances = _context.DBranch.Where(a => a.Bcode == sacco).Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

            var bankbrances = _context.DBankBranch.Where(a => a.BankCode == sacco).Select(b => b.Bname).ToList();
            ViewBag.bankbrances = new SelectList(bankbrances);

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

            if (!string.IsNullOrEmpty(zones.ToString()))
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;


            List<SelectListItem> gender = new()
            {
                new SelectListItem { Text = "Male" },
                new SelectListItem { Text = "Female" },
            };
            ViewBag.gender = gender;
            List<SelectListItem> payment = new()
            {
                new SelectListItem {  Text = "Weekly" },
                new SelectListItem { Text = "Monthly" },
            };
            ViewBag.payment = payment;
            List<SelectListItem> approved = new()
            {
                new SelectListItem { Value="0", Text = "No" },
                new SelectListItem { Value = "1", Text = "Yes" },
            };
            ViewBag.approved = approved;
        }
        // POST: DSuppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LocalId,Sno,Regdate,IdNo,Names,AccNo,Bcode,Bbranch,Type,Village,Location,Division,District,County,Trader,Active,Approval,Branch,PhoneNo,Address,Town,Email,TransCode,Sign,Photo,AuditId,Auditdatetime,Scode,Loan,Compare,Isfrate,Frate,Rate,Hast,Br,Mno,Branchcode,HasNursery,Notrees,Aarno,Tmd,Landsize,Thcpactive,Thcppremium,Status,Status2,Status3,Status4,Status5,Status6,Types,Dob,Freezed,Mass,Status1,Run,Zone")] DSupplier dSupplier)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";
            if (dSupplier == null)
            {
                _notyf.Error("Sorry, Supplier code cannot be empty");
                return NotFound();
            }

            var dSupplierExists = _context.DSuppliers.Any(i => (i.Sno == dSupplier.Sno || i.IdNo == dSupplier.IdNo)
            && i.Scode == sacco && i.Branch == saccoBranch && i.Zone== dSupplier.Zone);
            if (dSupplierExists)
            {
                //var sup = _context.DSuppliers.Where(i => i.Scode == sacco && i.Sno == dSupplier1.)
                GetInitialValues();
                _notyf.Error("Sorry, The Supplier already exist");
                return View();
            }
            //}

            if (ModelState.IsValid)
            {
                dSupplier.Scode = sacco;

                _context.Add(dSupplier);
                await _context.SaveChangesAsync();
                _notyf.Success("The Supplier saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dSupplier);
        }

        // GET: DSuppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }
            var dSupplier = await _context.DSuppliers.FindAsync(id);
            if (dSupplier == null)
            {
                return NotFound();
            }

            return View(dSupplier);
        }

        // POST: DSuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,LocalId,Sno,Regdate,IdNo,Names,AccNo,Bcode,Bbranch,Type,Village,Location,Division,District,County,Trader,Active,Approval,Branch,PhoneNo,Address,Town,Email,TransCode,Sign,Photo,AuditId,Auditdatetime,Scode,Loan,Compare,Isfrate,Frate,Rate,Hast,Br,Mno,Branchcode,HasNursery,Notrees,Aarno,Tmd,Landsize,Thcpactive,Thcppremium,Status,Status2,Status3,Status4,Status5,Status6,Types,Dob,Freezed,Mass,Status1,Run,Zone")] DSupplier dSupplier)
        {
            utilities.SetUpPrivileges(this);
            if (id != dSupplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dSupplier.Sno = dSupplier.Sno;
                    dSupplier.Regdate = dSupplier.Regdate;
                    dSupplier.Trader = false;
                    //dSupplier.Approval = false;
                    dSupplier.Br = "A";
                    dSupplier.Freezed = "0";
                    dSupplier.Mass = "0";
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                    sacco = sacco ?? "";
                    dSupplier.Zone = dSupplier.Zone;
                    dSupplier.Scode = sacco;
                    _context.Update(dSupplier);
                    await _context.SaveChangesAsync();
                    _notyf.Success("The Supplier edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DSupplierExists(dSupplier.Id))
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
            return View(dSupplier);
        }

        // GET: DSuppliers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dSupplier = await _context.DSuppliers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplier == null)
            {
                return NotFound();
            }

            return View(dSupplier);
        }

        // POST: DSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dSupplier = await _context.DSuppliers.FindAsync(id);
            _context.DSuppliers.Remove(dSupplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DSupplierExists(long id)
        {
            return _context.DSuppliers.Any(e => e.Id == id);
        }
    }
}
