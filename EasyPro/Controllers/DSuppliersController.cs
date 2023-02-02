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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            sacco = sacco ?? "";
            ViewData["Getsuppliers"]= Search;
            var suppliers = from x in _context.DSuppliers
                .Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()) && i.Approval) select x;
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(u => u.Branch == saccoBranch);
            if (!string.IsNullOrEmpty(Search))
                suppliers = suppliers.Where(x => x.IdNo.Contains(Search) || x.Names.ToUpper().Contains(Search.ToUpper()) || x.PhoneNo.Contains(Search) || x.Sno.ToString().Contains(Search));
            return View(await PaginatedList<DSupplier>.CreateAsync(suppliers.OrderByDescending(s => s.Id).AsNoTracking(), pageNumber ?? 1, pageSize ?? 20));
        }//return Json(shares);
        [HttpGet]
        public async Task<IActionResult> UnApprovedList(string Search)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            sacco = sacco ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var suppliers = from x in _context.DSuppliers
                    .Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()) && !i.Approval)
                            select x;
            try
            {
                

                ViewData["Getsuppliers"] = Search;
                
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    suppliers = suppliers.Where(u => u.Branch == saccoBranch);
                if (!string.IsNullOrEmpty(Search))
                    suppliers = suppliers.Where(x => x.IdNo.Contains(Search) || x.Names.ToUpper().Contains(Search.ToUpper()) || x.PhoneNo.Contains(Search) || x.Sno.ToString().Contains(Search));

                
                return View(await suppliers.AsNoTracking().ToListAsync());
            }
            catch(Exception e)
            {
                _notyf.Error("Sorry, Record Not exist");
                return RedirectToAction(nameof(UnApprovedList));
            }
            
        }
        public async Task<IActionResult> SaccoSupplierSummery()
        {
            utilities.SetUpPrivileges(this);
            var counties = _context.County.Select(c => c.Name).ToList();
            ViewBag.counties = new SelectList(counties);
            var countyPOS = _context.DCompanies.ToList().GroupBy(s => s.Province).ToList();
            var countySaccos = new List<CountySupplierSummery>();
            var totalSuppliers = 0;
            var totalMale = 0;
            var totalFemale = 0;
            countyPOS.ForEach(h=> {
                var saccoSuppliers = new List<SaccoSupplierSummery>();
                var totalCountySuppliers = 0;
                var totalCountyMale = 0;
                var totalCountyFemale = 0;
                var sacconame = _context.DCompanies.Where(m=>m.Province.ToUpper().Equals(h.Key.ToUpper())).ToList().GroupBy(s => s.Name).ToList();
                sacconame.ForEach(g=> {
                    //var getcountySuppliers = _context.DSuppliers.Where(j=>j.Scode.ToUpper().Equals(g.Key.ToUpper())).ToList().GroupBy(s => s.Scode).ToList();
                    var getcountySuppliers = _context.DSuppliers.Where(j=>j.Scode.ToUpper().Equals(g.Key.ToUpper())).ToList();
                    //getcountySuppliers.ForEach(s=> {
                        var total = getcountySuppliers.Count();
                        var male = getcountySuppliers.Where(g => g.Type.ToLower().Equals("male")).Count();
                        var female = getcountySuppliers.Where(g => g.Type.ToLower().Equals("female")).Count();
                        totalCountySuppliers += total;
                        totalCountyMale += male;
                        totalCountyFemale += female;
                        saccoSuppliers.Add(new SaccoSupplierSummery
                        {
                            Sacco = g.Key,
                            Suppliers = total,
                            Male = male,
                            Female = female
                        });
                    //});

                });
                //var countySuppliers = _context.DSuppliers.ToList().GroupBy(s => s.Scode).ToList();
                //countySuppliers.ForEach(c =>
                //{
                //    var saccoSuppliers = new List<SaccoSupplierSummery>();
                //    var totalCountySuppliers = 0;
                //    var totalCountyMale = 0;
                //    var totalCountyFemale = 0;
               
                //    var groupedSuppliers = c.ToList().GroupBy(s => s.Scode).ToList();
                //    groupedSuppliers.ForEach(s =>
                //    {
                //        var total = s.Count();
                //        var male = s.Where(g => g.Type.ToLower().Equals("male")).Count();
                //        var female = s.Where(g => g.Type.ToLower().Equals("female")).Count();
                //        totalCountySuppliers += total;
                //        totalCountyMale += male;
                //        totalCountyFemale += female;
                //        saccoSuppliers.Add(new SaccoSupplierSummery
                //        {
                //            Sacco = s.Key,
                //            Suppliers = total,
                //            Male = male,
                //            Female = female
                //        });
                //    });

                //    countySaccos.Add(new CountySupplierSummery
                //    {
                //        County = c.Key,
                //        suppliers = saccoSuppliers,
                //        TotalSupplies = totalCountySuppliers,
                //        TotalMale = totalCountyMale,
                //        TotalFemale = totalCountyFemale
                //    });

                //    totalSuppliers += totalCountySuppliers;
                //    totalMale += totalCountyMale;
                //    totalFemale += totalCountyFemale;
                //});

                countySaccos.Add(new CountySupplierSummery
                {
                    County = h.Key,
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
        public async Task<IActionResult> SaccoSupplierSummerycounty()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var counties = _context.County.Select(c => c.Name).ToList();
            ViewBag.counties = new SelectList(counties);
            var countyPOS = _context.DCompanies.ToList().GroupBy(s => s.Province).ToList();
            var countySaccos = new List<CountySupplierSummery>();
            var totalSuppliers = 0;
            var totalMale = 0;
            var totalFemale = 0;
            countyPOS.ForEach(h => {
                var saccoSuppliers = new List<SaccoSupplierSummery>();
                var totalCountySuppliers = 0;
                var totalCountyMale = 0;
                var totalCountyFemale = 0;
                var sacconame = _context.DCompanies.Where(m => m.Province.ToUpper().Equals(h.Key.ToUpper())).ToList().GroupBy(s => s.Name).ToList();
                sacconame.ForEach(g => {
                   // var getcountySuppliers = _context.DSuppliers.Where(j => j.Scode.ToUpper().Equals(g.Key.ToUpper())).ToList().GroupBy(s => s.Scode).ToList();
                    var getcountySuppliers = _context.DSuppliers.Where(j => j.Scode.ToUpper().Equals(g.Key.ToUpper())).ToList();
                    //getcountySuppliers.ForEach(s => {
                        var total = getcountySuppliers.Count();
                        var male = getcountySuppliers.Where(g => g.Type.ToLower().Equals("male")).Count();
                        var female = getcountySuppliers.Where(g => g.Type.ToLower().Equals("female")).Count();
                        totalCountySuppliers += total;
                        totalCountyMale += male;
                        totalCountyFemale += female;
                        saccoSuppliers.Add(new SaccoSupplierSummery
                        {
                            Sacco = g.Key,
                            Suppliers = total,
                            Male = male,
                            Female = female
                        });
                    //});

                });

                countySaccos.Add(new CountySupplierSummery
                {
                    County = h.Key,
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
        public async Task<IActionResult> SaccoSuppliersdetails(string id)
        {

            utilities.SetUpPrivileges(this);
            ViewBag.sacco = id;
            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(id.ToUpper()));
            return View();
        }
        [HttpPost] 
        public JsonResult getdetails([FromBody] CountyReportsVM CountyReport)
        {
            //var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var month = new DateTime(CountyReport.date2.Year, CountyReport.date2.Month, 1);
            var startDate = month.AddMonths(-1);
            var endDate = month.AddDays(-1);


            var checkbranchenquiry = _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(CountyReport.sacco.ToUpper())
            && i.TransDate >= startDate && i.TransDate <= CountyReport.date2 && i.Description!= "Transport").OrderByDescending(l => l.ProductType).ToList();
            var CountyReports = new List<ProductIntakelist>();
            if (checkbranchenquiry.Count > 0)
            {
                var transGroups = checkbranchenquiry.GroupBy(t => t.ProductType).ToList();
                transGroups.ForEach(l =>
                {
                    var intakes = l.FirstOrDefault();
                    var productIntakesdetails = checkbranchenquiry.Where(b=>b.ProductType.ToUpper().Equals(intakes.ProductType.ToUpper())).ToList();
                    CountyReports.Add(new ProductIntakelist
                    {
                        Id = intakes.Id,
                        Type = l.Key,
                        Amount = (productIntakesdetails.Sum(v=>v.CR)- productIntakesdetails.Sum(c=>c.DR)) ?? 0,

                    });
                    _context.SaveChanges();
                });
            }

            CountyReports = CountyReports.ToList();

            return Json(new
            {
                CountyReports,
                Expense = checkbranchenquiry.Sum(l => l.DR),
                Income = checkbranchenquiry.Sum(l => l.CR),
            });
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            sacco = sacco ?? "";
            ViewBag.sacco = sacco;
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var counties = _context.County.OrderBy(K => K.Name).Select(b => b.Name).ToList();
            ViewBag.counties = new SelectList(counties);
            var SubCountyName = _context.SubCounty.OrderBy(K => K.Name).ToList();
            ViewBag.SubCountyName = SubCountyName;
            ViewBag.subCounties = new SelectList(SubCountyName, "Name", "Name");
            var WardSubCounty = _context.Ward.OrderBy(K => K.Name).ToList();
            ViewBag.WardSubCounty = WardSubCounty;
            ViewBag.wards = new SelectList(WardSubCounty, "Name", "Name");
            var brances = _context.DBranch.Where(a => a.Bcode == sacco).OrderBy(K => K.Bname).ToList();

            var locations = _context.DLocations.Where(l => l.Lcode == sacco).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                locations = locations.Where(t => t.Branch == saccoBranch).ToList();
                brances = brances.Where(l=>l.Bname== saccoBranch).ToList();
            }

            ViewBag.brances = new SelectList(brances.Select(b => b.Bname));
            ViewBag.locations = new SelectList(locations.OrderBy(K => K.Lname).Select(b => b.Lname));

            var banksname = _context.DBanks.Where(a=>a.BankCode == sacco).OrderBy(K => K.BankName).Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            var bankbrances = _context.DBankBranch.Where(a => a.BankCode == sacco).OrderBy(K => K.Bname).Select(b => b.Bname).ToList();
            ViewBag.bankbrances = new SelectList(bankbrances);

            var routes = _context.Routes.Where(a => a.scode == sacco ).OrderBy(m => m.Name).Select(b => b.Name).ToList();
            ViewBag.routes = new SelectList(routes);


            List<SelectListItem> gender = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "Male" },
                new SelectListItem { Text = "Female" },
            };
            ViewBag.gender = gender;
            List<SelectListItem> payment = new()
            {
                new SelectListItem { Text = "" },
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

            var dSupplierExists = _context.DSuppliers.Any(i => i.Sno == dSupplier.Sno
            && i.Scode == sacco && i.Branch == saccoBranch && i.Zone== dSupplier.Zone);
            if (dSupplierExists)
            {
                //var sup = _context.DSuppliers.Where(i => i.Scode == sacco && i.Sno == dSupplier1.)
                GetInitialValues();
                _notyf.Error("Sorry, The Supplier Number already exist");
                return View();
            }
            var dSupplierExistsIDNo = _context.DSuppliers.Any(i => i.IdNo == dSupplier.IdNo
            && i.Scode == sacco && i.Branch == saccoBranch && i.Zone == dSupplier.Zone);
            if (dSupplierExistsIDNo)
            {
                //var sup = _context.DSuppliers.Where(i => i.Scode == sacco && i.Sno == dSupplier1.)
                GetInitialValues();
                _notyf.Error("Sorry, The Supplier IDNo already exist");
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

        [HttpPost]
        public JsonResult getsuppliers([FromBody] DSupplier supplier,string? filter, string? condition)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var companyTanykina = HttpContext.Session.GetString(StrValues.Tanykina);

            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            if(companyTanykina!="TANYKINA Dairy Plant Limited")
            {
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    suppliers = suppliers.Where(i => i.Branch == saccobranch).ToList();
            }
            
            if(!string.IsNullOrEmpty(filter) )
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    if (condition == "SNo")
                    {
                        suppliers = suppliers.Where(i => i.Sno.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "Name")
                    {
                        suppliers = suppliers.Where(i => i.Names.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "IdNo")
                    {
                        suppliers = suppliers.Where(i => i.IdNo.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "Phone")
                    {
                        suppliers = suppliers.Where(i => i.PhoneNo.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                    if (condition == "AccNo")
                    {
                        suppliers = suppliers.Where(i => i.AccNo.ToUpper().Contains(filter.ToUpper())).ToList();
                    }
                }
            }

            suppliers = suppliers.OrderByDescending(i => i.Sno).Take(15).ToList();
            return Json(suppliers);
        }


        private bool DSupplierExists(long id)
        {
            return _context.DSuppliers.Any(e => e.Id == id);
        }
    }
}
