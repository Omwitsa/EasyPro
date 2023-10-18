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
using Syncfusion.EJ2.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using EasyPro.Models.BosaModels;
using DocumentFormat.OpenXml.Drawing;

namespace EasyPro.Controllers
{
    public class ProductIntakesController : Controller
    {
        PrintDocument pdoc = null;
        private readonly MORINGAContext _context;
        private readonly BosaDbContext _bosaDbContext;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        private static object clientSock;

        public FarmersVM Farmersobj { get; private set; }


        public ProductIntakesController(MORINGAContext context, INotyfService notyf, BosaDbContext bosaDbContext)
        {
            _context = context;
            _notyf = notyf;
            _bosaDbContext = bosaDbContext;
            utilities = new Utilities(context);
        }
        // GET: ProductIntakes
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var productIntakeslist = _context.ProductIntake;
            var intakes = await _context.ProductIntake.Where(i => i.TransactionType == TransactionType.Intake
            && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.TransDate == DateTime.Today).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch).ToList();

            ViewBag.remarks = StrValues.Slopes == sacco ? "Invoice No." : "Remarks";
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
                && i.TransDate == date).OrderByDescending(l => l.Auditdatetime).ToList();

            var TodaysBranchkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) 
            && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date && s.Branch == saccoBranch).Sum(p => p.Qsupplied);

            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch).ToList();
            //intakes=intakes.OrderByDescending(l => l.Auditdatetime).ToList();
            ViewBag.intakes = TodaysBranchkg;
            return Json(new {
                intakes,
                TodaysBranchkg
            });
        }
        [HttpGet]
        public JsonResult listIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var intakes = _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && i.TransDate == date).OrderByDescending(l => l.Auditdatetime).ToList();
            //NewMethod(date);
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch).ToList();
            //intakes=intakes.OrderByDescending(l => l.Auditdatetime).ToList();
            //return Json(intakes.OrderByDescending(l => l.Auditdatetime).ToList());

            var TodaysBranchkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
            ViewBag.intakes = TodaysBranchkg;

            return Json(new
            {
                intakes,
                TodaysBranchkg
            });
        }

        public async Task<IActionResult> TDeductionList()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            
            var intakes = await _context.ProductIntake
                .Where(c => c.TransactionType == TransactionType.Correction && c.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && c.TransDate == DateTime.Today).ToListAsync();

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch).ToList();

            ViewBag.remarks = StrValues.Slopes == sacco ? "Invoice No." : "Remarks";
            return View(intakes.OrderByDescending(l => l.Auditdatetime).ToList());
        }
        public async Task<IActionResult> StaffDeductionList()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
        public async Task<IActionResult> Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            var intakes = await productIntakes.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction) && s.TransDate == DateTime.Today).ToListAsync();
            //var intakes = productIntakes.Where(s => s.TransDate == DateTime.Today);
            var Todayskg = intakes.Sum(p => p.Qsupplied);
            var TodaysBranchkg = intakes.Where(s => s.Branch == saccoBranch).Sum(p => p.Qsupplied);

            var remarks = "Remarks";
            var remarksValue = "";
            if (StrValues.Slopes == sacco)
            {
                var recentIntake = productIntakes.Where(s => s.TransactionType == TransactionType.Intake).OrderByDescending(i => i.Id).FirstOrDefault();
                long.TryParse(recentIntake?.Remarks ?? "", out long invoiceNo);
                invoiceNo++;
                remarks = "Invoice No.";
                remarksValue = "" + invoiceNo;
            }

            ViewBag.remarks = remarks;
            ViewBag.slopes = StrValues.Slopes == sacco;
            ViewBag.remarksValue = remarksValue;

            return View(new ProductIntakeVm
            {
                Todaykgs = Todayskg,
                TodayBranchkgs = TodaysBranchkg
            });
        }

        public async Task<IActionResult> PrintSupplierStatement()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var branches = await _context.DBranch.Where(s => s.Bcode == sacco)
            .Select(s => s.Bname).ToListAsync();

            ViewBag.slopes = StrValues.Slopes == sacco;
            ViewBag.branches = new SelectList(branches);
            var suppliers = await _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var transporters = await _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();
                transporters = transporters.Where(s => s.Tbranch == saccoBranch).ToList();
            }
            else
            {
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();
                transporters = transporters.Where(s => s.Tbranch == saccoBranch).ToList();
            }

            ViewBag.suppliers = suppliers.Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
            }).ToList();

            var transCodes = new SelectList(transporters.Select(t => t.TransCode).ToList());
            if(StrValues.Slopes == sacco)
                transCodes = new SelectList(transporters.Select(t => t.CertNo).ToList());

            ViewBag.transCodes = transCodes;
            ViewBag.transporters = transporters.Select(s => new DTransporter
            {
                TransCode = s.TransCode,
                TransName = s.TransName,
                CertNo = s.CertNo
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> PrintSupplierStatement([FromBody] StatementFilter filter)
        {
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            filter.Branch = filter?.Branch ?? HttpContext.Session.GetString(StrValues.Branch) ?? "";
            filter.LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.slopes = StrValues.Slopes == filter.Sacco;

            var statement = new SupplierStatement(_context, _bosaDbContext);
            var statementResp = await statement.GenerateStatement(filter);
            return Json(statementResp);
        }

        public async Task<IActionResult> PrintSupPayslip()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";

            var suppliers = await _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

            ViewBag.suppliers = suppliers.Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
            }).ToList();

            var standingOrders = _context.StandingOrder.Where(o => o.SaccoCode == sacco)
                .Select(o => new StandingOrder
                {
                    Sno = o.Sno,
                    Paid = o.Paid,
                    Description = o.Description,
                }).ToList();
            ViewBag.standingOrders = standingOrders;

            var banks = await _context.DBanks.Where(i => i.BankCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            ViewBag.bankNames = new SelectList(banks.Select(t => t.BankName).ToList());
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> PrintSupPayslip([FromBody] StatementFilter filter)
        {
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            filter.Code = filter?.Code ?? "";
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            filter.Branch = filter?.Branch ?? HttpContext.Session.GetString(StrValues.Branch) ?? "";
            filter.LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var payrolls = await _context.DPayrolls.Where(p => p.Sno.ToUpper().Equals(filter.Code.ToUpper())
            && p.EndofPeriod == endDate && p.SaccoCode == filter.Sacco).ToListAsync();
            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == filter.Sacco);

            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(filter.Code.ToUpper()) && s.Scode == filter.Sacco && s.Branch == filter.Branch);
            var transCode = _context.DTransports.FirstOrDefault(s => s.Sno.ToUpper().Equals(filter.Code.ToUpper()) && s.saccocode == filter.Sacco && s.Branch == filter.Branch)?.TransCode ?? "";
            var transporter = _context.DTransporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(transCode.ToUpper()) && s.ParentT == filter.Sacco && s.Tbranch == filter.Branch);
            if (transporter == null)
                transporter = new DTransporter
                {
                    CertNo = "",
                    TransCode = "",
                    TransName = ""
                };
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == filter.Sacco);
            company.SupStatementNote = company?.SupStatementNote ?? "";

            var loanTypes = await _bosaDbContext.LOANTYPE.Where(t => t.CompanyCode == StrValues.SlopesCode).ToListAsync();
            var loanBals = await _bosaDbContext.LOANBAL.Where(t => t.Companycode == StrValues.SlopesCode && t.MemberNo.ToUpper().Equals(filter.Code.ToUpper())).ToListAsync();
            var loans = await _context.SaccoLoans.Where(l => l.Saccocode == filter.Sacco && l.Sno == filter.Code).ToListAsync();
            loans.ForEach(l =>
            {
                var loanType = loanTypes.FirstOrDefault(t => t.LoanCode == l.LoanCode);
                var loanBal = loanBals.FirstOrDefault(s => s.LoanNo == l.LoanNo);
                l.LoanCode = loanType?.LoanType ?? "";
                l.Balance = loanBal?.Balance ?? 0;
            });

            var shares = await _bosaDbContext.CONTRIB.Where(s => s.MemberNo.ToUpper().Equals(filter.Code.ToUpper()) && s.CompanyCode == StrValues.SlopesCode && s.ReceiptNo != "1").ToListAsync();
            var deductedShares = await _context.SaccoShares.Where(l => l.Saccocode == filter.Sacco && l.Sno == filter.Code).ToListAsync();
            shares.ForEach(s =>
            {
                s.Paid = 0;
                if (s.Sharescode == "S03" && s.Amount < 5500)
                    s.Paid = deductedShares.FirstOrDefault()?.Amount ?? 0;
                if (s.Sharescode != "S03" && s.Amount >= 5500)
                    s.Paid = deductedShares.FirstOrDefault()?.Amount ?? 0;
            });
            return Json(new
            {
                payrolls,
                price,
                company,
                supplier,
                transporter,
                loans,
                shares
            });
        }

        [HttpPost]
        public async Task<JsonResult> GetBankFarmersPayslip([FromBody] StatementFilter filter)
        {
            filter.Code = filter?.Code ?? "";
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            filter.Branch = filter?.Branch ?? HttpContext.Session.GetString(StrValues.Branch) ?? "";
            filter.LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            ViewBag.slopes = StrValues.Slopes == filter.Sacco;

            var startDate = new DateTime(filter.Date.Year, filter.Date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var payrolls = await _context.DPayrolls.Where(p => p.Bank.ToUpper().Equals(filter.Code.ToUpper())
           && p.EndofPeriod == endDate && p.SaccoCode == filter.Sacco).ToListAsync();
            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == filter.Sacco);

            var suppliers = await _context.DSuppliers.Where(s => s.Scode == filter.Sacco).ToListAsync();
            var transports = await _context.DTransports.Where(s => s.saccocode == filter.Sacco).ToListAsync();
            var transporters = await _context.DTransporters.Where(s => s.ParentT == filter.Sacco).ToListAsync();
            
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(filter.LoggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                suppliers = suppliers.Where(s => s.Branch == filter.Branch).ToList();
                transports = transports.Where(s => s.Branch == filter.Branch).ToList();
                transporters = transporters.Where(s => s.Tbranch == filter.Branch).ToList();
            }

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == filter.Sacco);
            company.SupStatementNote = company?.SupStatementNote ?? "";

            var loanTypes = await _bosaDbContext.LOANTYPE.Where(t => t.CompanyCode == StrValues.SlopesCode).ToListAsync();
            var loanBals = await _bosaDbContext.LOANBAL.Where(t => t.Companycode == StrValues.SlopesCode).ToListAsync();
            var loans = await _context.SaccoLoans.Where(l => l.Saccocode == filter.Sacco).ToListAsync();

            var shares = await _bosaDbContext.CONTRIB.Where(s => s.CompanyCode == StrValues.SlopesCode && s.ReceiptNo != "1").ToListAsync();
            var deductedShares = await _context.SaccoShares.Where(l => l.Saccocode == filter.Sacco).ToListAsync();
            shares.ForEach(s =>
            {
                s.Paid = 0;
                if (s.Sharescode == "S03" && s.Amount < 5500)
                    s.Paid = deductedShares.FirstOrDefault()?.Amount ?? 0;
                if (s.Sharescode != "S03" && s.Amount >= 5500)
                    s.Paid = deductedShares.FirstOrDefault()?.Amount ?? 0;
            });

            var banksPayslip = new List<dynamic>();
            foreach(var payroll in payrolls)
            {
                payroll.Sno = payroll?.Sno ?? "";
                var supplier = suppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(payroll.Sno.ToUpper()));
                var transCode = transports.FirstOrDefault(s => s.Sno.ToUpper().Equals(payroll.Sno.ToUpper()))?.TransCode ?? "";
                var transporter = transporters.FirstOrDefault(s => s.TransCode.ToUpper().Trim().Equals(transCode.ToUpper().Trim()));
                var memberLoanBals = loanBals.Where(t => t.MemberNo.ToUpper().Equals(payroll.Sno.ToUpper())).ToList();
                var memberLoans = loans.Where(l => l.Sno == payroll.Sno).ToList();
                memberLoans.ForEach(l =>
                {
                    var loanType = loanTypes.FirstOrDefault(t => t.LoanCode == l.LoanCode);
                    var loanBal = memberLoanBals.FirstOrDefault(s => s.LoanNo == l.LoanNo);
                    l.LoanCode = loanType?.LoanType ?? "";
                    l.Balance = loanBal?.Balance ?? 0;
                });

                var memberShares = shares.Where(s => s.MemberNo.ToUpper().Equals(payroll.Sno.ToUpper())).ToList();
                var memberDeductedShares = deductedShares.Where(l => l.Sno == payroll.Sno).ToList();
                memberShares.ForEach(s =>
                {
                    s.Paid = 0;
                    if (s.Sharescode == "S03" && s.Amount < 5500)
                        s.Paid = memberDeductedShares.FirstOrDefault()?.Amount ?? 0;
                    if (s.Sharescode != "S03" && s.Amount >= 5500)
                        s.Paid = memberDeductedShares.FirstOrDefault()?.Amount ?? 0;
                });

                banksPayslip.Add(new
                {
                    payroll.Transport,
                    payroll.Agrovet,
                    payroll.Bonus,
                    payroll.extension,
                    payroll.SMS,
                    payroll.Tmshares,
                    payroll.Fsa,
                    payroll.Hshares,
                    payroll.Advance,
                    payroll.Others,
                    payroll.Tdeductions,
                    payroll.KgsSupplied,
                    payroll.Registration,
                    payroll.Gpay,
                    payroll.Subsidy,
                    payroll.Npay,
                    payroll.AccountNumber,
                    payroll.CLINICAL,
                    payroll.AI,
                    payroll.Tractor,
                    payroll.CurryForward,
                    payroll.MIDPAY,
                    payroll.NOV_OVPMNT,
                    payroll.SACCO_SAVINGS,
                    payroll.SACCO_SHARES,
                    payroll.INST_ADVANCE,
                    payroll.MILK_RECOVERY,
                    payroll.KIIGA,
                    payroll.KIROHA,
                    memberLoans,
                    memberShares,
                    supplier.Names,
                    supplier.Sno,
                    transName = transporter?.TransName ?? "",
                    CertNo = transporter?.CertNo ?? ""
                });
            }

            return Json(new
            {
                banksPayslip,
                price,
                company,
            });
        }


        [HttpPost]
        public async Task<JsonResult> GetTransporterFarmersStat([FromBody] StatementFilter filter)
        {
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            filter.Branch = filter?.Branch ?? HttpContext.Session.GetString(StrValues.Branch) ?? "";
            filter.LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            ViewBag.slopes = StrValues.Slopes == filter.Sacco;
            
            var statement = new SupplierStatement(_context, _bosaDbContext);
            var statementResp = await statement.GetTransporterFarmersStat(filter);
            return Json(statementResp);
        }

        public async Task<IActionResult> PrintTransporterStatement()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var branches = _context.DBranch.Where(s => s.Bcode == sacco)
                .Select(s => s.Bname).ToList();

            ViewBag.branches = new SelectList(branches);
            var transporters = await _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();

            var codes = transporters.Select(t => t.TransCode).ToList();
            if (StrValues.Slopes == sacco)
                codes = transporters.OrderBy(t => t.CertNo).Select(t => t.CertNo).ToList();

            ViewBag.codes = new SelectList(codes);
            ViewBag.slopes = StrValues.Slopes == sacco;
            ViewBag.transporters = transporters.Select(s => new DTransporter
            {
                TransName = s.TransName,
                TransCode = s.TransCode,
                CertNo = s.CertNo
            }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> PrintTransporterStatement([FromBody] StatementFilter filter)
        {
            filter.Code = filter?.Code ?? "";
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            filter.Sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            filter.Branch = filter?.Branch ?? HttpContext.Session.GetString(StrValues.Branch) ?? "";
            filter.LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (StrValues.Slopes == filter.Sacco)
                filter.Code = _context.DTransporters.FirstOrDefault(t => t.CertNo == filter.Code)?.TransCode ?? "";
            var statement = new TransporterStatement(_context);
            var statementResp = await statement.GenerateStatement(filter);
            return Json(statementResp);
        }

        [HttpGet]
        public async Task<JsonResult> SelectedDateIntake(string sno, DateTime? date)
        {
            sno = sno ?? "";
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var branch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var supplier = new DSupplier { Names = "" };
            var transporter = new DTransporter { TransName = "", TransCode = "" };
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var intakes = await _context.ProductIntake.Where(i => i.Sno.ToUpper().Equals(sno.ToUpper()) 
            && i.TransDate >= startDate && i.TransDate <= DateTime.Today 
            && (i.TransactionType == TransactionType.Intake || i.TransactionType == TransactionType.Correction) 
            && i.SaccoCode == sacco).ToListAsync();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == branch).ToList();

            var suppliers = await _context.DSuppliers.Where(L => L.Sno.ToUpper().Equals(sno.ToUpper()) && L.Scode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch.ToUpper().Equals(branch.ToUpper())).ToList();
            if (suppliers.Any())
                supplier = suppliers.FirstOrDefault();
            if (StrValues.Slopes != sacco)
            {
                var transports = await _context.DTransports.Where(t => t.Sno.ToUpper().Equals(sno.ToUpper()) && t.saccocode == sacco).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    transports = transports.Where(s => s.Branch.ToUpper().Equals(branch.ToUpper())).ToList();
                var trancode = transports.FirstOrDefault()?.TransCode ?? "";

                var transporters = await _context.DTransporters.Where(t => t.TransCode.ToUpper().Equals(trancode.ToUpper()) && t.ParentT == sacco).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    transporters = transporters.Where(s => s.Tbranch.ToUpper().Equals(branch.ToUpper())).ToList();
                if (transporters.Any())
                    transporter = transporters.FirstOrDefault();
            }

            return Json(new
            {
                supplier,
                transporter,
                supCum = intakes.Sum(i => i.Qsupplied)
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetTransporterDetails(string vehicleNo)
        {
            vehicleNo = vehicleNo ?? "";
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var branch = HttpContext.Session.GetString(StrValues.Branch);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var transporters = await _context.DTransporters.Where(t => t.CertNo.ToUpper().Equals(vehicleNo.ToUpper()) && t.ParentT == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(s => s.Tbranch.ToUpper().Equals(branch.ToUpper())).ToList();
            var transporter = transporters.FirstOrDefault();
            if (transporter == null)
                transporter = new DTransporter { TransName = "", TransCode = "" };
            return Json(new
            {
                transporter
            });
        }

        [HttpGet]
        public JsonResult checkifalreadyexist(string sno, DateTime date, string type)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var checkifdelievedtoday = _context.ProductIntake.Any(L => L.SaccoCode == sacco
            && L.Branch == saccoBranch && L.TransDate == date && L.ProductType.ToUpper().Equals(type.ToUpper()) && L.Description == "Transport" && L.Sno.ToUpper().Equals(sno.ToUpper()));
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
        public JsonResult SpecifiedDateCollected(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var intakes = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date);
            var Todayskg = intakes.Sum(p => p.Qsupplied);
            var TodaysBranchkg = intakes.Where(s => s.Branch == saccoBranch).Sum(p => p.Qsupplied);

            return Json(new
            {
                Todayskg,
                TodaysBranchkg
            });
        }

        [HttpGet]
        public JsonResult sumDateIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var Todayskg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date).Sum(p => p.Qsupplied);
            //var TodaysSubtractedkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && s.DR>0 && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date).Sum(p => p.Qsupplied);
            var TodaysBranchkg = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
            // var TodaysBranchkgSubtracted = productIntakeslist.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && s.DR>0 && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == date && s.Branch == saccoBranch).Sum(p => p.Qsupplied);

            return Json(new dailymilkVM
            {
                Todayskg = Todayskg, //- TodaysSubtractedkg,
                TodaysBranchkg = TodaysBranchkg  //- TodaysBranchkgSubtracted
            });
        }
        private async Task SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var suppliers = await _context.DSuppliers
                .Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();
            ViewBag.suppliers = suppliers;
            var products = await _context.DPrices.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            ViewBag.products = new SelectList(products, "Products", "Products");
            ViewBag.productPrices = products;

            var Branch = await _context.DBranch.Where(a => a.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToListAsync();
            ViewBag.Branch = new SelectList(Branch);

            var zones  = await _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToListAsync();
            ViewBag.zones = new SelectList(zones);

            var transporters = await _context.DTransporters.Where(t => t.ParentT == sacco).OrderBy(t => t.CertNo).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(s => s.Tbranch == saccoBranch).ToList();

            var vehicles = transporters.Select(t => t.CertNo).ToList();
            ViewBag.vehicles = new SelectList(vehicles);

            if (zones.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;

            List<SelectListItem> morningEve = new()
            {
                new SelectListItem { Text = "Morning" },
                new SelectListItem { Text = "Evening" },
            };
            ViewBag.morningEve = morningEve;

        }

        public IActionResult CreateDeduction()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco);
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);
            Farmersobj = new FarmersVM()
            {
                DSuppliers = suppliers,
                //ProductIntake = new ProductIntake
                //{
                //    TransDate = DateTime.Today
                //}
            };
            return View(Farmersobj);
        }

        public IActionResult CreateTDeduction()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            Farmersobj = new FarmersVM()
            {
                DTransporters = _context.DTransporters.Where(t => t.ParentT == sacco && t.Tbranch == saccoBranch),
                //ProductIntake = new ProductIntake
                //{
                //    TransDate = DateTime.Today
                //}
            };
            return View(Farmersobj);
        }
        public IActionResult CreateStaffDeduction()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
        public async Task<IActionResult> CreateCorrection()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var productIntakes = await _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction)).ToListAsync();
            var intakes = productIntakes.Where(s => s.TransDate == DateTime.Today);
            
            var remarks = "Remarks";
            var remarksValue = "";
            if (StrValues.Slopes == sacco)
            {
                var recentIntake = productIntakes.Where(s => s.TransactionType == TransactionType.Correction).OrderByDescending(i => i.Id).FirstOrDefault();
                long.TryParse(recentIntake?.Remarks ?? "", out long invoiceNo);
                invoiceNo++;
                remarks = "Invoice No.";
                remarksValue = "" + invoiceNo;
            }

            ViewBag.remarks = remarks;
            ViewBag.remarksValue = remarksValue;
            ViewBag.slopes = StrValues.Slopes == sacco;

            var Todayskg = intakes.Sum(p => p.Qsupplied);
            var TodaysBranchkg = intakes.Where(s => s.Branch == saccoBranch).Sum(p => p.Qsupplied);
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            return View(productIntake);
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] ProductIntakeVm productIntake, string vehicleNo)
        {
            vehicleNo = vehicleNo ?? "";
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            productIntake.Branch = saccoBranch;
            productIntake.Sno = productIntake?.Sno ?? "";
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.CR = productIntake?.CR ?? 0;
            productIntake.DR = productIntake?.DR ?? 0;
            productIntake.Auditdatetime = DateTime.Now;
            productIntake.Description = productIntake?.Description ?? "";
            productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json(new
                {
                    success = false
                });
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.Qsupplied < 0.01M)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return Json(new
                {
                    success = false
                });
            }
            var suppliers = await _context.DSuppliers.Where(s => s.Sno == productIntake.Sno && s.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return Json(new
                {
                    success = false
                });
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return Json(new
                {
                    success = false
                });
            }
            
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
             
            productIntake.SaccoCode = sacco;
            productIntake.Zone = supplier?.Zone ?? "";
            productIntake.TransactionType = TransactionType.Intake;
            var prices = 0;
            decimal totalamount = 0;
            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
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
                TransDate = productIntake?.TransDate ?? DateTime.Today,
                TransTime = productIntake.TransTime,
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
                Auditdatetime = productIntake.Auditdatetime,
                Branch = productIntake.Branch,
                SaccoCode = productIntake.SaccoCode,
                DrAccNo = productIntake.DrAccNo,
                CrAccNo = productIntake.CrAccNo,
                Zone = productIntake.Zone,
                MornEvening = productIntake.MornEvening
            };
            _context.ProductIntake.Add(collection);

            if (StrValues.Elburgon != sacco)
            {
                await calcDefaultdeductions(collection);
            }
                

            var activeAssignments = await _context.DTransports.Where(t => t.Active && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper()) && t.saccocode == sacco).ToListAsync();
            var transports = activeAssignments.Where(t => t.Sno == productIntake.Sno).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                transports = transports.Where(s => s.Branch == saccoBranch).ToList();

            var transporters = await _context.DTransporters.Where(h => h.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(s => s.Tbranch == saccoBranch).ToList();

            var transport = transports.FirstOrDefault();
            if (!string.IsNullOrEmpty(productIntake.MornEvening))
                transport = transports.FirstOrDefault(t => t.Morning == productIntake.MornEvening);

            if (StrValues.Elburgon == sacco)
            {
                SharesFilter filter = new SharesFilter()
                { 
                    Code = productIntake.Sno,
                    Sacco = sacco,
                    Branch = saccoBranch,
                    LoggedInUser = loggedInUser,
                    shares = false
                };
                bool wherefrom = false;
                var statement = new SupplierShares(_context, _bosaDbContext);
                await statement.deductshares(filter, wherefrom);
            }
                

            var transNo = transporters.FirstOrDefault(t => t.CertNo.Trim().ToUpper().Equals(vehicleNo.Trim().ToUpper()))?.TransCode ?? "";
            if (StrValues.Slopes == sacco)
            {
                transport = activeAssignments.FirstOrDefault(t => t.TransCode.ToUpper().Equals(transNo.ToUpper()) && t.Sno.ToUpper().Equals(productIntake.Sno.ToUpper()));
                if (transport == null)
                    transport = activeAssignments.FirstOrDefault(t => t.TransCode.ToUpper().Equals(transNo.ToUpper()));
            }
            transport = transport == null ? new DTransport() : transport;
            transport.Rate = transport?.Rate ?? 0;
            transport.TransCode = StrValues.Slopes == sacco ? transNo : transport?.TransCode ?? "";
            var transporter = transporters.FirstOrDefault(t => t.TransCode.Trim().ToUpper().Equals(transport.TransCode.Trim().ToUpper()));
            if (transporter != null)
            {
                // Debit supplier transport amount
                productIntake.CR = 0;
                productIntake.DR = productIntake.Qsupplied * transport.Rate;
                productIntake.Balance = productIntake.Balance - productIntake.DR;

                decimal kgsToEnter = (decimal)productIntake.Qsupplied;
                if (StrValues.Elburgon == sacco )
                {
                    kgsToEnter = 0;
                }

                collection = new ProductIntake
                {
                    Sno = productIntake.Sno.Trim().ToUpper(),
                    TransDate = productIntake?.TransDate ?? DateTime.Today,
                    TransTime = productIntake.TransTime,
                    ProductType = productIntake.ProductType,
                    Qsupplied = kgsToEnter,
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
                
                if (transport.Rate == 0 && transporter.Rate > 0)
                    productIntake.CR = productIntake.Qsupplied * (decimal)transporter.Rate;
                else
                    productIntake.CR = productIntake.Qsupplied * transport.Rate;

                productIntake.DR = 0;
                //if (StrValues.Slopes != sacco)
                productIntake.Remarks = "Intake for: " + productIntake.Sno;

                //var checktransportifalreadyded = productIntakes.FirstOrDefault(m=>m.SaccoCode == sacco && m.Branch == saccoBranch 
                //&& m.Sno.Trim().ToUpper().Equals(transport.TransCode.Trim().ToUpper()) && );
                _context.ProductIntake.Add(new ProductIntake
                {
                    Sno = transport.TransCode.Trim().ToUpper(),
                    TransDate = productIntake?.TransDate ?? DateTime.Today,
                    TransTime = productIntake.TransTime,
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

            var intakes = await _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                            && s.TransDate >= startDate && s.TransDate <= endDate
                            && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction)).ToListAsync();

            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(s => s.Branch == saccoBranch).ToList();

            var commulated = intakes.Sum(s => s.Qsupplied);
            if (productIntake.SMS)
            {
                if (supplier.PhoneNo != "0")
                {
                    if (supplier.PhoneNo.Length > 8)
                    {
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
            _context.SaveChanges();
            _notyf.Success("Intake saved successfully");

            var intake = new ProductIntake
            {
                Sno = productIntake.Sno,
                Qsupplied = (decimal)productIntake.Qsupplied,
                MornEvening = productIntake.MornEvening,
                SaccoCode = productIntake.SaccoCode,
                Branch = saccoBranch,
            };
            var receiptDetails = await GetReceiptDetails(intake, loggedInUser);

            

            //    var intakes = _context.ProductIntake
            //.FirstOrDefault(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            //&& i.Sno == collection.Sno && i.TransDate == collection.TransDate && i.TransTime == collection.TransTime);

            //    return RedirectToAction("createprinttest", new { id = intakes.Id });//"Details", "Event", new { id = thisEvent }

            //var Todayskg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today).Sum(p => p.Qsupplied);
            //var TodaysBranchkg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
            
            return Json(new
            {
                receiptDetails,
                success = true
            });
        }

        [HttpPost]
        public async Task<JsonResult> reprintreceipt([FromBody] ProductIntakeVm productIntake, string transCode)
        {
            transCode = transCode ?? "";
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            productIntake.Branch = saccoBranch;
            productIntake.Sno = productIntake?.Sno ?? "";
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json(new
                {
                    success = false
                });
            }
            IQueryable<DSupplier> dSuppliers = _context.DSuppliers;
            var suppliers = await dSuppliers.Where(s => s.Sno == productIntake.Sno && s.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return Json(new
                {
                    success = false
                });
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return Json(new
                {
                    success = false
                });
            }
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            var kgs = productIntakes.Where(n => n.Sno.ToUpper().Equals(productIntake.Sno.ToUpper()) && n.SaccoCode == sacco && n.Branch == saccoBranch
            && n.TransDate == productIntake.TransDate && (n.TransactionType == TransactionType.Intake || n.TransactionType == TransactionType.Correction)).Sum(c=>c.Qsupplied);
            productIntake.SaccoCode = sacco;
            var intake = new ProductIntake
            {
                Sno = productIntake.Sno,
                Qsupplied = (decimal)kgs,
                MornEvening = productIntake.MornEvening,
                SaccoCode = productIntake.SaccoCode,
                Branch = saccoBranch,
            };
            var receiptDetails = await GetReceiptDetails(intake, loggedInUser);

            var remarksValue = "";

            return Json(new
            {
                receiptDetails,
                remarksValue,
                success = true
            });
        }

        [HttpGet]
        public async Task<JsonResult> ReprintReceipt(long intakeId)
        {
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var intake = _context.ProductIntake.FirstOrDefault(i => i.Id == intakeId);
            var receiptDetails = await GetReceiptDetails(intake, loggedInUser);
            return Json(new
            {
                receiptDetails,
            });
        }

        private async Task<dynamic> GetReceiptDetails(ProductIntake intake, string loggedInUser)
        {
            intake.Sno = intake?.Sno ?? "";
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(intake.SaccoCode.ToUpper()));
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            // cummulative kgs calc
            var intakes = await _context.ProductIntake.Where(o => o.SaccoCode.ToUpper().Equals(intake.SaccoCode.ToUpper()) &&
            o.Sno == intake.Sno && o.Branch.ToUpper().Equals(intake.Branch.ToUpper()) &&
            o.TransDate >= startDate && o.TransDate <= endDate
            && (o.Description == "Intake" || o.Description == "Correction")).ToListAsync();

            var cumkg = intakes.Sum(d => d.Qsupplied);
            string cummkgs = string.Format("{0:.###}", cumkg);
            var suppliers = await _context.DSuppliers.Where(s => s.Sno.ToUpper().Equals(intake.Sno) 
            && s.Scode == intake.SaccoCode).ToListAsync();

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == intake.Branch).ToList();

            var supplier = suppliers.FirstOrDefault();
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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

        public async Task calcDefaultdeductions(ProductIntake collection)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            DateTime sDate = new DateTime(collection.TransDate.Year, collection.TransDate.Month, 1);
            DateTime enDate = sDate.AddMonths(1).AddDays(-1);

            var preSet = await _context.d_PreSets.Where(l => l.Sno.ToUpper().Equals(collection.Sno.ToUpper()) && !l.Stopped
            && l.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                preSet = preSet.Where(t => t.BranchCode == saccoBranch).ToList();

            var Checkanydefaultdeduction = preSet.FirstOrDefault();
            if (Checkanydefaultdeduction != null)
            {
                var intakes = await _context.ProductIntake.Where(f => f.Sno.ToUpper().Equals(collection.Sno.ToUpper())
                && f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= sDate && f.TransDate <= enDate).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    intakes = intakes.Where(t => t.Branch == saccoBranch).ToList();

                var totalkgs = intakes.Where(f => f.TransactionType == TransactionType.Intake).Sum(n => n.Qsupplied);
                var bonus = Checkanydefaultdeduction.Rate;
                if (Checkanydefaultdeduction.Rated == true)
                {
                    totalkgs = (decimal)(totalkgs + collection.Qsupplied);
                    bonus = totalkgs * Checkanydefaultdeduction.Rate;
                }
                var checkintake = intakes.Where(f => f.ProductType.ToUpper().Equals(Checkanydefaultdeduction.Deduction.ToUpper()));
                if (checkintake.Any())
                {
                    _context.ProductIntake.RemoveRange(checkintake);
                    _context.SaveChanges();
                }

                var checkgls = await _context.Gltransactions
                .Where(f => f.Source.ToUpper().Equals(collection.Sno.ToUpper())
                && f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= sDate
                && f.TransDate <= enDate && f.TransDescript.ToUpper().Equals("BONUS".ToUpper())).ToListAsync();
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
                    Branch = saccoBranch
                });

            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch,Zone")] ProductIntake productIntake)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            DateTime startdate = new DateTime(productIntake.TransDate.Year, productIntake.TransDate.Month, 1);
            DateTime enddate = startdate.AddMonths(1).AddDays(-1);
            productIntake.Description = productIntake?.Description ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            
            var intakes = _context.ProductIntake.Where(i => i.Sno == productIntake.Sno && i.SaccoCode == sacco
            && i.Qsupplied != 0 && i.TransDate >= startdate && i.TransDate <= enddate);
            var supplierList = _context.DSuppliers.Where(s => s.Scode == sacco);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                supplierList = supplierList.Where(s => s.Branch == saccoBranch);
                intakes = intakes.Where(i => i.Branch == saccoBranch);
            }

            var message = "Sorry, Supplier does not exist";
            var suppliers = supplierList.Where(s => s.Sno == productIntake.Sno && s.Active);
            if (suppliers.Any())
            {
                message = "Sorry, Kindly approve the supplier";
                suppliers = suppliers.Where(s => s.Approval);
            }

            // s.Approval
            if (!suppliers.Any())
            {
                _notyf.Error(message);
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = supplierList,
                    ProductIntake = productIntake
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (!intakes.Any())
            {
                _notyf.Error("Sorry, Supplier has not deliver any product for this month" + " " + startdate + "To " + " " + enddate);
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = supplierList,
                    ProductIntake = productIntake
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Farmer code cannot be zero");
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = supplierList,
                    ProductIntake = productIntake
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (ModelState.IsValid)
            {
                decimal amounttoshares = 0;
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser);
                if (StrValues.Elburgon == sacco && productIntake.ProductType.ToLower().Contains("share"))
                    auditId = "admin";
                productIntake.AuditId = auditId ?? "";
                productIntake.TransactionType = TransactionType.Deduction;
                productIntake.TransDate = productIntake.TransDate;
                productIntake.SaccoCode = sacco;
                productIntake.Qsupplied = 0;
                if (productIntake.DR > 0)
                {
                    productIntake.DR = productIntake.DR;
                    productIntake.CR = 0;
                    amounttoshares = (decimal)productIntake.DR;
                }
                else
                {
                    productIntake.CR = (productIntake.DR*-1);
                    productIntake.DR = 0;
                    amounttoshares = (decimal)productIntake.CR;
                }
                string re = productIntake.Remarks;
                if (string.IsNullOrEmpty(productIntake.Remarks))
                    re = productIntake.ProductType;
                productIntake.Description = re;
                productIntake.Remarks = re+ "Deducted";
                productIntake.Balance = utilities.GetBalance(productIntake);
                productIntake.Zone = productIntake.Zone;
                _context.Add(productIntake);

                ///for shares deduction it should add to shares and product intake table
                if (productIntake.ProductType.ToLower().Contains("share"))
                {
                    var dSupplier = suppliers.FirstOrDefault();
                    _context.DShares.Add(new DShare
                    {
                        Sno = productIntake.Sno,
                        Bal = amounttoshares,
                        IdNo = dSupplier.IdNo,
                        Code ="",
                        Name = dSupplier.Names,
                        Sex = dSupplier.Type,
                        Loc = dSupplier.Location,
                        Type = dSupplier.Type,
                        TransDate = productIntake.TransDate,
                        Pmode = "Checkoff",
                        Cash = false,
                        Period = productIntake.TransDate.ToString("M"),
                        Amnt =  0,
                        AuditId = loggedInUser,
                        AuditDateTime = DateTime.Now,
                        Shares = 0,
                        Regdate = dSupplier.Regdate,
                        Mno = "0",
                        Amount = amounttoshares,
                        Premium = 0,
                        Spu = 0,
                        SaccoCode = sacco,
                        Branch = productIntake.Branch,
                        zone = "",
                    });
                }
                

                _notyf.Success("Deducted successfully");
                await _context.SaveChangesAsync();
            }

            Farmersobj = new FarmersVM()
            {
                DSuppliers = supplierList,
                ProductIntake = productIntake
            };
            return View(Farmersobj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            //long.TryParse(, out long sno);
            productIntake.Remarks = productIntake?.Remarks ?? "";
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco && t.Tbranch == saccoBranch);
            if (!_context.DTransporters.Any(i => i.TransCode == productIntake.Sno && i.Active == true && i.ParentT == sacco && i.Tbranch == saccoBranch))
            {
                _notyf.Error("Sorry, Transporter code does not exist");
                Farmersobj = new FarmersVM()
                {
                    DTransporters = transporters,
                    ProductIntake = productIntake
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
                    ProductIntake = productIntake
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
                    ProductIntake = productIntake
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
                if (productIntake.DR > 0)
                {
                    productIntake.DR = productIntake.DR;
                    productIntake.CR = 0;
                }
                else
                {
                    productIntake.CR = (productIntake.DR * -1);
                    productIntake.DR = 0;
                }
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
            }

            Farmersobj = new FarmersVM()
            {
                DTransporters = transporters,
                ProductIntake = productIntake
            };
            return View(Farmersobj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaffDeduction([Bind("Id,Empno, Date, Deduction, Amount, Remarks, AuditId, saccocode")] EmployeesDed employeesDed)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            return View(productIntake);
        }

        [HttpPost]
        public async Task<JsonResult> SaveCorrection([FromBody] ProductIntakeVm productIntake, string vehicleNo)
        {
            vehicleNo = vehicleNo ?? "";
            await SetIntakeInitialValues(); 
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            productIntake.SaccoCode = sacco;
            productIntake.Branch = saccoBranch;
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.CR = productIntake?.CR ?? 0;
            productIntake.DR = productIntake?.DR ?? 0;
            productIntake.Auditdatetime = DateTime.Now;
            productIntake.Description = "Correction";
            productIntake.AuditId = loggedInUser ?? "";
            productIntake.TransactionType = TransactionType.Correction;
            productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;

            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json(new
                {
                    success = false
                });
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.Qsupplied == 0)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return Json(new
                {
                    success = false
                });
            }
            
            var suppliers = await _context.DSuppliers.Where(s => s.Sno == productIntake.Sno && s.Scode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return Json(new
                {
                    success = false
                });
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return Json(new
                {
                    success = false
                });
            }
           
            var prices = 0;
            decimal totalamount = 0;
            productIntake.Zone = supplier?.Zone ?? "";
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
                TransTime = productIntake.TransTime,
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

            if (StrValues.Elburgon != sacco)
            {
                await calcDefaultdeductions(collection);
            }

            if (StrValues.Elburgon == sacco)
            {
                SharesFilter filter = new SharesFilter()
                {
                    Code = productIntake.Sno,
                    Sacco = sacco,
                    Branch = saccoBranch,
                    LoggedInUser = loggedInUser,
                    shares = false
                };
                bool wherefrom = false;
                var statement = new SupplierShares(_context, _bosaDbContext);
                await statement.deductshares(filter, wherefrom);
            }

            var activeAssignments = await _context.DTransports.Where(t => t.Active && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper()) && t.saccocode == sacco).ToListAsync();
            var transports = activeAssignments.Where(t => t.Sno == productIntake.Sno).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                transports = transports.Where(s => s.Branch == saccoBranch).ToList();

            var transporters = await _context.DTransporters.Where(h => h.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(s => s.Tbranch == saccoBranch).ToList();

            var transport = transports.FirstOrDefault();
            if (!string.IsNullOrEmpty(productIntake.MornEvening))
                transport = transports.FirstOrDefault(t => t.Morning == productIntake.MornEvening);
            var transNo = transporters.FirstOrDefault(t => t.CertNo.Trim().ToUpper().Equals(vehicleNo.Trim().ToUpper()))?.TransCode ?? "";
            if (StrValues.Slopes == sacco)
            {
                transport = activeAssignments.FirstOrDefault(t => t.TransCode.ToUpper().Equals(transNo.ToUpper()) && t.Sno.ToUpper().Equals(productIntake.Sno.ToUpper()));
                if (transport == null)
                    transport = activeAssignments.FirstOrDefault(t => t.TransCode.ToUpper().Equals(transNo.ToUpper()));
            }
            transport = transport == null ? new DTransport() : transport;
            transport.Rate = transport?.Rate ?? 0;
            transport.TransCode = StrValues.Slopes == sacco ? transNo : transport?.TransCode ?? "";
            var transporter = transporters.FirstOrDefault(t => t.TransCode.Trim().ToUpper().Equals(transport.TransCode.Trim().ToUpper()));
            if (transporter != null)
            {
                // Debit supplier transport amount
                productIntake.CR = 0;
                productIntake.DR = productIntake.Qsupplied * transport.Rate;
                if (ch < 0)
                {
                    productIntake.CR = (decimal?)((productIntake.Qsupplied * transport.Rate) * -1);
                    productIntake.DR = 0;
                }
                decimal kgsToEnter = (decimal)productIntake.Qsupplied;
                if (sacco == "ELBURGON PROGRESSIVE DAIRY FCS")
                {
                    kgsToEnter=0;
                }

                productIntake.Balance = productIntake.Balance + productIntake.CR;
                collection = new ProductIntake
                {
                    Sno = productIntake.Sno.Trim(),
                    TransDate = (DateTime)productIntake.TransDate,
                    TransTime = productIntake.TransTime,
                    ProductType = productIntake.ProductType,
                    Qsupplied = kgsToEnter,
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
                    productIntake.CR = productIntake.Qsupplied * (decimal)transporterscheck.Rate;
                else
                    productIntake.CR = productIntake.Qsupplied * transport.Rate;

                productIntake.DR = 0;
                if (ch < 0)
                {
                    productIntake.DR = (decimal?)((productIntake.CR) * -1);
                    productIntake.CR = 0;
                }

                if(StrValues.Slopes != sacco)
                    productIntake.Remarks = "Intake for " + productIntake.Sno;
                _context.ProductIntake.Add(new ProductIntake
                {
                    Sno = transport.TransCode.Trim(),
                    TransDate = (DateTime)productIntake.TransDate,
                    TransTime = productIntake.TransTime,
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
           
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var intakes = await _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                            && s.TransDate >= startDate && s.TransDate <= endDate 
                            && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction)).ToListAsync();

            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(s => s.Branch == saccoBranch).ToList();

            var commulated = intakes.Sum(s => s.Qsupplied);
            if (productIntake.SMS)
            {
                if (supplier.PhoneNo != "0")
                {
                    if (supplier.PhoneNo.Length > 8)
                    {

                        var startDate1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        var endDate1 = startDate1.AddMonths(1).AddDays(-1);
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

            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(sacco.ToUpper()));
            string cummkgs = string.Format("{0:.###}", commulated + productIntake.Qsupplied);
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

            _context.SaveChanges();
            _notyf.Success("Correction saved successfully");
           
            return Json(new
            {
                receiptDetails,
                success = true
            });
        }

        public IActionResult Reprint(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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

        public IActionResult ChangeTransporter()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco).OrderBy(t => t.CertNo).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(s => s.Tbranch == saccoBranch).ToList();

            var vehicles = transporters.Select(t => t.CertNo).ToList();
            ViewBag.vehicles = new SelectList(vehicles);

            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTransporter([Bind("Code,ReceiptNoFrom,ReceiptNoTo")] TransporterChangeVm changeVm)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var branch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            ViewBag.slopes = StrValues.Slopes == sacco;

            IQueryable<ProductIntake> intakes = _context.ProductIntake.Where(p => p.Id >= changeVm.ReceiptNoFrom && p.Id <= changeVm.ReceiptNoTo 
            && p.SaccoCode == sacco && p.AuditId.ToUpper().Equals(loggedInUser.ToUpper()));
            var transporters = await _context.DTransporters.Where(s => s.ParentT == sacco && s.CertNo == changeVm.Code).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                intakes = intakes.Where(s => s.Branch == branch);
                transporters = transporters.Where(s => s.Tbranch == branch).ToList();
            }

            var transporter = transporters.FirstOrDefault();
            var farmerIntakes = await intakes.Where(i => i.Description != "Transport").ToListAsync();
            var transporterIntakes = await intakes.Where(i => i.Sno.ToUpper().Contains("T")).ToListAsync();
            var productIntakes = new List<ProductIntake>();
            foreach(var intake in farmerIntakes)
            {
                var transporterIntake = transporterIntakes.FirstOrDefault(i => i.Auditdatetime == intake.Auditdatetime);
                if(transporterIntake != null)
                {
                    transporterIntake.Sno = transporter?.TransCode ?? "";
                    transporterIntake.AuditId = loggedInUser;
                }
                if(transporterIntake == null && transporter != null)
                    productIntakes.Add(new ProductIntake
                    {
                        Sno = transporter?.TransCode ?? "",
                        TransDate = intake.TransDate,
                        TransTime = intake.TransTime,
                        ProductType = intake.ProductType,
                        Qsupplied = intake.Qsupplied,
                        Ppu = (decimal?)transporter.Rate,
                        CR = (decimal?)transporter.Rate * intake.Qsupplied,
                        DR = 0,
                        Balance = 0,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = intake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = intake.Auditdatetime,
                        Branch = intake.Branch,
                        SaccoCode = intake.SaccoCode,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = intake.Posted,
                        Zone = intake.Zone,
                        MornEvening = intake.MornEvening
                    });
            }

            if (productIntakes.Any())
                _context.ProductIntake.AddRange(productIntakes);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }

    internal class ReceiptPrinter
    {
    }
}
