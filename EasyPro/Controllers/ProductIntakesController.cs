using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.FarmersVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Neodynamic.SDK.Web;
using Grpc.Core;
using System.Diagnostics;
using Stripe;
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.IProvider;
using EasyPro.Provider;
using NPOI.SS.Formula.Functions;
using static EasyPro.ViewModels.AccountingVm;
using DocumentFormat.OpenXml.Wordprocessing;

namespace EasyPro.Controllers
{
    public class ProductIntakesController : Controller
    {
        PrintDocument pdoc = null;
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        private static object clientSock;

        public FarmersVM Farmersobj { get; private set; }


        public ProductIntakesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        // GET: ProductIntakes
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var intakes = _context.ProductIntake.Where(i => i.TransactionType == TransactionType.Intake
            && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.TransDate == DateTime.Today);
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch);

            // var intaelist = await intakes.OrderByDescending(l => l.Auditdatetime).ToListAsync();
            return View(intakes.OrderByDescending(l => l.Auditdatetime).ToList());
        }
        [HttpGet]
        public JsonResult listcorrectionIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var intakes = _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Correction && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && i.TransDate == date);
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch);
            //intakes=intakes.OrderByDescending(l => l.Auditdatetime).ToList();
            return Json(intakes.OrderByDescending(l => l.Auditdatetime).ToList());
        }
        [HttpGet]
        public JsonResult listIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var intakes = _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && i.TransDate == date);
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch);
            //intakes=intakes.OrderByDescending(l => l.Auditdatetime).ToList();
            return Json(intakes.OrderByDescending(l => l.Auditdatetime).ToList());
        }
        public async Task<IActionResult> TDeductionList()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction
            && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                productIntakes = productIntakes.Where(i => i.Branch == saccoBranch).ToList();
            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                var supplier = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.Sno);
                if (supplier != null)
                {
                    intakes.Add(new ProductIntakeVm
                    {
                        Sno = supplier.TransCode,
                        SupName = supplier.TransName,
                        TransDate = intake.TransDate,
                        ProductType = intake.ProductType,
                        DR = intake.DR,
                        Remarks = intake.Remarks,
                        Branch = intake.Branch
                    });
                }
            }
            return View(intakes);
        }
        public async Task<IActionResult> DeductionList()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction
            && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                productIntakes = productIntakes.Where(i => i.Branch == saccoBranch).ToList();

            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == intake.Sno && i.Scode == sacco);
                if (supplier != null)
                {
                    if (intake.CR < 0)
                    {
                        intake.CR = 0;
                        intake.DR = intake.CR * -1;
                    }
                    intakes.Add(new ProductIntakeVm
                    {
                        Sno = intake.Sno,
                        SupName = supplier.Names,
                        TransDate = intake.TransDate,
                        ProductType = intake.ProductType,
                        Qsupplied = intake.Qsupplied,
                        Ppu = intake.Ppu,
                        CR = intake.CR,
                        DR = intake.DR,
                        Balance = intake.Balance,
                        Description = intake.Description,
                        Remarks = intake.Remarks,
                        Branch = intake.Branch
                    });
                }
            }
            return View(intakes);
        }
        public async Task<IActionResult> CorrectionList()
        {
            utilities.SetUpPrivileges(this);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var intakes = _context.ProductIntake
                .Where(c => c.TransactionType == TransactionType.Correction && c.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && c.TransDate == DateTime.Today);

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch);
            return View(await intakes.OrderByDescending(l => l.Auditdatetime).ToListAsync());
        }
        public async Task<IActionResult> StaffDeductionList()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var productIntakes = await _context.EmployeesDed.Where(c => c.Deduction != "Store" && c.saccocode == sacco
            && c.Date == DateTime.Today).ToListAsync();
            var intakes = new List<EmployeeVm>();
            foreach (var intake in productIntakes)
            {
                var emploeyeename = _context.Employees.FirstOrDefault(i => i.EmpNo == intake.Empno && i.SaccoCode == sacco);
                if (emploeyeename != null)
                {
                    intakes.Add(new EmployeeVm
                    {
                        Empno = intake.Empno,
                        Name = emploeyeename.Surname + " " + emploeyeename.Othernames,
                        Date = intake.Date,
                        Deduction = intake.Deduction,
                        Amount = intake.Amount,
                        Remarks = intake.Remarks
                    });
                }
            }
            return View(intakes);
        }

        // GET: ProductIntakes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productIntake = await _context.ProductIntake
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productIntake == null)
            {
                return NotFound();
            }

            return View(productIntake);
        }

        // GET: ProductIntakes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var Todayskg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today).ToList().Sum(p => p.Qsupplied);
            var TodaysBranchkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today && s.Branch == saccoBranch).ToList().Sum(p => p.Qsupplied);
            return View(new ProductIntakeVm
            {
                Todaykgs = Todayskg,
                TodayBranchkgs = TodaysBranchkg
            });
        }

        public IActionResult PrintSupplierStatement()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var branches = _context.DBranch.Where(s => s.Bcode == sacco)
                .Select(s => s.Bname).ToList();

            ViewBag.branches = new SelectList(branches);

            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()));
            ViewBag.suppliers = suppliers.Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
            }).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult PrintSupplierStatement([FromBody] StatementFilter filter)
        {
            utilities.SetUpPrivileges(this);
            SetIntakeInitialValues();
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;

            if (filter.Sacco == "MBURUGU DAIRY F.C.S" && !string.IsNullOrEmpty(filter.Code))
            {
                var deletetransport = productIntakeslist.Where(n => n.Sno.ToUpper().Equals(filter.Code.ToUpper())
                && n.SaccoCode.ToUpper().Equals(filter.Sacco.ToUpper())
                && n.Description == "Transport" && n.TransactionType == TransactionType.Deduction && n.TransDate >= startDate
                && n.TransDate <= endDate).ToList();
                _context.RemoveRange(deletetransport);
                //check transport rate

                var getpricegls = _context.DPrices.FirstOrDefault(j => j.SaccoCode.ToUpper().Equals(filter.Sacco.ToUpper()));

                var gettransportersrate = _context.DTransports.FirstOrDefault(h => h.Sno.ToUpper().Equals(filter.Code.ToUpper())
                && h.saccocode.ToUpper().Equals(filter.Sacco.ToUpper()));
                if (gettransportersrate != null)
                {
                    decimal Rate = 0;
                    Rate = (decimal)gettransportersrate.Rate;

                    var sumkgs = productIntakeslist.Where(i => i.Sno.ToUpper().Equals(filter.Code.ToUpper())
                    && i.SaccoCode.ToUpper().Equals(filter.Sacco.ToUpper())
                    && (i.TransactionType == TransactionType.Intake || i.TransactionType == TransactionType.Correction)
                    && i.TransDate >= startDate && i.TransDate <= endDate).ToList().Sum(n => n.Qsupplied);
                    var actualrate = Rate * sumkgs;

                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = filter.Code.ToUpper(),
                        TransDate = (DateTime)endDate,
                        TransTime = DateTime.Now.TimeOfDay,
                        ProductType = getpricegls.Products,
                        Qsupplied = sumkgs,
                        Ppu = Rate,
                        CR = 0,
                        DR = actualrate,
                        Balance = actualrate,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = "",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = filter.Branch,
                        SaccoCode = filter.Sacco,
                        DrAccNo = getpricegls.TransportCrAccNo,
                        CrAccNo = getpricegls.TransportDrAccNo,

                    });
                    _context.SaveChanges();
                }

            }

            var statement = new SupplierStatement(_context);
            var statementResp = statement.GenerateStatement(filter);
            return Json(statementResp);
        }

        public IActionResult PrintTransporterStatement()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var branches = _context.DBranch.Where(s => s.Bcode == sacco)
                .Select(s => s.Bname).ToList();

            ViewBag.branches = new SelectList(branches);

            var transporters = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper()));
            ViewBag.transporters = transporters.Select(s => new DTransporter
            {
                TransName = s.TransName,
                TransCode = s.TransCode,
            }).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult PrintTransporterStatement([FromBody] StatementFilter filter)
        {
            utilities.SetUpPrivileges(this);
            SetIntakeInitialValues();
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var statement = new TransporterStatement(_context);
            var statementResp = statement.GenerateStatement(filter);
            return Json(statementResp);
        }

        [HttpGet]
        public JsonResult SelectedDateIntake(string sno)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);

            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco && L.Branch == saccoBranch).Select(b => b.Names).ToList();

            return Json(todaysIntake);
        }

        [HttpGet]
        public JsonResult checkifalreadyexist(string sno, DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var checkifdelievedtoday = _context.ProductIntake.Any(L => L.SaccoCode == sacco
            && L.Branch == saccoBranch && L.TransDate == date && L.Sno.ToUpper().Equals(sno.ToUpper()));
            return Json(checkifdelievedtoday);
        }
        
        [HttpGet]
        public JsonResult SelectedName(string sno)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco);
            return Json(todaysIntake.Select(b => b.Names).ToList());
        }
        
        [HttpGet]
        public JsonResult sumDateIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var Todayskg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date).Sum(p => p.Qsupplied);
            var TodaysSubtractedkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && s.DR>0 && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date).Sum(p => p.Qsupplied);
            var TodaysBranchkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
            var TodaysBranchkgSubtracted = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && s.DR>0 && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date && s.Branch == saccoBranch).Sum(p => p.Qsupplied);

            return Json(new dailymilkVM
            {
                Todayskg = Todayskg - TodaysSubtractedkg,
                TodaysBranchkg = TodaysBranchkg- TodaysBranchkgSubtracted
            });
        }
        private void SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var suppliers = _context.DSuppliers
                .Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();
            ViewBag.suppliers = suppliers;
            var products = _context.DPrices.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.products = new SelectList(products, "Products", "Products");
            ViewBag.productPrices = products;

            var Branch = _context.DBranch.Where(a => a.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.Branch = new SelectList(Branch);

            var zones  = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

            if (zones.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;

            List<SelectListItem> morningEve = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "Morning" },
                new SelectListItem { Text = "Evening" },
            };
            ViewBag.morningEve = morningEve;

        }

        public IActionResult CreateDeduction()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco);
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);
            Farmersobj = new FarmersVM()
            {
                DSuppliers = suppliers,
                ProductIntake = new ProductIntake
                {
                    TransDate = DateTime.Today
                }
            };
            return View(Farmersobj);
        }

        public IActionResult CreateTDeduction()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            Farmersobj = new FarmersVM()
            {
                DTransporters = _context.DTransporters.Where(t => t.ParentT == sacco && t.Tbranch == saccoBranch),
                ProductIntake = new ProductIntake
                {
                    TransDate = DateTime.Today
                }
            };
            return View(Farmersobj);
        }
        public IActionResult CreateStaffDeduction()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            Farmersobj = new FarmersVM()
            {
                Employees = _context.Employees.Where(t => t.SaccoCode == sacco),
                EmployeesDed = new EmployeesDed
                {
                    Date = DateTime.Today
                }
            };
            return View(Farmersobj);
        }
        private void GetInitialValues()
        {
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var Descriptionname = _context.DDcodes.Where(d => d.Dcode == sacco).Select(b => b.Description).ToList();
            ViewBag.Description = new SelectList(Descriptionname);
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var brances = _context.DBranch.Where(b => b.Bcode == sacco).ToList();


            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(LoggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                brances = brances.Where(i => i.Bname == saccoBranch).ToList();

            ViewBag.brances = new SelectList(brances.Select(b => b.Bname));

            var suppliers = _context.DSuppliers.Where(a => a.Scode == sacco).ToList();
            ViewBag.suppliers = new SelectList(suppliers);

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

            if (zones.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;

            //var checkifdelievedtoday = _context.ProductIntake.Where(L => L.SaccoCode == sacco
            //&& L.Branch == saccoBranch && L.TransDate == date).ToList();


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


            var employees = _context.Employees.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var staffs = new List<EmployeeDetVm>();
            employees.ForEach(e =>
            {
                staffs.Add(new EmployeeDetVm
                {
                    Details = e.Surname + " " + e.Othernames + "(" + e.EmpNo + ")",
                    EmpNo = e.EmpNo
                });
            });
            ViewBag.staffs = new SelectList(staffs, "EmpNo", "Details");
        }
        public IActionResult CreateCorrection()
        {
            utilities.SetUpPrivileges(this);
            SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var Todayskg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today).Sum(p => p.Qsupplied);
            var TodaysBranchkg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
            return View(new ProductIntakeVm
            {
                Todaykgs = Todayskg,
                TodayBranchkgs = TodaysBranchkg
            });
        }

        // POST: ProductIntakes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch,DrAccNo,CrAccNo,Print,SMS,Zone,MornEvening")] ProductIntakeVm productIntake)
        {
            return View(productIntake);
        }

        [HttpPost]
        public JsonResult Save([FromBody] ProductIntakeVm productIntake)
        {
            utilities.SetUpPrivileges(this);
            SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            productIntake.Branch = saccoBranch;
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.Description = productIntake?.Description ?? "";
            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json("");
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return Json("");
            }
            if (productIntake.Qsupplied < 0.01M)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return Json("");
            }
            var suppliers = _context.DSuppliers.Where(s => s.Sno == productIntake.Sno && s.Scode.ToUpper().Equals(sacco.ToUpper()));
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);
            }
            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return Json("");
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return Json("");
            }
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            if (ModelState.IsValid)
            {
                productIntake.SaccoCode = sacco;
                productIntake.TransactionType = TransactionType.Intake;
                var prices = 0;
                decimal totalamount = 0;
                var price = _context.DPrices
                   .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                   && p.Products.ToUpper().Equals(productIntake.ProductType.ToUpper()));
                var checkifdifferentprice = _context.d_Price2.FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && p.Branch == saccoBranch && p.Sno.ToUpper().Equals(productIntake.Sno.ToUpper()) && p.Active== true);

                if (checkifdifferentprice != null)
                    prices = (int)checkifdifferentprice.Price;
                else
                    prices = (int)price.Price;

                totalamount=(decimal)productIntake.Qsupplied* prices;

                var collection = new ProductIntake
                {
                    Sno = productIntake.Sno.Trim().ToUpper(),
                    TransDate = DateTime.Today,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productIntake.ProductType,
                    Qsupplied = (decimal)productIntake.Qsupplied,
                    Ppu = prices,
                    CR = totalamount,
                    DR = 0,
                    Balance = productIntake.Balance,
                    Description = "Intake",
                    TransactionType = productIntake.TransactionType,
                    Remarks = productIntake.Remarks,
                    AuditId = loggedInUser,
                    Auditdatetime = DateTime.Now,
                    Branch = productIntake.Branch,
                    SaccoCode = productIntake.SaccoCode,
                    DrAccNo = productIntake.DrAccNo,
                    CrAccNo = productIntake.CrAccNo,
                    Zone = productIntake.Zone,
                    MornEvening = productIntake.MornEvening
                };
                _context.ProductIntake.Add(collection);

                calcDefaultdeductions(collection);

                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == productIntake.Sno && t.Active
               && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
               && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()) && t.Branch == saccoBranch);

                if (!string.IsNullOrEmpty(productIntake.MornEvening))
                {
                    transport = _context.DTransports.FirstOrDefault(t => t.Sno.ToUpper().Equals(productIntake.Sno.ToUpper()) && t.Active
                && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
                && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()) && t.Branch == saccoBranch
                && t.Morning == productIntake.MornEvening);
                }

                if (transport != null)
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR = productIntake.Qsupplied * transport.Rate;
                    productIntake.Balance = productIntake.Balance - productIntake.DR;
                    collection = new ProductIntake
                    {
                        Sno = productIntake.Sno.Trim().ToUpper(),
                        TransDate = DateTime.Today,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = productIntake.ProductType,
                        Qsupplied = (decimal)productIntake.Qsupplied,
                        Ppu = transport.Rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = productIntake.Balance,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = productIntake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = productIntake.DrAccNo,
                        CrAccNo = productIntake.CrAccNo,
                        Zone = productIntake.Zone,
                        MornEvening = productIntake.MornEvening
                    };
                    _context.ProductIntake.Add(collection);

                    // Credit transpoter transport amount
                    ///CHECK IF TRANSPORTER IS PAID BY SOCIETY
                    var transporterscheck = _context.DTransporters.FirstOrDefault(h => h.ParentT.ToUpper().Equals(sacco.ToUpper())
                    && h.Tbranch.ToUpper().Equals(saccoBranch.ToUpper()));
                    if (transport.Rate == 0 && transporterscheck.Rate > 0)
                    {
                        productIntake.CR = productIntake.Qsupplied * (decimal)transporterscheck.Rate;
                    }
                    else
                    {
                        productIntake.CR = productIntake.Qsupplied * transport.Rate;
                    }

                    productIntake.DR = 0;
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = transport.TransCode.Trim().ToUpper(),
                        TransDate = DateTime.Today,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = productIntake.ProductType,
                        Qsupplied = (decimal)productIntake.Qsupplied,
                        Ppu = transport.Rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = productIntake.Balance,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = productIntake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = price.TransportDrAccNo,
                        CrAccNo = price.TransportCrAccNo,
                        Zone = productIntake.Zone,
                        MornEvening = productIntake.MornEvening
                    });
                }

                if (sacco == "EMUKA MORINGA FCS" || sacco == "USWET UMOJA DAIRIES FCS")
                {
                    if (productIntake.SMS)
                    {
                        if (supplier.PhoneNo != "0")
                        {
                            if (supplier.PhoneNo.Length > 8)
                            {

                                var intakes = _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                                         && s.TransDate >= startDate && s.TransDate <= endDate && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction));
                                if (user.AccessLevel == AccessLevel.Branch)
                                    intakes = intakes.Where(s => s.Branch == saccoBranch);
                                var commulated = intakes.Sum(s => s.Qsupplied);
                                var note = "";
                                if (productIntake.ProductType.ToLower().Equals("milk"))
                                {
                                    note = "Kindly observe withdrawal period after cow treatment";
                                    note = "";
                                }

                                var phone_first = supplier.PhoneNo.Substring(0, 1);
                                if (phone_first == "0")
                                    supplier.PhoneNo = supplier.PhoneNo.Substring(1);
                                var phone_three = supplier.PhoneNo.Substring(0, 3);
                                if (phone_three == "254")
                                    supplier.PhoneNo = supplier.PhoneNo.Substring(3);
                                var phone_four = supplier.PhoneNo.Substring(0, 4);
                                if (phone_four == "+254")
                                    supplier.PhoneNo = supplier.PhoneNo.Substring(4);

                                supplier.PhoneNo = "254" + supplier.PhoneNo;
                                var totalkgs = string.Format("{0:.0###}", commulated + productIntake.Qsupplied);
                                String[] GetFirstName = supplier.Names.Split(' ');
                                _context.Messages.Add(new Message
                                {
                                    Telephone = supplier.PhoneNo,

                                    Content = $"{DateTime.Now} Dear {GetFirstName[0].Trim()}, You have supplied {productIntake.Qsupplied} kgs to {sacco}. Total for {DateTime.Today.ToString("MMMM/yyyy")} is {totalkgs} kgs.\n {note}",
                                    ProcessTime = DateTime.Now.ToString(),
                                    MsgType = "Outbox",
                                    Replied = false,
                                    DateReceived = DateTime.Now,
                                    Source = loggedInUser,
                                    Code = sacco
                                });

                            }
                        }
                    }
                }


                //if (productIntake.Print)
                //    PrintP(collection);

                _context.SaveChanges();
                _notyf.Success("Intake saved successfully");

                //    var intakes = _context.ProductIntake
                //.FirstOrDefault(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                //&& i.Sno == collection.Sno && i.TransDate == collection.TransDate && i.TransTime == collection.TransTime);
                //    return RedirectToAction("createprinttest", new { id = intakes.Id });//"Details", "Event", new { id = thisEvent }

                //var Todayskg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today).Sum(p => p.Qsupplied);
                //var TodaysBranchkg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
            }

            var intake = new ProductIntake
            {
                Sno = productIntake.Sno,
                Qsupplied = (decimal)productIntake.Qsupplied,
                MornEvening = productIntake.MornEvening,
                SaccoCode = productIntake.SaccoCode,
                Branch = saccoBranch,
            };
            var receiptDetails = GetReceiptDetails(intake, loggedInUser);
            return Json(new
            {
                receiptDetails,
            });
        }

        [HttpGet]
        public JsonResult ReprintReceipt(long intakeId)
        {
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var intake = _context.ProductIntake.FirstOrDefault(i => i.Id == intakeId);
            var receiptDetails = GetReceiptDetails(intake, loggedInUser);
            return Json(new
            {
                receiptDetails,
            });
        }

        private dynamic GetReceiptDetails(ProductIntake intake, string loggedInUser)
        {
            intake.Sno = intake?.Sno ?? "";
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(intake.SaccoCode.ToUpper()));
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            // cummulative kgs calc
            var cumkg = _context.ProductIntake.Where(o => o.SaccoCode.ToUpper().Equals(intake.SaccoCode.ToUpper()) &&
            o.Sno == intake.Sno && o.Branch.ToUpper().Equals(intake.Branch.ToUpper()) &&
            o.TransDate >= startDate && o.TransDate <= endDate
            && (o.Description == "Intake" || o.Description == "Correction")).Sum(d => d.Qsupplied);

            string cummkgs = string.Format("{0:.###}", cumkg);

            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(intake.Sno) 
            && s.Scode == intake.SaccoCode && s.Branch == intake.Branch);
            return new
            {
                companies.Name,
                companies.Adress,
                companies.Town,
                companies.PhoneNo,
                saccoBranch = intake.Branch,
                intake.Sno,
                supName = supplier.Names,
                intake.Qsupplied,
                cummkgs,
                loggedInUser,
                MornEvening = intake.MornEvening ?? "Mornnig",
            };
        }

        //public IActionResult printtest(ProductIntake collection)
        public IActionResult createprinttest(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var collection = _context.ProductIntake.FirstOrDefault(u => u.Id == id);
            var company = _context.DCompanies.Where(u => u.Name == sacco);
            var supplier = _context.DSuppliers.FirstOrDefault(u => u.Scode == sacco && u.Sno.ToUpper().Equals(collection.Sno.ToUpper()));
            //intakes.ToList()



            ViewBag.company = company.Select(s => new DCompany
            {
                Name = s.Name,
                Adress = s.Adress,
                Town = s.Town,
                PhoneNo = s.PhoneNo,
                Email = s.Email,
                Website = s.Website
            }).ToList();

            return View(new ViewModels.SuppliersreceiptVM.Suppliersreceipt
            {
                TransDate = collection.TransDate,
                ProductType = collection.ProductType,
                Qsupplied = collection.Qsupplied,
            });
        }
        public IActionResult complainprinttest(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var collection = _context.ProductIntake.Where(u => u.Id == id);
            return View(id);
        }
        [HttpPost]
        public JsonResult getdetatils(long? id)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var collection = _context.ProductIntake.Where(u => u.Id == id);

            return Json(collection);
        }
        public IActionResult printtest(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var collection = _context.ProductIntake.Where(u => u.Id == id);
            //intakes.ToList()
            return View(collection.ToList());
        }

        [HttpPost]
        public JsonResult getsuppliersup([FromBody] DSupplier supplier, string filter, string condition, DateTime date)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var companyTanykina = HttpContext.Session.GetString(StrValues.Tanykina);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();

            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(i => i.Branch == saccobranch).ToList();

            var suplliersdeductions = _context.ProductIntake.Where(d => d.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && d.TransDate >= startDate && d.TransDate <= enDate && suppliers.Select(s => s.Sno.ToUpper()).Contains(d.Sno.ToUpper())
            && d.TransactionType == TransactionType.Deduction && d.Qsupplied == 0).ToList();

            var MilkBranchList = new List<ProductIntakeVm>();
            suppliers = suppliers.Where(d => suplliersdeductions.Select(s => s.Sno.ToUpper()).Contains(d.Sno.ToUpper())).ToList();
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(condition))
            {

                if (condition == "SNo")
                {
                    suppliers = suppliers.Where(i => i.Sno.ToUpper().Contains(filter.ToUpper())).ToList();
                }
                if (condition == "Name")
                {
                    suppliers = suppliers.Where(i => i.Names.ToUpper().Contains(filter.ToUpper())).ToList();
                }

            }
            else
            {
                suplliersdeductions = suplliersdeductions.Where(d => d.TransDate == date).ToList();
            }

            var grouptransporters = suppliers.GroupBy(m => m.Sno).ToList();
            grouptransporters.ForEach(k =>
            {
                var transdetails = k.FirstOrDefault();
                if (suppliers.Count > 0)
                {
                    var transdeductionsget = suplliersdeductions.Where(d => d.Sno.ToUpper().Equals(transdetails.Sno.ToUpper())).OrderBy(m => m.TransDate).ToList();

                    if (transdeductionsget.Count > 0)
                    {
                        foreach (var items in transdeductionsget)
                        {
                            MilkBranchList.Add(new ProductIntakeVm
                            {
                                Sno = transdetails.Sno,
                                SupName = transdetails.Names,
                                TransDate = items.TransDate,
                                ProductType = items.ProductType,
                                DR = items.DR,
                                Remarks = items.Remarks,
                                Branch = items.Branch
                            });
                        }
                    }
                }

            });


            MilkBranchList = MilkBranchList.OrderByDescending(i => i.Sno).Take(15).ToList();
            return Json(MilkBranchList);
        }


        [HttpPost]
        public JsonResult getsuppliers([FromBody] DTransporter transporter, string filter, string condition, DateTime date)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var companyTanykina = HttpContext.Session.GetString(StrValues.Tanykina);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var transporters = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();

            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(i => i.Tbranch == saccobranch).ToList();

            var transdeductions = _context.ProductIntake.Where(d => d.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && d.TransDate >= startDate && d.TransDate <= enDate && transporters.Select(s => s.TransCode.ToUpper()).Contains(d.Sno.ToUpper())
            && d.TransactionType == TransactionType.Deduction && d.Qsupplied == 0).ToList();

            var MilkBranchList = new List<ProductIntakeVm>();
            transporters = transporters.Where(d => transdeductions.Select(s => s.Sno.ToUpper()).Contains(d.TransCode.ToUpper())).ToList();
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(condition))
            {

                if (condition == "TCode")
                {
                    transporters = transporters.Where(i => i.TransCode.ToUpper().Contains(filter.ToUpper())).ToList();
                }
                if (condition == "Name")
                {
                    transporters = transporters.Where(i => i.TransName.ToUpper().Contains(filter.ToUpper())).ToList();
                }

            }
            else
            {
                transdeductions = transdeductions.Where(d => d.TransDate == date).ToList();
            }

            var grouptransporters = transporters.GroupBy(m => m.TransCode).ToList();
            grouptransporters.ForEach(k =>
            {
                var transdetails = k.FirstOrDefault();
                if (transporters.Count > 0)
                {
                    var transdeductionsget = transdeductions.Where(d => d.Sno.ToUpper().Equals(transdetails.TransCode.ToUpper())).OrderBy(m => m.TransDate).ToList();

                    if (transdeductionsget.Count > 0)
                    {
                        foreach (var items in transdeductionsget)
                        {
                            MilkBranchList.Add(new ProductIntakeVm
                            {
                                Sno = transdetails.TransCode,
                                SupName = transdetails.TransName,
                                TransDate = items.TransDate,
                                ProductType = items.ProductType,
                                DR = items.DR,
                                Remarks = items.Remarks,
                                Branch = items.Branch
                            });
                        }
                    }
                }

            });


            MilkBranchList = MilkBranchList.OrderByDescending(i => i.Sno).Take(15).ToList();
            return Json(MilkBranchList);
        }

        public void calcDefaultdeductions(ProductIntake collection)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            DateTime sDate = new DateTime(collection.TransDate.Year, collection.TransDate.Month, 1);
            DateTime enDate = sDate.AddMonths(1).AddDays(-1);

            var Checkanydefaultdeduction = _context.d_PreSets.FirstOrDefault(l => l.Sno.ToUpper().Equals(collection.Sno.ToUpper()) && !l.Stopped
            && l.saccocode.ToUpper().Equals(sacco.ToUpper()));
            if (Checkanydefaultdeduction != null)
            {
                var totalkgs = _context.ProductIntake.Where(f => f.Sno.ToUpper().Equals(collection.Sno.ToUpper())
                && f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= sDate && f.TransDate <= enDate
                && f.TransactionType == TransactionType.Intake).Sum(n => n.Qsupplied);
                var bonus = Checkanydefaultdeduction.Rate;
                if (Checkanydefaultdeduction.Rated == true)
                {
                    totalkgs = (decimal)(totalkgs + collection.Qsupplied);
                    bonus = totalkgs * Checkanydefaultdeduction.Rate;
                }
                var checkintake = _context.ProductIntake
                .Where(f => f.Sno.ToUpper().Equals(collection.Sno.ToUpper())
                && f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= sDate
                && f.TransDate <= enDate && f.ProductType.ToUpper().Equals(Checkanydefaultdeduction.Deduction.ToUpper()));
                if (checkintake.Any())
                {
                    _context.ProductIntake.RemoveRange(checkintake);
                    _context.SaveChanges();
                }

                var checkgls = _context.Gltransactions
                .Where(f => f.Source.ToUpper().Equals(collection.Sno.ToUpper())
                && f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= sDate
                && f.TransDate <= enDate && f.TransDescript.ToUpper().Equals("BONUS".ToUpper()));
                if (checkgls.Any())
                {
                    _context.Gltransactions.RemoveRange(checkgls);
                    _context.SaveChanges();
                }

                collection = new ProductIntake
                {
                    Sno = collection.Sno.Trim().ToUpper(),
                    TransDate = enDate,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = Checkanydefaultdeduction.Deduction,
                    Qsupplied = 0,
                    Ppu = 0,
                    CR = 0,
                    DR = bonus,
                    Balance = 0,
                    Description = Checkanydefaultdeduction.Remark,
                    TransactionType = TransactionType.Deduction,
                    Remarks = Checkanydefaultdeduction.Remark,
                    AuditId = loggedInUser,
                    Auditdatetime = collection.Auditdatetime,
                    Branch = collection.Branch,
                    SaccoCode = collection.SaccoCode,
                    DrAccNo = "0",
                    CrAccNo = "0",
                    Zone = collection.Zone
                };
                _context.ProductIntake.Add(collection);

                var glsforbonus = _context.DDcodes.FirstOrDefault(m => m.Description.ToUpper().Equals(Checkanydefaultdeduction.Deduction.ToUpper()));

                _context.Gltransactions.Add(new Gltransaction
                {
                    AuditId = loggedInUser,
                    TransDate = enDate,
                    Amount = (decimal)bonus,
                    AuditTime = DateTime.Now,
                    Source = collection.Sno.Trim().ToUpper(),
                    TransDescript = "Bonus",
                    Transactionno = $"{loggedInUser}{DateTime.Now}",
                    SaccoCode = sacco,
                    DrAccNo = glsforbonus.Dedaccno,
                    CrAccNo = glsforbonus.Contraacc,
                });

            }
        }
        //start

        //private IActionResult PrintStatement(ProductIntakeVm collection)
        //{
        //    PrintDocument printDocument = new PrintDocument();
        //    printDocument.PrintPage += (sender, args) => printStatementDocument_PrintPage(collection, args);
        //    printDocument.Print();
        //    return Ok(200);
        //}
        //private void printStatementDocument_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        //    var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        //    var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        //    var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(sacco.ToUpper()));

        //    ProductIntakeVm items = sender as ProductIntakeVm;
        //    if (items != null)
        //    {
        //        // Start printing your items.
        //        DateTime endmonth = (DateTime)items.TransDate;
        //        DateTime startDate = new DateTime(endmonth.Year, endmonth.Month, 1);
        //        DateTime enDate = startDate.AddMonths(1).AddDays(-1);

        //        // cummulative kgs calc
        //        var productIntakes  = _context.ProductIntake.Where(o => o.SaccoCode.ToUpper().Equals(sacco.ToUpper()) &&
        //        o.Sno.ToUpper().Equals(items.Sno.ToUpper()) && o.Branch.ToUpper().Equals(items.Branch.ToUpper()) &&
        //        o.TransDate >= startDate && o.TransDate <= enDate ).ToList();

        //        var IntakesOnly = productIntakes.Where(o => (o.Description == "Intake" || o.Description == "Correction")).OrderBy(o=>o.TransDate);
        //        var DeductionsOnly = productIntakes.Where(o => o.Description != "Intake" && o.Description != "Correction").ToList();
        //        var TotalMonthKgs = string.Format("{0:.###}", IntakesOnly.Sum(l=>l.Qsupplied));
        //        var Gross = string.Format("{0:.###}", IntakesOnly.Sum(l => l.CR)- IntakesOnly.Sum(l => l.DR));
        //        var TotalDeductions = string.Format("{0:.###}", DeductionsOnly.Sum(l => l.DR)- DeductionsOnly.Sum(l => l.CR));

        //        var supplier = _context.DSuppliers.FirstOrDefault(u => u.Scode.ToUpper().Equals(sacco.ToUpper()) &&
        //        u.Sno.ToUpper().Equals(items.Sno.ToUpper()) && u.Branch.ToUpper().Equals(saccoBranch.ToUpper()));


        //        var transport = _context.DTransports.FirstOrDefault(u => u.saccocode.ToUpper().Equals(sacco.ToUpper()) &&
        //        u.Sno.ToUpper().Equals(items.Sno.ToUpper()) && u.Active);

        //        string transporter = "SELF";
        //        if (transport != null)
        //        {
        //            transporter = _context.DTransporters.FirstOrDefault(u => u.ParentT.ToUpper().Equals(sacco.ToUpper()) &&
        //        u.TransCode.Trim().ToUpper().Equals(transport.TransCode.Trim().ToUpper()) && u.Active).TransName.ToString();
        //        }


        //        Graphics graphics = e.Graphics;
        //        Font font = new Font("Times New Roman", 12);
        //        float fontHeight = font.GetHeight();

        //        int startX = 10;
        //        int startY = -40;
        //        int offset = 40;


        //        graphics.DrawString(companies.Name, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString(companies.Adress.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString(companies.Town.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString("Tell: " + companies.PhoneNo.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString("Branch: " + saccoBranch, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string line = "---------------------------------------------";
        //        graphics.DrawString(line, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        var datet = enDate.ToString("dd/MM/yyy");
        //        graphics.DrawString("Supplier Statement For: " + datet.PadRight(15), new Font("Times New Roman", 14), new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString("Transporter: ".PadLeft(10) + transporter.PadRight(10), font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string sno = items.Sno.PadRight(10);
        //        graphics.DrawString("SNo: " + sno, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string name = supplier.Names.ToString();
        //        graphics.DrawString("Name: " + name, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string HDate = "Date".PadLeft(3);
        //        string HPrice = "Price".PadLeft(16);
        //        string HQnty = "Qnty  Amount".PadLeft(17);
        //        string Hsession = "Session".PadLeft(18);
        //        string Heading = HDate + HPrice+ HQnty + Hsession;
        //        graphics.DrawString(Heading, new Font("Times New Roman", 9), new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;
        //        string session = "MORNING";

        //        var intakes = IntakesOnly.GroupBy(i => i.TransDate).ToList();
        //        intakes.ForEach(i => {
        //            var sessionGroups = i.GroupBy(t => t.MornEvening).ToList();
        //            sessionGroups.ForEach(t => {
        //                    var intake = t.FirstOrDefault();
        //                    string BDate = intake.TransDate.ToString("dd/MM/yyy").PadLeft(0);
        //                    string BPrice = string.Format("{0:.###}", intake.Ppu).PadLeft(8);
        //                    string BQnty = string.Format("{0:.###}", t.Sum(k=>k.Qsupplied)).PadLeft(12);
        //                    string Amt = string.Format("{0:.###}", (t.Sum(k => k.Qsupplied)* intake.Ppu)).PadLeft(14);
        //                    if (items.MornEvening != null)
        //                        session = intake.MornEvening.ToUpper().ToString();
        //                    string Bsession = session.PadLeft(16);
        //                    string Body = BDate + BPrice + BQnty + Amt + Bsession;

        //                    graphics.DrawString(Body, new Font("Times New Roman", 9), new SolidBrush(Color.Black), startX, startY + offset);
        //                    offset = offset + (int)fontHeight + 5;
        //            });
        //        });


        //        string bline = "---------------------------------------------";
        //        graphics.DrawString(bline, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString("Total Kgs: " + TotalMonthKgs, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;
        //        graphics.DrawString("Gross Pay Kshs: " + Gross, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString(bline, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString("DEDUCTIONS: ".PadLeft(10), new Font("Times New Roman", 14), new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString(bline, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string DDate = "Date".PadLeft(5);
        //        string DPrice = "Amount".PadLeft(16);
        //        string DQnty = "Description".PadLeft(17);
        //        string Deduction = DDate + DPrice + DQnty ;
        //        graphics.DrawString(Deduction, new Font("Times New Roman", 10), new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;
        //        var deduction = DeductionsOnly.GroupBy(s=>s.ProductType).ToList();
        //        deduction.ForEach(i => {
        //            var deductionGroups = i.GroupBy(t => t.TransDate).ToList();
        //            deductionGroups.ForEach(t => {
        //                var deductionGroupsdetails = t.FirstOrDefault();
        //                string DBDate = deductionGroupsdetails.TransDate.ToString("dd/MM/yyy").PadLeft(0);

        //                string DeductionType = (deductionGroupsdetails.ProductType.ToString()).PadLeft(15);
        //                if (DeductionType == "MILK")
        //                    DeductionType = "Transport";
        //                string DDescription = (deductionGroupsdetails.Description.ToString());
        //                string FinalDDescription = DeductionType +" " + DDescription;

        //                var DR = t.Sum(t => t.DR);
        //                var CR = t.Sum(t => t.CR);
        //                var CombineAmount = DR-CR;
        //                if (CombineAmount != 0)
        //                {
        //                    string DAmount = string.Format("{0:.###}", CombineAmount).PadLeft(10);
        //                    string DBody = DBDate + DAmount + FinalDDescription;
        //                    graphics.DrawString(DBody, new Font("Times New Roman", 10), new SolidBrush(Color.Black), startX, startY + offset);
        //                    offset = offset + (int)fontHeight + 5;
        //                }
        //            });
        //        });

        //        graphics.DrawString("Total Deductions Kshs: " + TotalDeductions, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string line1 = "---------------------------------------------";
        //        graphics.DrawString(line1, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        decimal NetPay = Convert.ToDecimal(Gross)- Convert.ToDecimal(TotalDeductions);

        //        graphics.DrawString("Net Pay Kshs: " + NetPay, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        graphics.DrawString("Bank Name: "+ supplier.Bcode, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;
        //        graphics.DrawString("Bank AccNo: " + supplier.AccNo, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;
        //        graphics.DrawString("Bank Branch: " + supplier.Bbranch, font, new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string line7 = "---------------------------------------------";
        //        graphics.DrawString(line7, font, new SolidBrush(Color.Black), startX, startY + offset);

        //        offset = offset + (int)fontHeight + 5;
        //        string dev = "DEVELOP BY: AMTECH TECHNOLOGIES LIMITED";
        //        graphics.DrawString(dev.PadRight(13), new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);

        //        offset = offset + (int)fontHeight + 5;
        //        startY = startY + 20;
        //        string dev1 = " ";
        //        graphics.DrawString(dev1.PadRight(13), new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
        //        offset = offset + (int)fontHeight + 5;

        //        string line16 = "---------------------------------------------";
        //        graphics.DrawString(line16, font, new SolidBrush(Color.Black), startX, startY + offset);


        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch,Zone")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            DateTime startdate = new DateTime(productIntake.TransDate.Year, productIntake.TransDate.Month, 1);
            DateTime enddate = startdate.AddMonths(1).AddDays(-1);
            productIntake.Description = productIntake?.Description ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var suppliers = _context.DSuppliers.Where(s => s.Sno == productIntake.Sno && s.Scode == sacco && s.Active && s.Approval);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            var intakes = _context.ProductIntake.Where(i => i.Sno == productIntake.Sno && i.SaccoCode == sacco
            && i.Qsupplied != 0 && i.TransDate >= startdate && i.TransDate <= enddate);

            if (user.AccessLevel == AccessLevel.Branch)
            {
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);
                intakes = intakes.Where(i => i.Branch == saccoBranch);
            }


            if (!suppliers.Any())
            {
                _notyf.Error("Sorry, Supplier Number code does not exist");
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = suppliers,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (!intakes.Any())
            {
                _notyf.Error("Sorry, Supplier has not deliver any product for this month" + " " + startdate + "To " + " " + enddate);
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = suppliers,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                GetInitialValues();
                _notyf.Error("Sorry, Farmer code cannot be zero");
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = suppliers,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (ModelState.IsValid)
            {
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser);
                productIntake.AuditId = auditId ?? "";
                productIntake.TransactionType = TransactionType.Deduction;
                productIntake.TransDate = productIntake.TransDate;
                productIntake.SaccoCode = sacco;
                productIntake.Qsupplied = 0;
                if (productIntake.DR > 0)
                {
                    productIntake.DR = productIntake.DR;
                    productIntake.CR = 0;
                }
                else
                {
                    productIntake.CR = (productIntake.DR*-1);
                    productIntake.DR = 0;
                }
                string re = productIntake.Remarks;
                if (string.IsNullOrEmpty(productIntake.Remarks))
                    re = productIntake.ProductType;
                productIntake.Description = re;
                productIntake.Remarks = re;
                productIntake.Balance = utilities.GetBalance(productIntake);
                productIntake.Zone = productIntake.Zone;
                _context.Add(productIntake);
                _notyf.Success("Deducted successfully");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DeductionList));
            }
            return View(productIntake);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            //long.TryParse(, out long sno);
            productIntake.Remarks = productIntake?.Remarks ?? "";
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco && t.Tbranch == saccoBranch);
            if (!_context.DTransporters.Any(i => i.TransCode == productIntake.Sno && i.Active == true && i.ParentT == sacco && i.Tbranch == saccoBranch))
            {
                _notyf.Error("Sorry, Transporter code does not exist");
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DTransporters = transporters,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (productIntake.DR == 0)
            {
                GetInitialValues();
                _notyf.Error("Sorry, Amount cannot be zero");
                Farmersobj = new FarmersVM()
                {
                    DTransporters = transporters,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (productIntake.Sno == "0")
            {
                GetInitialValues();
                _notyf.Error("Sorry, Transporter code cannot be zero");
                Farmersobj = new FarmersVM()
                {
                    DTransporters = transporters,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (ModelState.IsValid)
            {
                //productIntake.TransactionType = TransactionType.Deduction;
                //productIntake.TransDate = DateTime.Today;
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser);
                productIntake.AuditId = auditId ?? "";
                productIntake.Qsupplied = 0;
                productIntake.CR = 0;
                string re = productIntake.Remarks;
                if (string.IsNullOrEmpty(productIntake.Remarks))
                    re = productIntake.ProductType;
                productIntake.Description = re;
                productIntake.Remarks = re;
                productIntake.TransactionType = TransactionType.Deduction;
                productIntake.SaccoCode = sacco;
                productIntake.Balance = utilities.GetBalance(productIntake);
                productIntake.Branch = productIntake.Branch;
                _context.Add(productIntake);
                _notyf.Success("Deducted successfully");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TDeductionList));
            }
            return View(productIntake);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaffDeduction([Bind("Id,Empno, Date, Deduction, Amount, Remarks, AuditId, saccocode")] EmployeesDed employeesDed)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            //long.TryParse(, out long sno);
            employeesDed.Remarks = employeesDed?.Remarks ?? "";
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco && t.Tbranch == saccoBranch);
            if (!_context.Employees.Any(i => i.EmpNo == employeesDed.Empno && i.SaccoCode == sacco))
            {
                _notyf.Error("Sorry, Staff code does not exist");
                GetInitialValues();
                //return Json(new { data = Farmersobj });
                return View();
            }
            if (employeesDed.Amount == 0)
            {
                GetInitialValues();
                _notyf.Error("Sorry, Amount cannot be zero");
                //return Json(new { data = Farmersobj });
                return View();
            }
            if (employeesDed.Empno == "")
            {
                GetInitialValues();
                _notyf.Error("Sorry, Staff Name cannot be zero");
                //return Json(new { data = Farmersobj });
                return View();
            }
            if (ModelState.IsValid)
            {
                //productIntake.TransactionType = TransactionType.Deduction;
                //productIntake.TransDate = DateTime.Today;
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser);
                employeesDed.AuditId = auditId ?? "";
                employeesDed.saccocode = sacco;
                employeesDed.Empno = employeesDed.Empno;
                _context.Add(employeesDed);
                _notyf.Success("Deducted successfully");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(StaffDeductionList));
            }
            return View(employeesDed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCorrection([Bind("Id, Sno, TransDate, ProductType, Qsupplied, Ppu, CR, DR, Balance, Description, Remarks, AuditId, Auditdatetime, Branch, DrAccNo, CrAccNo, Print, SMS,Zone,MornEvening")] ProductIntakeVm productIntake)
        {
            return View(productIntake);
        }

        [HttpPost]
        public JsonResult SaveCorrection([FromBody] ProductIntakeVm productIntake)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            productIntake.SaccoCode = sacco;
            //var branch = _context.DBranch.FirstOrDefault(b => b.Bcode.ToUpper().Equals(sacco.ToUpper()));
            productIntake.Branch = saccoBranch;
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.Description = productIntake?.Description ?? "";
            productIntake.DR = 0;

            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json("");
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return Json("");
            }
            if (productIntake.Qsupplied == 0)
            {
                //_notyf.Error("Sorry, Kindly provide quantity");
                return Json("");
            }
            var suppliers = _context.DSuppliers.Where(s => s.Sno == productIntake.Sno && s.Scode == sacco);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);


            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return Json("");
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return Json("");
            }
           

            if (ModelState.IsValid)
            {
                productIntake.AuditId = loggedInUser ?? "";
                productIntake.Description = "Correction";
                productIntake.TransactionType = TransactionType.Correction;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Auditdatetime = DateTime.Now;
                //productIntake.Balance = utilities.GetBalance(productIntake);
                //_context.Add(productIntake);

                var prices = 0;
                decimal totalamount = 0;

                var price = _context.DPrices
                    .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                    && p.Products.ToUpper().Equals(productIntake.ProductType.ToUpper()));

                var checkifdifferentprice = _context.d_Price2.FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && p.Branch == saccoBranch && p.Sno.ToUpper().Equals(productIntake.Sno.ToUpper()) && p.Active == true);

                if (checkifdifferentprice != null)
                    prices = (int)checkifdifferentprice.Price;
                else
                    prices = (int)price.Price;

                totalamount = (decimal)productIntake.Qsupplied * prices;
                double ch = 0;
                if (productIntake.CR < 0)
                {
                    productIntake.DR = (productIntake.CR) * -1;
                    ch = (double)productIntake.CR;
                    productIntake.CR = 0;
                    totalamount = 0;
                }
                if (productIntake.DrAccNo == null)
                {
                    productIntake.DrAccNo = "0";
                }
                if (productIntake.CrAccNo == null)
                {
                    productIntake.CrAccNo = "0";
                }

                productIntake.TransDate = productIntake.TransDate;
                var collection = new ProductIntake
                {
                    Sno = productIntake.Sno.Trim(),
                    TransDate = (DateTime)productIntake.TransDate,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productIntake.ProductType,
                    Qsupplied = (decimal)productIntake.Qsupplied,
                    Ppu = prices,
                    CR = totalamount,
                    DR = productIntake.DR,
                    Balance = productIntake.Balance,
                    Description = productIntake.Description,
                    TransactionType = productIntake.TransactionType,
                    Remarks = productIntake.Remarks,
                    AuditId = loggedInUser,
                    Auditdatetime = productIntake.Auditdatetime,
                    Branch = productIntake.Branch,
                    SaccoCode = productIntake.SaccoCode,
                    DrAccNo = productIntake.DrAccNo,
                    CrAccNo = productIntake.CrAccNo,
                    Zone = productIntake.Zone,
                    MornEvening = productIntake.MornEvening
                };
                _context.ProductIntake.Add(collection);

                calcDefaultdeductions(collection);

                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == productIntake.Sno && t.Active
               && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
               && t.saccocode.ToUpper().Equals(sacco.ToUpper()));

                if (!string.IsNullOrEmpty(productIntake.MornEvening))
                {
                    transport = _context.DTransports.FirstOrDefault(t => t.Sno == productIntake.Sno && t.Active
                && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
                && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()) && t.Branch == saccoBranch
                && t.Morning == productIntake.MornEvening);
                }


                if (transport != null)
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR = productIntake.Qsupplied * transport.Rate;


                    if (ch < 0)
                    {
                        productIntake.CR = (decimal?)((productIntake.Qsupplied * transport.Rate) * -1);
                        productIntake.DR = 0;
                    }

                    productIntake.Balance = productIntake.Balance + productIntake.CR;
                    collection = new ProductIntake
                    {
                        Sno = productIntake.Sno.Trim(),
                        TransDate = (DateTime)productIntake.TransDate,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = productIntake.ProductType,
                        Qsupplied = (decimal)productIntake.Qsupplied,
                        Ppu = transport.Rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = productIntake.Balance,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = productIntake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = productIntake.DrAccNo,
                        CrAccNo = productIntake.CrAccNo,
                        Zone = productIntake.Zone,
                        MornEvening = productIntake.MornEvening
                    };
                    _context.ProductIntake.Add(collection);

                    // Credit transpoter transport amount
                    ///CHECK IF TRANSPORTER IS PAID BY SOCIETY
                    var transporterscheck = _context.DTransporters.FirstOrDefault(h => h.ParentT.ToUpper().Equals(sacco.ToUpper())
                    && h.Tbranch.ToUpper().Equals(saccoBranch.ToUpper()));
                    if (transport.Rate == 0 && transporterscheck.Rate > 0)
                    {
                        productIntake.CR = productIntake.Qsupplied * (decimal)transporterscheck.Rate;
                    }
                    else
                    {
                        productIntake.CR = productIntake.Qsupplied * transport.Rate;
                    }
                    productIntake.DR = 0;
                    if (ch < 0)
                    {
                        productIntake.DR = (decimal?)((productIntake.CR) * -1);
                        productIntake.CR = 0;
                    }
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = transport.TransCode.Trim(),
                        TransDate = (DateTime)productIntake.TransDate,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = productIntake.ProductType,
                        Qsupplied = (decimal)productIntake.Qsupplied,
                        Ppu = transport.Rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = productIntake.Balance,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = productIntake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = price.TransportDrAccNo,
                        CrAccNo = price.TransportCrAccNo,
                        Zone = productIntake.Zone,
                        MornEvening = productIntake.MornEvening
                    });
                }
                //decimal? amount = 0;
                //if (productIntake.Qsupplied > 1)
                //{
                //     amount = productIntake.CR > 0 ? productIntake.CR : productIntake.CR;
                //}
                //else
                //{
                //    amount = productIntake.DR > 0 ? productIntake.DR : productIntake.DR;
                //    amount = amount * -1;

                //}
                //_context.Gltransactions.Add(new Gltransaction
                //{
                //    AuditId = auditId,
                //    TransDate = DateTime.Today,
                //    Amount = (decimal)amount,
                //    AuditTime = DateTime.Now,
                //    Source = productIntake.Sno,
                //    TransDescript = "Correction",
                //    Transactionno = $"{auditId}{DateTime.Now}",
                //    SaccoCode = sacco,
                //    DrAccNo = productIntake.DrAccNo,
                //    CrAccNo = productIntake.CrAccNo,
                //});

                if (sacco == "EMUKA MORINGA FCS" || sacco == "USWET UMOJA DAIRIES FCS")
                {
                    if (productIntake.SMS)
                    {
                        if (supplier.PhoneNo != "0")
                        {
                            if (supplier.PhoneNo.Length > 8)
                            {

                                var startDate1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                                var endDate1 = startDate1.AddMonths(1).AddDays(-1);

                                var intakes = _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                                && s.TransDate >= startDate1 && s.TransDate <= endDate1 && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction));
                                if (user.AccessLevel == AccessLevel.Branch)
                                    intakes = intakes.Where(i => i.Branch == saccoBranch);

                                var commulated = intakes.Sum(s => s.Qsupplied);

                                var phone_first = supplier.PhoneNo.Substring(0, 1);
                                if (phone_first == "0")
                                    supplier.PhoneNo = supplier.PhoneNo.Substring(1);
                                var phone_three = supplier.PhoneNo.Substring(0, 3);
                                if (phone_three == "254")
                                    supplier.PhoneNo = supplier.PhoneNo.Substring(3);
                                var phone_four = supplier.PhoneNo.Substring(0, 4);
                                if (phone_four == "+254")
                                    supplier.PhoneNo = supplier.PhoneNo.Substring(4);

                                supplier.PhoneNo = "254" + supplier.PhoneNo;

                                var totalkgs = string.Format("{0:.0###}", commulated + productIntake.Qsupplied);
                                String[] GetFirstName = supplier.Names.Split(' ');
                                _context.Messages.Add(new Message
                                {
                                    Telephone = supplier.PhoneNo,
                                    Content = $"{DateTime.Now} Dear {GetFirstName[0].Trim()}, Your have supplied {productIntake.Qsupplied} kgs to {sacco}. Total for {DateTime.Today.ToString("MMMM/yyyy")} is {totalkgs} kgs.",
                                    ProcessTime = DateTime.Now.ToString(),
                                    MsgType = "Outbox",
                                    Replied = false,
                                    DateReceived = DateTime.Now,
                                    Source = loggedInUser,
                                    Code = sacco
                                });
                            }
                        }
                    }
                }


                //if (productIntake.Print)
                //    PrintP(collection);

                _context.SaveChanges();
                _notyf.Success("Correction saved successfully");
                SetIntakeInitialValues();
                //return RedirectToAction(nameof(CreateCorrection));
            }

            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(sacco.ToUpper()));
            // cummulative kgs calc
            var cumkg = _context.ProductIntake.Where(o => o.SaccoCode.ToUpper().Equals(sacco.ToUpper()) &&
            o.Sno == productIntake.Sno && o.Branch.ToUpper().Equals(productIntake.Branch.ToUpper()) &&
            o.TransDate >= startDate && o.TransDate <= endDate
            && (o.Description == "Intake" || o.Description == "Correction")).Sum(d => d.Qsupplied);

            string cummkgs = string.Format("{0:.###}", cumkg);
            var receiptDetails = new
            {
                companies.Name,
                companies.Adress,
                companies.Town,
                companies.PhoneNo,
                companies.Motto,
                saccoBranch,
                productIntake.Sno,
                productIntake.SupName,
                productIntake.Qsupplied,
                cummkgs,
                loggedInUser,
                MornEvening = productIntake.MornEvening ?? "Mornnig",
                date = productIntake.TransDate
            };
            return Json(new
            {
                receiptDetails,
            });

        }

        public IActionResult Reprint(long? id)
        {
            var collection = _context.ProductIntake.FirstOrDefault(u => u.Id == id);
            //printtest(collection);
            return RedirectToAction(nameof(Index));
        }
        // GET: ProductIntakes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productIntake = await _context.ProductIntake.FindAsync(id);
            if (productIntake == null)
            {
                return NotFound();
            }
            return View(productIntake);
        }

        // POST: ProductIntakes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            productIntake.Description = productIntake?.Description ?? "";
            if (id != productIntake.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productIntake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductIntakeExists(productIntake.Id))
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
            return View(productIntake);
        }

        // GET: ProductIntakes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var productIntake = await _context.ProductIntake
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productIntake == null)
            {
                return NotFound();
            }

            return View(productIntake);
        }

        // POST: ProductIntakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var productIntake = await _context.ProductIntake.FindAsync(id);
            _context.ProductIntake.Remove(productIntake);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductIntakeExists(long id)
        {
            return _context.ProductIntake.Any(e => e.Id == id);
        }
    }

    internal class ReceiptPrinter
    {
    }
}
