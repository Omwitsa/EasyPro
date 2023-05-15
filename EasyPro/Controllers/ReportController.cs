using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Repository;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.Reports;
using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class ReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly IReportProvider _reportProvider;
        readonly IReporting _IReporting;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public ReportController(IReporting iReporting, IReportProvider reportProvider,
            MORINGAContext context, INotyfService notyf)
        {
            _IReporting = iReporting;
            _reportProvider = reportProvider;
            utilities = new Utilities(context);
            _context = context;
            _notyf = notyf;
        }

        public IEnumerable<DSupplier> suppliersobj { get; set; }
        public IEnumerable<ProductIntake> productIntakeobj { get; set; }
        public IEnumerable<DTransporter> transporterobj { get; set; }
        public IEnumerable<DTransportersPayRoll> transporterpayrollobj { get; set; }
        public IEnumerable<DPayroll> dpayrollobj { get; set; }
        public IEnumerable<DCompany> companyobj { get; set; }
        public IEnumerable<DBranch> branchobj { get; set; }
        public IEnumerable<TransportersBalancing> TransportersBalancingobj { get; set; }
        public IEnumerable<DispatchBalancing> DispatchBalancingobj { get; set; }
        public IEnumerable<FLMD> FLMDobj { get; set; }
        public IEnumerable<FLMDCrops> FLMDCropsobj { get; set; }
        public IEnumerable<FLMDLand> FLMDLandobj { get; set; }

        [HttpGet]
        public IActionResult DownloadReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadTransportersBalReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }

        public IActionResult TestReport()
        {
            utilities.SetUpPrivileges(this);
            var webReport = new WebReport();
            webReport.Report.Load(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "report.frx"));

            return View(webReport);
        }

        [HttpGet]//DownloadDispatchBalReport
        public IActionResult DownloadDispatchBalReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadTReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadPReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadActiveSuppliersPReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        public IActionResult FlmdData(long? id)
        {
            utilities.SetUpPrivileges(this);
           
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(0);
            var endDate = month.AddMonths(1).AddDays(-1);
            string sno = null;
            var flmdData = Gedflmdfarmers(startDate, endDate,sno);
            return View(new FlmdDataVM { farmerdetails= flmdData.OrderBy(m=>m.Sno)});
        }

        [HttpGet]
        public JsonResult Selectemaxloan(string sno)
        {
            utilities.SetUpPrivileges(this);

            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(0);
            var endDate = month.AddMonths(1).AddDays(-1);
            var flmdData = Gedflmdfarmers(startDate, endDate,sno);
            return Json(flmdData.Sum(b=>b.Total));
        }
        private List<farmerdetail> Gedflmdfarmers(DateTime startDate, DateTime endDate, string? sno)
        {
            decimal Total = 0;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);

            var flmdsup = _context.FLMD.Where(j => j.SaccoCode == sacco).Select(m => m.Sno).Distinct().ToList();
            var getactivesup1 = _context.ProductIntake.Where(M => M.TransDate >= startDate && M.TransDate <= endDate && M.SaccoCode == sacco).ToList();
            var getactivesup = getactivesup1.Select(m => m.Sno).Distinct().ToList();
            var supplierslist = _context.DSuppliers.Where(M => M.Scode == sacco && (flmdsup.Contains(M.Sno) || getactivesup.Contains(M.Sno))).ToList().OrderBy(s => s.Sno).GroupBy(n => n.Sno).ToList();// .ToList();

            
                //return false;
            //}
            var FlmdData = new List<farmerdetail>();
            if (supplierslist.Count > 0)
            {
                supplierslist.ForEach(l =>
                {
                    var supplier = _context.DSuppliers.FirstOrDefault(m => m.Scode == sacco && m.Sno.ToUpper().Equals(l.Key.ToUpper()));
                    decimal milk = 0, assets = 0, land = 0, crops = 0, eductaion = 0, animals = 0;
                    var productIntakes = getactivesup1.Where(M => M.Sno.ToUpper().Equals(supplier.Sno.ToUpper())).ToList();
                    var flmdanimalandeducation = _context.FLMD.FirstOrDefault(k => k.Sno.ToUpper().Equals(supplier.Sno.ToUpper()) && k.SaccoCode == sacco);
                    var flmdcrops = _context.FLMDCrops.FirstOrDefault(k => k.Sno.ToUpper().Equals(supplier.Sno.ToUpper()) && k.SaccoCode == sacco);
                    var flmdland = _context.FLMDLand.FirstOrDefault(k => k.Sno.ToUpper().Equals(supplier.Sno.ToUpper()) && k.SaccoCode == sacco);

                    if (productIntakes.Count > 0)
                    {
                        milk = (productIntakes.Sum(d => d.CR) ?? 0) - (productIntakes.Sum(d => d.DR) ?? 0);
                    }

                    //animals = (decimal)(((double)flmdanimalandeducation.Sum(g => g.ExoticCattle)*40000)+((double)flmdanimalandeducation.Sum(g => g.IndigenousCattle)*20000)+((double)flmdanimalandeducation.Sum(g => g.Sheep)*6000)+((double)flmdanimalandeducation.Sum(g => g.Goats)*5000)+((double)flmdanimalandeducation.Sum(g => g.Donkeys)*4000)+((double)flmdanimalandeducation.Sum(g => g.Pigs)*8000));
                    if (flmdanimalandeducation != null)
                    {
                        animals = (decimal)(((double)(flmdanimalandeducation.ExoticCattle ?? 0) * 40000) + ((double)(flmdanimalandeducation.IndigenousCattle ?? 0) * 20000) + ((double)(flmdanimalandeducation.Sheep ?? 0) * 6000) + ((double)(flmdanimalandeducation.Goats ?? 0) * 5000) + ((double)(flmdanimalandeducation.Donkeys ?? 0) * 4000) + ((double)(flmdanimalandeducation.Pigs ?? 0) * 8000));
                        eductaion = (decimal)(((double)(flmdanimalandeducation.Graduates ?? 0) * 120000) + ((double)(flmdanimalandeducation.UnderGraduates ?? 0) * 80000) + ((double)(flmdanimalandeducation.Secondary ?? 0) * 53000) + ((double)(flmdanimalandeducation.Primary ?? 0) * 26000));
                    }

                    //eductaion = (decimal)(((double)flmdanimalandeducation.Sum(g => g.Graduates) * 120000) + ((double)flmdanimalandeducation.Sum(g => g.UnderGraduates) * 80000) + ((double)flmdanimalandeducation.Sum(g => g.Secondary) * 53000) + ((double)flmdanimalandeducation.Sum(g => g.Primary) * 26000));
                    if (flmdland != null)
                    {
                        crops = 120000;
                    }

                    //land = (decimal)((double)flmdland.Sum(g => g.TotalAcres) * 1200000);
                    if (flmdland != null)
                    {
                        land = (decimal)((double)flmdland.TotalAcres * 1200000);
                    }

                    assets = (animals - eductaion + crops + land) * (decimal)(0.01);
                    Total = milk + assets;
                    if (Total > 0)
                    {
                        FlmdData.Add(new farmerdetail
                        {
                            Sno = supplier.Sno,
                            Name = supplier.Names,
                            Phone = supplier.PhoneNo,
                            MilkKgs = milk,
                            Assets = assets,
                            Total = Total,
                        });
                        _context.SaveChanges();
                    }

                });
            }
            return FlmdData;

        }

        [HttpPost]
        public IActionResult FlmdDataExcel([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            suppliersobj = _context.DSuppliers
                .Where(p => p.Scode == sacco)
                .OrderBy(s => s.Sno);
            return FlmdDataDownloadExcel();
        }
        public IActionResult FlmdDataDownloadExcel()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("suppliersobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Flmd Loan Register Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Phone No";
                worksheet.Cell(currentRow, 4).Value = "Milk Amount";
                worksheet.Cell(currentRow, 5).Value = "Asset Amount";
                worksheet.Cell(currentRow, 6).Value = "Maximum Loan Amount";
                int sum = 0, sum2 = 0;
                sum2 = suppliersobj.Count();

                var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var startDate = month.AddMonths(0);
                var endDate = month.AddMonths(1).AddDays(-1);
                string sno = null;
                var flmdData = Gedflmdfarmers(startDate, endDate,sno);
                decimal totals = 0;
                flmdData.ForEach(c=> {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = c.Sno;
                    worksheet.Cell(currentRow, 2).Value = c.Name;
                    worksheet.Cell(currentRow, 3).Value = "'"+c.Phone;
                    worksheet.Cell(currentRow, 4).Value = c.MilkKgs;
                    worksheet.Cell(currentRow, 5).Value = c.Assets;
                    worksheet.Cell(currentRow, 6).Value = c.Total;
                    totals += c.Total;
                });
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Total Maximum Loan:";
                worksheet.Cell(currentRow, 6).Value = totals;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Flmd Loan Report Register.xlsx");
                }
            }
        }


        public IActionResult DownloadcorrectionIntakeReport()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }

        public IActionResult DownloadSalesReport()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var debtors = _context.DDebtors.Where(g => g.Dcode == sacco).ToList();
            ViewBag.debtors = new SelectList(debtors, "Dname", "Dname");
            return View();
        }

        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var brances = _context.DBranch.Where(i => i.Bcode == sacco).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                brances = brances.Where(i => i.Bname == saccoBranch).ToList();
            ViewBag.brances = new SelectList(brances.Select(b => b.Bname));

            var Transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                Transporters = Transporters.Where(i => i.Tbranch == saccoBranch).ToList();
            ViewBag.Transporters = new SelectList(Transporters, "TransName", "TransName");

            var Transportersdetails = _context.DTransporters.Where(i => i.ParentT == sacco).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                Transportersdetails = Transportersdetails.Where(i => i.Tbranch == saccoBranch).ToList();
            ViewBag.Transportersdetails = Transportersdetails;

            var bankname = _context.DPayrolls.Where(i => i.SaccoCode == sacco).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                bankname = bankname.Where(i => i.Branch == saccoBranch).ToList();
            ViewBag.bankname = new SelectList(bankname.OrderBy(m => m.Bank).Select(h => h.Bank).Distinct());
        }

        [HttpPost]
        public IActionResult Suppliers([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            suppliersobj = _context.DSuppliers.Where(u => u.Scode == sacco);
            return Excel();
        }

        [HttpPost]
        public IActionResult SuppliersPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            suppliersobj = _context.DSuppliers.Where(u => u.Scode == sacco);
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Supplier";
            var pdfFile = _reportProvider.GetSuppliersReport(suppliersobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult Transporter([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            transporterobj = _context.DTransporters.Where(u => u.ParentT == sacco);
            return TransporterExcel();
        }

        [HttpPost]
        public IActionResult TransporterPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            //var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            //var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            transporterobj = _context.DTransporters.Where(u => u.ParentT == sacco);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Transporters List";
            var pdfFile = _reportProvider.GetTransporterReport(transporterobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult SuppliersPayrollDetail([Bind("DateFrom,DateTo,BankName")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            dpayrollobj = _context.DPayrolls
                .Where(p => p.EndofPeriod >= filter.DateFrom && p.Bank== filter.BankName && p.Npay>0 && p.EndofPeriod <= filter.DateTo && p.SaccoCode == sacco)
                .OrderBy(s => s.Sno);
            
            return SuppliersPayrollDetailExcel();
        }

        [HttpPost]
        public IActionResult SuppliersPayrollExcel([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            dpayrollobj = _context.DPayrolls
                .Where(p => p.EndofPeriod >= filter.DateFrom && p.EndofPeriod <= filter.DateTo && p.SaccoCode == sacco)
                .OrderBy(s => s.Sno);
            return suppliersPayrollExcel();
        }

        [HttpPost]
        public JsonResult SuppliersPayrollPdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("SuppliersPayrollPdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }

        [HttpGet]
        public IActionResult SuppliersPayrollPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            dpayrollobj = _context.DPayrolls
                .Where(p => p.EndofPeriod >= dateFrom && p.EndofPeriod <= dateTo && p.SaccoCode == sacco)
                .OrderBy(s => s.Sno);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = $"Suppliers Payroll Reports ({dateTo})";
            var pdfFile = _reportProvider.GetSuppliersPayroll(dpayrollobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult SuppliersActive([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            
            return SuppliersActiveExcel(filter);
        }

        [HttpPost]
        public IActionResult SuppliersInActive([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";
            var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= filter.DateFrom && i.TransDate <= filter.DateTo
            && i.SaccoCode == sacco && i.Branch == saccoBranch)
                .Select(i => i.Sno);
            suppliersobj = _context.DSuppliers
                .Where(u => u.Scode == sacco && !activeSuppliers.Contains(u.Sno.ToString()))
                .OrderBy(u => u.Sno);
            return SuppliersInActiveExcel(filter);
        }

        [HttpPost]
        public JsonResult SuppliersActivePdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("SuppliersActivePdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }

        [HttpGet]
        public IActionResult SuppliersActivePdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= dateFrom && i.TransDate <= dateTo)
                .Select(i => i.Sno);
            suppliersobj = _context.DSuppliers
                .Where(u => u.Scode == sacco && activeSuppliers.Contains(u.Sno.ToString()))
                .OrderBy(u => u.Sno);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Active Supplier";
            var pdfFile = _reportProvider.GetSuppliersReport(suppliersobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public JsonResult SuppliersInActivePdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("SuppliersInActivePdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }

        [HttpGet]
        public IActionResult SuppliersInActivePdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";
            var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= dateFrom
            && i.TransDate <= dateTo && i.SaccoCode == sacco && i.Branch == saccoBranch).Select(i => i.Sno);
            suppliersobj = _context.DSuppliers
                .Where(u => u.Scode == sacco && !activeSuppliers.Contains(u.Sno.ToString()))
                .OrderBy(u => u.Sno);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "In Active Supplier";
            var pdfFile = _reportProvider.GetSuppliersReport(suppliersobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult BranchIntake([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            if (string.IsNullOrEmpty(filter.Branch))
                branchobj = _context.DBranch.Where(u => u.Bcode == sacco);
            else
                branchobj = _context.DBranch.Where(u => u.Bcode == sacco && u.Bname == filter.Branch);
            return BranchIntakeExcel(DateFrom, DateTo, filter.Branch);
        }

        [HttpPost]
        public IActionResult BranchIntakeAudit([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                if (string.IsNullOrEmpty(filter.Branch))
                {
                    GetInitialValues();
                    _notyf.Error("Sorry, Provide the branch Name");
                    return RedirectToAction(nameof(DownloadcorrectionIntakeReport));
                }
            }

            if (string.IsNullOrEmpty(filter.Branch))
                branchobj = _context.DBranch.Where(u => u.Bcode == sacco);
            else
                branchobj = _context.DBranch.Where(u => u.Bcode == sacco && u.Bname == filter.Branch);
            return BranchIntakeAuditExcel(DateFrom, DateTo, filter.Branch);
        }
        [HttpPost]
        public IActionResult correctionIntake([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            //if (string.IsNullOrEmpty(filter.Branch))
            //    branchobj = _context.DBranch.Where(u => u.Bcode == sacco);
            //else
            //    branchobj = _context.DBranch.Where(u => u.Bcode == sacco && u.Bname == filter.Branch);
            return CorrectionIntakeExcel(DateFrom, DateTo, filter.Branch);
        }

        [HttpPost]
        public IActionResult SalesReport([Bind("DateFrom,DateTo,Debtor")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());

            //if (string.IsNullOrEmpty(filter.Branch))
            //    branchobj = _context.DBranch.Where(u => u.Bcode == sacco);
            //else
            //    branchobj = _context.DBranch.Where(u => u.Bcode == sacco && u.Bname == filter.Branch);
            return SalesReportExcel(filter);
        }

        public IActionResult SalesReportExcel(FilterVm filter)
        {
            decimal totalAmount = 0, totalQuantity = 0;
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var worksheet = workbook.Worksheets.Add("Gltransaction");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }

                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = $"{filter.Debtor} Sales Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "TransDate";
                worksheet.Cell(currentRow, 2).Value = "Qsupplied";
                worksheet.Cell(currentRow, 3).Value = "Price";
                worksheet.Cell(currentRow, 4).Value = "Amount";

                var sales = _context.Gltransactions.Where(u => u.SaccoCode == sacco
                && u.TransDescript == "Sales" && u.TransDate >= filter.DateFrom && u.TransDate <= filter.DateTo
                && (string.IsNullOrEmpty(filter.Debtor) || u.Source.ToUpper().Equals(filter.Debtor.ToUpper())))
                    .OrderByDescending(u => u.Id);
                foreach (var sale in sales)
                {
                    currentRow++;
                    var dispatch = _context.Dispatch.FirstOrDefault(d => d.DName.ToUpper().Equals(sale.Source.ToUpper())
                    && d.Dcode.ToUpper().Equals(sale.SaccoCode.ToUpper()) && d.Transdate >= filter.DateFrom
                    && d.Transdate <= filter.DateTo);

                    var debtor = _context.DDebtors.FirstOrDefault(d => d.Dcode == sacco && d.Dname.ToUpper().Equals(sale.Source.ToUpper()));

                    worksheet.Cell(currentRow, 1).Value = sale.TransDate;
                    worksheet.Cell(currentRow, 2).Value = dispatch.Dispatchkgs;
                    worksheet.Cell(currentRow, 3).Value = debtor.Price;
                    worksheet.Cell(currentRow, 4).Value = sale.Amount;

                    totalAmount += (dispatch.Dispatchkgs);
                    totalQuantity += (sale.Amount);
                }
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Total";
                worksheet.Cell(currentRow, 2).Value = totalAmount;
                worksheet.Cell(currentRow, 4).Value = totalQuantity;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Sales.xlsx");
                }
            }
        }

        [HttpPost]
        public JsonResult BranchIntakePdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("BranchIntakePdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo, branch = filter.Branch }),
                isRedirect = true
            });
        }

        [HttpGet]
        public IActionResult BranchIntakePdf(DateTime? dateFrom, DateTime? dateTo, string branch)
        {
            var filter = new FilterVm
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                Branch = branch
            };
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            if (string.IsNullOrEmpty(filter.Branch))
                branchobj = _context.DBranch.Where(u => u.Bcode == sacco);
            else
                branchobj = _context.DBranch.Where(u => u.Bcode == sacco && u.Bname == filter.Branch);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Branch Intake";
            var pdfFile = _reportProvider.GetBranchIntakeReport(branchobj, company, title, filter);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public async Task<IActionResult> DSumarryIntake([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var endDate = Convert.ToDateTime(filter.DateTo.ToString());
            var DateFrom = new DateTime(endDate.Year, endDate.Month, 1);
            var DateTo = DateFrom.AddMonths(1).AddDays(-1);
            var Branch = filter.Branch;

            if (Branch == null)
                branchobj = await _context.DBranch.Where(u => u.Bcode == sacco).ToListAsync();
            else
                branchobj = await _context.DBranch.Where(u => u.Bcode == sacco && u.Bname == Branch).ToListAsync();
            return await DSumarryIntakeExcel(DateFrom, DateTo, Branch);
        }
        [HttpPost] //DownloadDispatchBalReport
        public async Task<IActionResult> TransportersBalancing([Bind("DateFrom,DateTo,Branch,Transporter")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var endDate = Convert.ToDateTime(filter.DateTo.ToString());
            var DateFrom = new DateTime(endDate.Year, endDate.Month, 1);
            var DateTo = DateFrom.AddMonths(1).AddDays(-1);
            var Branch = filter.Branch;
            var Transporter = filter.Transporter;

            if (Transporter == null)
                transporterobj = await _context.DTransporters.Where(u => u.ParentT == sacco).ToListAsync();
            else
                transporterobj = await _context.DTransporters.Where(u => u.ParentT == sacco && u.TransCode == Transporter).ToListAsync();
            return await TransportersBalancingExcel(DateFrom, DateTo, Transporter);
        }
        [HttpPost]
        public async Task<IActionResult> DownloadDispatchBalReport([Bind("DateFrom,DateTo,Branch,Transporter")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var endDate = Convert.ToDateTime(filter.DateTo.ToString());
            var DateFrom = new DateTime(endDate.Year, endDate.Month, 1);
            var DateTo = DateFrom.AddMonths(1).AddDays(-1);
            var Branch = filter.Branch;
            var Transporter = filter.Transporter;
            DispatchBalancingobj = await _context.DispatchBalancing.Where(u => u.Saccocode == sacco).ToListAsync();

            return await DispatchBalancingExcel(DateFrom, DateTo, Transporter);
        }

        [HttpPost]
        public IActionResult TransportersPayrollExcel([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            transporterpayrollobj = _context.DTransportersPayRolls
                .Where(p => p.SaccoCode == sacco && p.EndPeriod >= filter.DateFrom && p.EndPeriod <= filter.DateTo)
                .OrderBy(p => p.Code);
            return TransportersPayrollExcel();
        }

        [HttpPost]
        public JsonResult TransportersPayrollPdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("TransportersPayrollPdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }

        [HttpGet]
        public IActionResult TransportersPayrollPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            transporterpayrollobj = _context.DTransportersPayRolls
                .Where(p => p.SaccoCode == sacco && p.EndPeriod >= dateFrom && p.EndPeriod <= dateTo)
                .OrderBy(p => p.Code);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = $"Transporters Payroll Reports ({dateTo})";
            var pdfFile = _reportProvider.GetTransportersPayroll(transporterpayrollobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult Deductions([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied == 0 && u.SaccoCode == sacco);
            
            return DeductionsExcel();
        }

        [HttpPost]
        public JsonResult DeductionsPdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("DeductionsPdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }

        [HttpGet]
        public IActionResult DeductionsPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= dateFrom && u.TransDate <= dateTo && u.Qsupplied == 0 && u.SaccoCode == sacco);
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Suppliers Deductions";
            var pdfFile = _reportProvider.GetIntakesPdf(productIntakeobj, company, title, TransactionType.Deduction);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult TDeductions([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());

            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied == 0 && u.SaccoCode == sacco).OrderBy(n=>n.TransDate);
            return TDeductionsExcel();
        }

        [HttpPost]
        public IActionResult Intake([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            //dSupplier.Scode = sacco;

            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied != 0 && u.SaccoCode == sacco && u.Description != "Transport");
            return IntakeExcel();
        }

        [HttpPost]
        public JsonResult IntakePdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("IntakePdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }
        [HttpGet]
        public IActionResult IntakePdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= dateFrom && u.TransDate <= dateTo && u.Qsupplied != 0 && u.SaccoCode == sacco);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Intakes";
            var pdfFile = _reportProvider.GetIntakesPdf(productIntakeobj, company, title, TransactionType.Intake);
            return File(pdfFile, "application/pdf");
        }
        [HttpPost]
        public IActionResult TIntake([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            transporterobj = _context.DTransporters.Where(u => u.ParentT == sacco);
            return TIntakeExcel(DateFrom, DateTo);
        }
        [HttpPost]
        public JsonResult TIntakePdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("TIntakePdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
                isRedirect = true
            });
        }
        [HttpGet]
        public IActionResult TIntakePdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            var filter = new FilterVm
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            transporterobj = _context.DTransporters.Where(u => u.ParentT == sacco);
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Transpoter Intakes";
            var pdfFile = _reportProvider.GetTIntakePdf(transporterobj, company, title, filter);
            return File(pdfFile, "application/pdf");
        }

        public IActionResult TransporterExcel()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("transporterobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Transporters Register";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "TransCode";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "RegDate";
                worksheet.Cell(currentRow, 4).Value = "IDNo";
                worksheet.Cell(currentRow, 5).Value = "Phone";
                worksheet.Cell(currentRow, 6).Value = "Bank";
                worksheet.Cell(currentRow, 7).Value = "AccNo";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                worksheet.Cell(currentRow, 9).Value = "Active";
                worksheet.Cell(currentRow, 10).Value = "Payment Mode";
                worksheet.Cell(currentRow, 11).Value = "Station";

                foreach (var emp in transporterobj)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.TransCode;
                    worksheet.Cell(currentRow, 2).Value = emp.TransName;
                    worksheet.Cell(currentRow, 3).Value = emp.TregDate;
                    worksheet.Cell(currentRow, 4).Value = emp.CertNo;
                    worksheet.Cell(currentRow, 5).Value = emp.Phoneno;
                    worksheet.Cell(currentRow, 6).Value = emp.Bcode;
                    worksheet.Cell(currentRow, 7).Value = emp.Accno;
                    worksheet.Cell(currentRow, 8).Value = emp.Bbranch;
                    worksheet.Cell(currentRow, 9).Value = emp.Active;
                    worksheet.Cell(currentRow, 10).Value = emp.PaymenMode;
                    worksheet.Cell(currentRow, 11).Value = emp.Tbranch;

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transporters Register.xlsx");
                }
            }
        }
        public IActionResult TransportersPayrollExcel()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("transporterpayrollobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Transporters Payroll Report";
                var EndPeriod = transporterpayrollobj.FirstOrDefault();
                worksheet.Cell(currentRow, 4).Value = EndPeriod.EndPeriod;
                // SNo, Transport, Agrovet, Bonus,, HShares, Advance, TDeductions
                // , KgsSupplied, GPay,
                //Bank, AccountNumber, BBranch
                //Tractor, CLINICAL, VARIANCE, CurryForward, extension
                //  TMShares, FSA, AI, 
                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "Code";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "IdNo";
                worksheet.Cell(currentRow, 4).Value = "Kgs";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Subsidy";
                worksheet.Cell(currentRow, 7).Value = "GrossPay";
                worksheet.Cell(currentRow, 8).Value = "Agrovet";
                worksheet.Cell(currentRow, 9).Value = "Shares";
                worksheet.Cell(currentRow, 10).Value = "Advance";
                worksheet.Cell(currentRow, 11).Value = "Tractor";
                worksheet.Cell(currentRow, 12).Value = "CLINICAL";
                worksheet.Cell(currentRow, 13).Value = "VARIANCE";
                worksheet.Cell(currentRow, 14).Value = "CurryForward";
                worksheet.Cell(currentRow, 15).Value = "extension";
                worksheet.Cell(currentRow, 16).Value = "AI";
                worksheet.Cell(currentRow, 17).Value = "Others";
                worksheet.Cell(currentRow, 18).Value = "Totaldeductions";
                worksheet.Cell(currentRow, 19).Value = "NPay";
                worksheet.Cell(currentRow, 20).Value = "Bank";
                worksheet.Cell(currentRow, 21).Value = "AccountNumber";
                worksheet.Cell(currentRow, 22).Value = "Branch";

                double? QntySup = 0;
                decimal? Subsidy = 0;
                decimal? GrossPay = 0;
                decimal? Hshares = 0;
                decimal? Advance = 0;
                decimal? Tractor = 0;
                decimal? CLINICAL = 0;
                decimal? VARIANCE = 0;
                decimal? CurryForward = 0;
                decimal? extension = 0;
                decimal? AI = 0;
                decimal? Amnt = 0;
                decimal? Agrovet = 0;
                decimal? Others = 0;
                decimal? Totaldeductions = 0;
                decimal? Gpay = 0;
                decimal? Npay = 0;
                foreach (var emp in transporterpayrollobj)
                {
                    QntySup = transporterpayrollobj.Sum(k => k.QntySup);
                    Agrovet = transporterpayrollobj.Sum(k => k.Agrovet);
                    Subsidy = transporterpayrollobj.Sum(k => k.Subsidy);
                    GrossPay = transporterpayrollobj.Sum(k => k.GrossPay);
                    Hshares = transporterpayrollobj.Sum(k => k.Hshares);
                    Advance = transporterpayrollobj.Sum(k => k.Advance);
                    Tractor = transporterpayrollobj.Sum(k => k.Tractor);
                    CLINICAL = transporterpayrollobj.Sum(k => k.CLINICAL);
                    VARIANCE = transporterpayrollobj.Sum(k => k.VARIANCE);
                    CurryForward = transporterpayrollobj.Sum(k => k.CurryForward);
                    extension = transporterpayrollobj.Sum(k => k.extension);
                    AI = transporterpayrollobj.Sum(k => k.AI);
                    Amnt = transporterpayrollobj.Sum(k => k.Amnt);
                    Others = transporterpayrollobj.Sum(k => k.Others);
                    Totaldeductions = transporterpayrollobj.Sum(k => k.Totaldeductions);
                    Gpay = transporterpayrollobj.Sum(k => k.GrossPay);
                    Npay = transporterpayrollobj.Sum(k => k.NetPay);

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Code;
                    var TName = _context.DTransporters.FirstOrDefault(u => u.TransCode == emp.Code && u.ParentT == sacco);
                    worksheet.Cell(currentRow, 2).Value = TName.TransName;
                    worksheet.Cell(currentRow, 3).Value = "" + TName.CertNo;
                    worksheet.Cell(currentRow, 4).Value = emp.QntySup;
                    worksheet.Cell(currentRow, 5).Value = emp.Amnt;
                    worksheet.Cell(currentRow, 6).Value = emp.Subsidy;
                    worksheet.Cell(currentRow, 7).Value = emp.GrossPay;
                    worksheet.Cell(currentRow, 8).Value = emp.Agrovet;
                    worksheet.Cell(currentRow, 9).Value = emp.Hshares;
                    worksheet.Cell(currentRow, 10).Value = emp.Advance;
                    worksheet.Cell(currentRow, 11).Value = emp.Tractor;
                    worksheet.Cell(currentRow, 12).Value = emp.CLINICAL;
                    worksheet.Cell(currentRow, 13).Value = emp.VARIANCE;
                    worksheet.Cell(currentRow, 14).Value = emp.CurryForward;
                    worksheet.Cell(currentRow, 15).Value = emp.extension;
                    worksheet.Cell(currentRow, 16).Value = emp.AI;
                    worksheet.Cell(currentRow, 17).Value = emp.Others;
                    worksheet.Cell(currentRow, 18).Value = emp.Totaldeductions;
                    worksheet.Cell(currentRow, 19).Value = emp.NetPay;
                    worksheet.Cell(currentRow, 20).Value = emp.BankName;
                    worksheet.Cell(currentRow, 21).Value = "" + emp.AccNo;
                    worksheet.Cell(currentRow, 22).Value = emp.Branch;
                }
                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total";
                worksheet.Cell(currentRow, 4).Value = QntySup;
                worksheet.Cell(currentRow, 5).Value = Amnt;
                worksheet.Cell(currentRow, 6).Value = Subsidy;
                worksheet.Cell(currentRow, 7).Value = GrossPay;
                worksheet.Cell(currentRow, 8).Value = Agrovet;
                worksheet.Cell(currentRow, 9).Value = Hshares;
                worksheet.Cell(currentRow, 10).Value = Advance;
                worksheet.Cell(currentRow, 11).Value = Tractor;
                worksheet.Cell(currentRow, 12).Value = CLINICAL;
                worksheet.Cell(currentRow, 13).Value = VARIANCE;
                worksheet.Cell(currentRow, 14).Value = CurryForward;
                worksheet.Cell(currentRow, 15).Value = extension;
                worksheet.Cell(currentRow, 16).Value = AI;
                worksheet.Cell(currentRow, 17).Value = Others;
                worksheet.Cell(currentRow, 18).Value = Totaldeductions;
                worksheet.Cell(currentRow, 19).Value = Npay;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transporters Payroll Report.xlsx");
                }
            }
        }

        public IActionResult SuppliersPayrollDetailExcel()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("dpayrollobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                
                var EndofPeriod = dpayrollobj.FirstOrDefault();
                worksheet.Cell(currentRow, 2).Value = EndofPeriod.Bank.ToUpper() + " Payroll List For:";
                worksheet.Cell(currentRow, 4).Value = EndofPeriod.EndofPeriod;
                // SNo, Transport, Agrovet, Bonus,, HShares, Advance, TDeductions, KgsSupplied, GPay,
                //Bank, AccountNumber, BBranch
                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "IdNo";
                worksheet.Cell(currentRow, 4).Value = "Bank";
                worksheet.Cell(currentRow, 5).Value = "AccountNumber";
                worksheet.Cell(currentRow, 6).Value = "BBranch";
                worksheet.Cell(currentRow, 7).Value = "NPay";

                decimal? SNpay=0, TNpay = 0;

                var gettransporterspayrolllist = _context.DTransportersPayRolls
                .Where(p => p.SaccoCode == sacco && p.BankName == EndofPeriod.Bank && p.NetPay > 0 && p.EndPeriod == EndofPeriod.EndofPeriod)
                .OrderBy(p => p.Code).ToList();

                TNpay = gettransporterspayrolllist.Sum(k => k.NetPay);
                var transporterspayroll = gettransporterspayrolllist.GroupBy(l=>l.Code).ToList();

                foreach (var emp in dpayrollobj)
                {
                    SNpay = dpayrollobj.Sum(k => k.Npay); 

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Sno;
                    
                    //long.TryParse(emp.Sno, out long sno);
                    var TName = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                    foreach (var al in TName)
                        worksheet.Cell(currentRow, 2).Value = al.Names;
                    worksheet.Cell(currentRow, 3).Value = "'" + emp.IdNo;
                    worksheet.Cell(currentRow, 4).Value = emp.Bank;
                    worksheet.Cell(currentRow, 5).Value = "'"+emp.AccountNumber;
                    worksheet.Cell(currentRow, 6).Value = emp.Bbranch;
                    worksheet.Cell(currentRow, 7).Value = emp.Npay;
                }
                
                transporterspayroll.ForEach(l => {
                    
                    var transporterdetails = l.FirstOrDefault();
                    
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = transporterdetails.Code;
                    var namelist = _context.DTransporters.FirstOrDefault(u => u.TransCode == transporterdetails.Code && u.ParentT == sacco);
                    worksheet.Cell(currentRow, 2).Value = namelist.TransName;
                    worksheet.Cell(currentRow, 3).Value = "'" + namelist.CertNo;
                    worksheet.Cell(currentRow, 4).Value = transporterdetails.BankName;
                    worksheet.Cell(currentRow, 5).Value = "'" + transporterdetails.AccNo;
                    worksheet.Cell(currentRow, 6).Value = transporterdetails.BBranch;
                    worksheet.Cell(currentRow, 7).Value = transporterdetails.NetPay;
                });
                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total";
                worksheet.Cell(currentRow, 7).Value = SNpay + TNpay;
                currentRow++;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = "Created By:";
                worksheet.Cell(currentRow, 4).Value = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = "Approved By:";
                worksheet.Cell(currentRow, 4).Value = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = "Signed By:";
                worksheet.Cell(currentRow, 4).Value = "______________________";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Bank Payroll List.xlsx");
                }
            }
        }
        public IActionResult suppliersPayrollExcel()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("dpayrollobj");

                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Payroll Report";
                var EndofPeriod = dpayrollobj.FirstOrDefault();
                worksheet.Cell(currentRow, 4).Value = EndofPeriod.EndofPeriod;
                
                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "IdNo";
                worksheet.Cell(currentRow, 4).Value = "Transport";
                worksheet.Cell(currentRow, 5).Value = "Agrovet";
                worksheet.Cell(currentRow, 6).Value = "Bonus";
                worksheet.Cell(currentRow, 7).Value = "Shares";
                worksheet.Cell(currentRow, 8).Value = "Advance";
                worksheet.Cell(currentRow, 9).Value = "midmonth";
                worksheet.Cell(currentRow, 10).Value = "Others";
                worksheet.Cell(currentRow, 11).Value = "Tractor";
                worksheet.Cell(currentRow, 12).Value = "Clinical";
                worksheet.Cell(currentRow, 13).Value = "Extension";
                worksheet.Cell(currentRow, 14).Value = "AI";
                worksheet.Cell(currentRow, 15).Value = "CurryForward";
                worksheet.Cell(currentRow, 16).Value = "SMS";
                worksheet.Cell(currentRow, 17).Value = "Loan";
                worksheet.Cell(currentRow, 18).Value = "TDeductions";
                worksheet.Cell(currentRow, 19).Value = "KgsSupplied";
                worksheet.Cell(currentRow, 20).Value = "GPay";
                worksheet.Cell(currentRow, 21).Value = "NPay";
                worksheet.Cell(currentRow, 22).Value = "Bank";
                worksheet.Cell(currentRow, 23).Value = "AccountNumber";
                worksheet.Cell(currentRow, 24).Value = "BBranch";
                worksheet.Cell(currentRow, 25).Value = "Station";

                decimal? Transport = 0;
                decimal? Agrovet = 0;
                decimal? Bonus = 0;
                decimal? Hshares = 0;
                decimal? Advance = 0;
                decimal? Midmonth = 0;
                decimal? Others = 0;
                decimal? Tractor = 0;
                decimal? CLINICAL = 0;
                decimal? extension = 0;
                decimal? AI = 0;
                decimal? CurryForward = 0;
                decimal? SMS = 0;
                decimal? Tdeductions = 0;
                double? KgsSupplied = 0;
                decimal? Gpay = 0;
                decimal? Npay = 0;
                decimal? loans = 0;
                foreach (var emp in dpayrollobj)
                {
                    Transport = dpayrollobj.Sum(k => k.Transport);
                    Agrovet = dpayrollobj.Sum(k => k.Agrovet);
                    Bonus = dpayrollobj.Sum(k => k.Bonus);
                    Hshares = dpayrollobj.Sum(k => k.Hshares);
                    Advance = dpayrollobj.Sum(k => k.Advance);
                    Midmonth = dpayrollobj.Sum(k => k.Midmonth);
                    Others = dpayrollobj.Sum(k => k.Others);
                    Tractor = dpayrollobj.Sum(k => k.Tractor);
                    CLINICAL = dpayrollobj.Sum(k => k.CLINICAL);
                    extension = dpayrollobj.Sum(k => k.extension);
                    AI = dpayrollobj.Sum(k => k.AI);
                    CurryForward = dpayrollobj.Sum(k => k.CurryForward);
                    SMS = dpayrollobj.Sum(k => k.SMS);
                    Tdeductions = dpayrollobj.Sum(k => k.Tdeductions);
                    KgsSupplied = dpayrollobj.Sum(k => k.KgsSupplied);
                    Gpay = dpayrollobj.Sum(k => k.Gpay);
                    Npay = dpayrollobj.Sum(k => k.Npay);
                    loans = dpayrollobj.Sum(k => k.Fsa);

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Sno;

                    //long.TryParse(emp.Sno, out long sno);

                    var TName = _context.DSuppliers.FirstOrDefault(u => u.Sno == emp.Sno && u.Scode == sacco);
                    worksheet.Cell(currentRow, 2).Value = TName.Names;
                    worksheet.Cell(currentRow, 3).Value = "'" + emp.IdNo;
                    worksheet.Cell(currentRow, 4).Value = emp.Transport;
                    worksheet.Cell(currentRow, 5).Value = emp.Agrovet;
                    worksheet.Cell(currentRow, 6).Value = emp.Bonus;
                    worksheet.Cell(currentRow, 7).Value = emp.Hshares;
                    worksheet.Cell(currentRow, 8).Value = emp.Advance;
                    worksheet.Cell(currentRow, 9).Value = emp.Midmonth;
                    worksheet.Cell(currentRow, 10).Value = emp.Others;
                    worksheet.Cell(currentRow, 11).Value = emp.Tractor;
                    worksheet.Cell(currentRow, 12).Value = emp.CLINICAL;
                    worksheet.Cell(currentRow, 13).Value = emp.extension;
                    worksheet.Cell(currentRow, 14).Value = emp.AI;
                    worksheet.Cell(currentRow, 15).Value = emp.CurryForward;
                    worksheet.Cell(currentRow, 16).Value = emp.SMS;
                    worksheet.Cell(currentRow, 17).Value = emp.Fsa;
                    worksheet.Cell(currentRow, 18).Value = emp.Tdeductions;
                    worksheet.Cell(currentRow, 19).Value = emp.KgsSupplied;
                    worksheet.Cell(currentRow, 20).Value = emp.Gpay;
                    worksheet.Cell(currentRow, 21).Value = emp.Npay;
                    worksheet.Cell(currentRow, 22).Value = emp.Bank;
                    worksheet.Cell(currentRow, 23).Value = "'" + emp.AccountNumber;
                    worksheet.Cell(currentRow, 24).Value = emp.Bbranch;
                    worksheet.Cell(currentRow, 25).Value = emp.Branch;
                }
                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total";
                worksheet.Cell(currentRow, 4).Value = Transport;
                worksheet.Cell(currentRow, 5).Value = Agrovet;
                worksheet.Cell(currentRow, 6).Value = Bonus;
                worksheet.Cell(currentRow, 7).Value = Hshares;
                worksheet.Cell(currentRow, 8).Value = Advance;
                worksheet.Cell(currentRow, 9).Value = Midmonth;
                worksheet.Cell(currentRow, 10).Value = Others;
                worksheet.Cell(currentRow, 11).Value = Tractor;
                worksheet.Cell(currentRow, 12).Value = CLINICAL;
                worksheet.Cell(currentRow, 13).Value = extension;
                worksheet.Cell(currentRow, 14).Value = AI;
                worksheet.Cell(currentRow, 15).Value = CurryForward;
                worksheet.Cell(currentRow, 16).Value = SMS;
                worksheet.Cell(currentRow, 17).Value = loans;
                worksheet.Cell(currentRow, 18).Value = Tdeductions;
                worksheet.Cell(currentRow, 19).Value = KgsSupplied;
                worksheet.Cell(currentRow, 20).Value = Gpay;
                worksheet.Cell(currentRow, 21).Value = Npay;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Payroll Report.xlsx");
                }
            }
        }
        public IActionResult SuppliersActiveExcel(FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            //var DateFro = Convert.ToDateTime(DateFrom.ToString());
            //var DateT = Convert.ToDateTime(DateTo.ToString());
            var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= filter.DateFrom && i.TransDate <= filter.DateTo)
                .Select(i => i.Sno);
            suppliersobj = _context.DSuppliers
                .Where(u => u.Scode == sacco && activeSuppliers.Contains(u.Sno.ToString()))
                .OrderBy(u => u.Sno);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("suppliersobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Active Suppliers Register";
                worksheet.Cell(currentRow, 3).Value = filter.DateTo;

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "RegDate";
                worksheet.Cell(currentRow, 4).Value = "IDNo";
                worksheet.Cell(currentRow, 5).Value = "Phone";
                worksheet.Cell(currentRow, 6).Value = "Bank";
                worksheet.Cell(currentRow, 7).Value = "AccNo";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                worksheet.Cell(currentRow, 9).Value = "Gender";
                worksheet.Cell(currentRow, 10).Value = "Village";
                worksheet.Cell(currentRow, 11).Value = "Location";
                worksheet.Cell(currentRow, 12).Value = "Division";
                worksheet.Cell(currentRow, 13).Value = "District";
                worksheet.Cell(currentRow, 14).Value = "County";
                worksheet.Cell(currentRow, 15).Value = "Active";
                worksheet.Cell(currentRow, 16).Value = "Address";
                worksheet.Cell(currentRow, 17).Value = "Station";
                int sum , sum2 = 0;
                sum2 = suppliersobj.Count();
                suppliersobj = suppliersobj.OrderBy(p => p.Branch).ToList();
                var branches = suppliersobj.GroupBy(b => b.Branch).ToList();
                branches.ForEach(s =>
                {
                    var branchname = s.FirstOrDefault();
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = branchname.Branch;
                    var suppliers = suppliersobj.Where(k => k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                    .OrderBy(h => h.Sno).ToList();
                    sum = suppliers.Count();
                    foreach (var emp in suppliers)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        worksheet.Cell(currentRow, 2).Value = emp.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                        worksheet.Cell(currentRow, 4).Value = emp.IdNo;
                        worksheet.Cell(currentRow, 5).Value = "'" + emp.PhoneNo;
                        worksheet.Cell(currentRow, 6).Value = emp.Bcode;
                        worksheet.Cell(currentRow, 7).Value = emp.AccNo;
                        worksheet.Cell(currentRow, 8).Value = emp.Bbranch;
                        worksheet.Cell(currentRow, 9).Value = emp.Type;
                        worksheet.Cell(currentRow, 10).Value = emp.Village;
                        worksheet.Cell(currentRow, 11).Value = emp.Location;
                        worksheet.Cell(currentRow, 12).Value = emp.Division;
                        worksheet.Cell(currentRow, 13).Value = emp.District;
                        worksheet.Cell(currentRow, 14).Value = emp.County;
                        worksheet.Cell(currentRow, 15).Value = emp.Active;
                        worksheet.Cell(currentRow, 16).Value = emp.Address;
                        worksheet.Cell(currentRow, 17).Value = emp.Branch;
                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "No of Suppliers:";
                    worksheet.Cell(currentRow, 2).Value = sum;
                });
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Total Suppliers:";
                worksheet.Cell(currentRow, 2).Value = sum2;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Active Suppliers Register.xlsx");
                }
            }
        }
        public IActionResult SuppliersInActiveExcel(FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            //var DateFro = Convert.ToDateTime(DateFrom.ToString());
            //var DateT = Convert.ToDateTime(DateTo.ToString());
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("suppliersobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "InActive Suppliers Register";
                worksheet.Cell(currentRow, 3).Value = filter.DateTo;

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "RegDate";
                worksheet.Cell(currentRow, 4).Value = "IDNo";
                worksheet.Cell(currentRow, 5).Value = "Phone";
                worksheet.Cell(currentRow, 6).Value = "Bank";
                worksheet.Cell(currentRow, 7).Value = "AccNo";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                worksheet.Cell(currentRow, 9).Value = "Gender";
                worksheet.Cell(currentRow, 10).Value = "Village";
                worksheet.Cell(currentRow, 11).Value = "Location";
                worksheet.Cell(currentRow, 12).Value = "Division";
                worksheet.Cell(currentRow, 13).Value = "District";
                worksheet.Cell(currentRow, 14).Value = "County";
                worksheet.Cell(currentRow, 15).Value = "Active";
                worksheet.Cell(currentRow, 16).Value = "Address";
                worksheet.Cell(currentRow, 17).Value = "Station";
                int sum = 0, sum2 = 0;
                sum2 = suppliersobj.Count();
                suppliersobj = suppliersobj.OrderBy(p => p.Branch).ToList();
                var branches = suppliersobj.GroupBy(b => b.Branch).ToList();
                branches.ForEach(s =>
                {
                    var branchname = s.FirstOrDefault();
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = branchname.Branch;
                    var suppliers = suppliersobj.Where(k => k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                    .OrderBy(h => h.Sno).ToList();
                    sum = suppliers.Count();
                    foreach (var emp in suppliers)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        worksheet.Cell(currentRow, 2).Value = emp.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                        worksheet.Cell(currentRow, 4).Value = emp.IdNo;
                        worksheet.Cell(currentRow, 5).Value = "'" + emp.PhoneNo;
                        worksheet.Cell(currentRow, 6).Value = emp.Bcode;
                        worksheet.Cell(currentRow, 7).Value = emp.AccNo;
                        worksheet.Cell(currentRow, 8).Value = emp.Bbranch;
                        worksheet.Cell(currentRow, 9).Value = emp.Type;
                        worksheet.Cell(currentRow, 10).Value = emp.Village;
                        worksheet.Cell(currentRow, 11).Value = emp.Location;
                        worksheet.Cell(currentRow, 12).Value = emp.Division;
                        worksheet.Cell(currentRow, 13).Value = emp.District;
                        worksheet.Cell(currentRow, 14).Value = emp.County;
                        worksheet.Cell(currentRow, 15).Value = emp.Active;
                        worksheet.Cell(currentRow, 16).Value = emp.Address;
                        worksheet.Cell(currentRow, 17).Value = emp.Branch;
                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "No of Suppliers:";
                    worksheet.Cell(currentRow, 2).Value = sum;
                });
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Total Suppliers:";
                worksheet.Cell(currentRow, 2).Value = sum2;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "InActive Suppliers Register.xlsx");
                }
            }
        }
       
        public IActionResult Excel()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("suppliersobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Register";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "RegDate";
                worksheet.Cell(currentRow, 4).Value = "IDNo";
                worksheet.Cell(currentRow, 5).Value = "Phone";
                worksheet.Cell(currentRow, 6).Value = "BBank";
                worksheet.Cell(currentRow, 7).Value = "AccNo";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                worksheet.Cell(currentRow, 9).Value = "Gender";
                worksheet.Cell(currentRow, 10).Value = "Village";
                worksheet.Cell(currentRow, 11).Value = "Location";
                worksheet.Cell(currentRow, 12).Value = "Ward";
                worksheet.Cell(currentRow, 13).Value = "Sub-County";
                worksheet.Cell(currentRow, 14).Value = "County";
                worksheet.Cell(currentRow, 15).Value = "Active";
                worksheet.Cell(currentRow, 16).Value = "Address";
                worksheet.Cell(currentRow, 17).Value = "Station";
                int sum=0, sum2 = 0;
                sum2 = suppliersobj.Count();
                suppliersobj = suppliersobj.OrderBy(p => p.Branch).ToList();
                var branches = suppliersobj.GroupBy(b => b.Branch).ToList();
                branches.ForEach(s =>
                {
                    var branchname = s.FirstOrDefault();
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = branchname.Branch;
                    var suppliers = suppliersobj.Where(k => k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                    .OrderBy(h=>h.Sno).ToList();
                    sum = suppliers.Count();
                    foreach (var emp in suppliers)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        worksheet.Cell(currentRow, 2).Value = emp.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                        worksheet.Cell(currentRow, 4).Value = emp.IdNo;
                        worksheet.Cell(currentRow, 5).Value = emp.PhoneNo;
                        worksheet.Cell(currentRow, 6).Value = emp.Bcode;
                        worksheet.Cell(currentRow, 7).Value = emp.AccNo;
                        worksheet.Cell(currentRow, 8).Value = emp.Bbranch;
                        worksheet.Cell(currentRow, 9).Value = emp.Type;
                        worksheet.Cell(currentRow, 10).Value = emp.Village;
                        worksheet.Cell(currentRow, 11).Value = emp.Location;
                        worksheet.Cell(currentRow, 12).Value = emp.Division;
                        worksheet.Cell(currentRow, 13).Value = emp.District;
                        worksheet.Cell(currentRow, 14).Value = emp.County;
                        worksheet.Cell(currentRow, 15).Value = emp.Active;
                        worksheet.Cell(currentRow, 16).Value = emp.Address;
                        worksheet.Cell(currentRow, 17).Value = emp.Branch;

                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "No of Suppliers:";
                    worksheet.Cell(currentRow, 2).Value = sum;
                });
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Total Suppliers:";
                worksheet.Cell(currentRow, 2).Value = sum2;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Register.xlsx");
                }
            }
        }
        public IActionResult IntakeExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Intake Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Qsupplied";
                worksheet.Cell(currentRow, 6).Value = "Price";
                worksheet.Cell(currentRow, 7).Value = "Description";
                decimal sum = 0;
                foreach (var emp in productIntakeobj)
                {
                    var checkifexist = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                    if (checkifexist.Any())
                    {


                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;

                        var TName = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                        foreach (var al in TName)
                            worksheet.Cell(currentRow, 2).Value = al.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                        worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                        worksheet.Cell(currentRow, 5).Value = emp.Qsupplied;
                        worksheet.Cell(currentRow, 6).Value = emp.Ppu;
                        worksheet.Cell(currentRow, 7).Value = emp.Description;
                        sum += (emp.Qsupplied);
                    }
                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Kgs";
                worksheet.Cell(currentRow, 5).Value = sum;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Intake.xlsx");
                }
            }
        }
        public async Task<IActionResult> DSumarryIntakeExcel(DateTime DateFrom, DateTime DateTo, string Branch)
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var DateFro = Convert.ToDateTime(DateFrom.ToString());
                var DateT = Convert.ToDateTime(DateTo.ToString());
                var Branchh = Branch;
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = await _context.DCompanies.Where(u => u.Name == sacco).ToListAsync();
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Summary Intake Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "IDNo";
                worksheet.Cell(currentRow, 4).Value = "1";
                worksheet.Cell(currentRow, 5).Value = "2";
                worksheet.Cell(currentRow, 6).Value = "3";
                worksheet.Cell(currentRow, 7).Value = "4";
                worksheet.Cell(currentRow, 8).Value = "5";
                worksheet.Cell(currentRow, 9).Value = "6";
                worksheet.Cell(currentRow, 10).Value = "7";
                worksheet.Cell(currentRow, 11).Value = "8";
                worksheet.Cell(currentRow, 12).Value = "9";
                worksheet.Cell(currentRow, 13).Value = "10";
                worksheet.Cell(currentRow, 14).Value = "11";
                worksheet.Cell(currentRow, 15).Value = "12";
                worksheet.Cell(currentRow, 16).Value = "13";
                worksheet.Cell(currentRow, 17).Value = "14";
                worksheet.Cell(currentRow, 18).Value = "15";
                worksheet.Cell(currentRow, 19).Value = "16";
                worksheet.Cell(currentRow, 20).Value = "17";
                worksheet.Cell(currentRow, 21).Value = "18";
                worksheet.Cell(currentRow, 22).Value = "19";
                worksheet.Cell(currentRow, 23).Value = "20";
                worksheet.Cell(currentRow, 24).Value = "21";
                worksheet.Cell(currentRow, 25).Value = "22";
                worksheet.Cell(currentRow, 26).Value = "23";
                worksheet.Cell(currentRow, 27).Value = "24";
                worksheet.Cell(currentRow, 28).Value = "25";
                worksheet.Cell(currentRow, 29).Value = "26";
                worksheet.Cell(currentRow, 30).Value = "27";
                worksheet.Cell(currentRow, 31).Value = "28";
                worksheet.Cell(currentRow, 32).Value = "29";
                worksheet.Cell(currentRow, 33).Value = "30";
                worksheet.Cell(currentRow, 34).Value = "31";
                worksheet.Cell(currentRow, 35).Value = "Total";
                worksheet.Cell(currentRow, 36).Value = "Branch";
                decimal sum = 0, sum1 = 0;
                foreach (var branch in branchobj)
                {
                    suppliersobj = await _context.DSuppliers.Where(u => u.Scode == sacco && u.Branch == branch.Bname).OrderBy(k => k.Sno).ToListAsync();
                    foreach (var sup in suppliersobj)
                    {
                        var supExist = await _context.ProductIntake
                        .AnyAsync(i => i.SaccoCode == sacco && i.Sno == sup.Sno.ToString() && i.TransDate >= DateFro
                        && i.Qsupplied != 0 && i.TransDate <= DateT && i.Branch == branch.Bname && i.Description != "Transport");
                        if (supExist)
                        {
                            var startdate = DateFro;
                            //productIntakeobj = _context.ProductIntake
                            //.Where(i => i.SaccoCode == sacco && i.Sno==sup.Sno.ToString() && i.TransDate >= DateFro && i.Qsupplied != 0 && i.TransDate <= DateT && i.Branch == branch.Bname);

                            //long.TryParse(sup.Sno, out long sno);
                            var TransporterExist = await _context.DSuppliers.AnyAsync(u => u.Scode == sacco && u.Sno == sup.Sno);
                            if (TransporterExist)
                            {

                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = sup.Sno;
                                worksheet.Cell(currentRow, 2).Value = sup.Names;
                                worksheet.Cell(currentRow, 3).Value = sup.IdNo;
                                while (startdate <= DateT)
                                {
                                    var datereceived = startdate;
                                    var sumkgs = _context.ProductIntake
                            .Where(i => i.SaccoCode == sacco && i.Sno == sup.Sno.ToString() && i.TransDate >= datereceived
                            && i.Qsupplied != 0 && i.TransDate <= datereceived && i.Branch == branch.Bname && i.Description != "Transport").Sum(s => s.Qsupplied);
                                    var datereceive = startdate.Day;

                                    if (datereceive == 1)
                                        worksheet.Cell(currentRow, 4).Value = sumkgs;
                                    if (datereceive == 2)
                                        worksheet.Cell(currentRow, 5).Value = sumkgs;
                                    if (datereceive == 3)
                                        worksheet.Cell(currentRow, 6).Value = sumkgs;
                                    if (datereceive == 4)
                                        worksheet.Cell(currentRow, 7).Value = sumkgs;
                                    if (datereceive == 5)
                                        worksheet.Cell(currentRow, 8).Value = sumkgs;
                                    if (datereceive == 6)
                                        worksheet.Cell(currentRow, 9).Value = sumkgs;
                                    if (datereceive == 7)
                                        worksheet.Cell(currentRow, 10).Value = sumkgs;
                                    if (datereceive == 8)
                                        worksheet.Cell(currentRow, 11).Value = sumkgs;
                                    if (datereceive == 9)
                                        worksheet.Cell(currentRow, 12).Value = sumkgs;
                                    if (datereceive == 10)
                                        worksheet.Cell(currentRow, 13).Value = sumkgs;
                                    if (datereceive == 11)
                                        worksheet.Cell(currentRow, 14).Value = sumkgs;
                                    if (datereceive == 12)
                                        worksheet.Cell(currentRow, 15).Value = sumkgs;
                                    if (datereceive == 13)
                                        worksheet.Cell(currentRow, 16).Value = sumkgs;
                                    if (datereceive == 14)
                                        worksheet.Cell(currentRow, 17).Value = sumkgs;
                                    if (datereceive == 15)
                                        worksheet.Cell(currentRow, 18).Value = sumkgs;
                                    if (datereceive == 16)
                                        worksheet.Cell(currentRow, 19).Value = sumkgs;
                                    if (datereceive == 17)
                                        worksheet.Cell(currentRow, 20).Value = sumkgs;
                                    if (datereceive == 18)
                                        worksheet.Cell(currentRow, 21).Value = sumkgs;
                                    if (datereceive == 19)
                                        worksheet.Cell(currentRow, 22).Value = sumkgs;
                                    if (datereceive == 20)
                                        worksheet.Cell(currentRow, 23).Value = sumkgs;
                                    if (datereceive == 21)
                                        worksheet.Cell(currentRow, 24).Value = sumkgs;
                                    if (datereceive == 22)
                                        worksheet.Cell(currentRow, 25).Value = sumkgs;
                                    if (datereceive == 23)
                                        worksheet.Cell(currentRow, 26).Value = sumkgs;
                                    if (datereceive == 24)
                                        worksheet.Cell(currentRow, 27).Value = sumkgs;
                                    if (datereceive == 25)
                                        worksheet.Cell(currentRow, 28).Value = sumkgs;
                                    if (datereceive == 26)
                                        worksheet.Cell(currentRow, 29).Value = sumkgs;
                                    if (datereceive == 27)
                                        worksheet.Cell(currentRow, 30).Value = sumkgs;
                                    if (datereceive == 28)
                                        worksheet.Cell(currentRow, 31).Value = sumkgs;
                                    if (datereceive == 29)
                                        worksheet.Cell(currentRow, 32).Value = sumkgs;
                                    if (datereceive == 30)
                                        worksheet.Cell(currentRow, 33).Value = sumkgs;
                                    if (datereceive == 31)
                                        worksheet.Cell(currentRow, 34).Value = sumkgs;
                                    sum += sumkgs;
                                    sum1 += sumkgs;
                                    startdate = startdate.AddDays(1);
                                }
                                worksheet.Cell(currentRow, 35).Value = sum1;
                                worksheet.Cell(currentRow, 36).Value = sup.Branch;
                                sum1 = 0;
                            }
                        }
                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 4).Value = "Branch Kgs";
                    worksheet.Cell(currentRow, 5).Value = sum1;

                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Kgs";
                worksheet.Cell(currentRow, 5).Value = sum;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Summary Intake.xlsx");
                }
            }
        }
        public async Task<IActionResult> TransportersBalancingExcel(DateTime DateFrom, DateTime DateTo, string Transporter)
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var DateFro = Convert.ToDateTime(DateFrom.ToString());
                var DateT = Convert.ToDateTime(DateTo.ToString());
                var Transporters = Transporter;
                var worksheet = workbook.Worksheets.Add("TransportersBalancingobj");
                var currentRow = 1;
                companyobj = await _context.DCompanies.Where(u => u.Name == sacco).ToListAsync();
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Transporter Balancing Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "TCode";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Date";
                worksheet.Cell(currentRow, 4).Value = "Quantity";
                worksheet.Cell(currentRow, 5).Value = "ActualBal";
                worksheet.Cell(currentRow, 6).Value = "Rejects";
                worksheet.Cell(currentRow, 7).Value = "Spillage";
                worksheet.Cell(currentRow, 8).Value = "Varriance";
                decimal sum = 0, sum1 = 0;
                foreach (var transporters in transporterobj)
                {
                    TransportersBalancingobj = await _context.TransportersBalancings
                    .Where(u => u.Code == sacco && u.Transporter == Transporters && u.Date >= DateFro && u.Date <= DateT)
                    .OrderBy(k => k.Transporter).ToListAsync();
                    foreach (var sup in TransportersBalancingobj)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = sup.Transporter;
                        worksheet.Cell(currentRow, 2).Value = transporters.TransName;
                        worksheet.Cell(currentRow, 3).Value = sup.Date;
                        worksheet.Cell(currentRow, 4).Value = sup.Quantity;
                        worksheet.Cell(currentRow, 5).Value = sup.ActualBal;
                        worksheet.Cell(currentRow, 6).Value = sup.Rejects;
                        worksheet.Cell(currentRow, 7).Value = sup.Spillage;
                        worksheet.Cell(currentRow, 8).Value = sup.Varriance;
                        sum1 += sum1 + Convert.ToDecimal(sup.Quantity);
                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 3).Value = "Total Kgs";
                    worksheet.Cell(currentRow, 4).Value = sum1;
                }
                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total Kgs";
                worksheet.Cell(currentRow, 4).Value = sum;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transporter Balancing Report.xlsx");
                }
            }
        }
        public async Task<IActionResult> DispatchBalancingExcel(DateTime DateFrom, DateTime DateTo, string Transporter)
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var DateFro = Convert.ToDateTime(DateFrom.ToString());
                var DateT = Convert.ToDateTime(DateTo.ToString());
                var Transporters = Transporter;
                var worksheet = workbook.Worksheets.Add("DispatchBalancingobj");
                var currentRow = 1;
                companyobj = await _context.DCompanies.Where(u => u.Name == sacco).ToListAsync();
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Dispatch Balancing Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "Date";
                worksheet.Cell(currentRow, 2).Value = "Intake";
                worksheet.Cell(currentRow, 3).Value = "BF";
                worksheet.Cell(currentRow, 4).Value = "Actuals";
                worksheet.Cell(currentRow, 5).Value = "Dispatch";
                worksheet.Cell(currentRow, 6).Value = "Spillage";
                worksheet.Cell(currentRow, 7).Value = "Rejects";
                worksheet.Cell(currentRow, 8).Value = "CF";
                worksheet.Cell(currentRow, 9).Value = "Varriance";
                decimal sum = 0, sum1 = 0;
                foreach (var transporters in DispatchBalancingobj)
                {
                    DispatchBalancingobj = await _context.DispatchBalancing
                    .Where(u => u.Saccocode == sacco && u.Date >= DateFro && u.Date <= DateT)
                    .OrderBy(k => k.Date).ToListAsync();
                    foreach (var sup in DispatchBalancingobj)
                    {// Date, Intake, Dispatch, CF, BF, Actuals, Spillage, Rejects, Varriance
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = sup.Date;
                        worksheet.Cell(currentRow, 2).Value = sup.Intake;
                        worksheet.Cell(currentRow, 3).Value = sup.BF;
                        worksheet.Cell(currentRow, 4).Value = sup.Actuals;
                        worksheet.Cell(currentRow, 5).Value = sup.Dispatch;
                        worksheet.Cell(currentRow, 6).Value = sup.Spillage;
                        worksheet.Cell(currentRow, 7).Value = sup.Rejects;
                        worksheet.Cell(currentRow, 8).Value = sup.BF;
                        worksheet.Cell(currentRow, 9).Value = sup.Varriance;
                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 3).Value = "Total Kgs";
                    worksheet.Cell(currentRow, 4).Value = sum1;
                }
                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total Kgs";
                worksheet.Cell(currentRow, 4).Value = sum;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Dispatch Balancing Report.xlsx");
                }
            }
        }
        public IActionResult CorrectionIntakeExcel(DateTime DateFrom, DateTime DateTo, string Branch)
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var DateFro = Convert.ToDateTime(DateFrom.ToString());
                var DateT = Convert.ToDateTime(DateTo.ToString());
                var Branchh = Branch;
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Intake Correction Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Qsupplied";
                worksheet.Cell(currentRow, 6).Value = "Price";
                worksheet.Cell(currentRow, 7).Value = "Description";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                decimal sum = 0, sum1 = 0;

                productIntakeobj = _context.ProductIntake
                    .Where(i => i.SaccoCode == sacco && i.TransDate >= DateFro && i.Qsupplied != 0
                    && i.TransDate <= DateT && i.TransactionType == TransactionType.Correction && i.Description != "Transport")
                    .OrderBy(i => i.Sno);
                foreach (var emp in productIntakeobj)
                {
                    var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                    if (TransporterExist > 0)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        var TName = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                        foreach (var al in TName)
                            worksheet.Cell(currentRow, 2).Value = al.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                        worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                        worksheet.Cell(currentRow, 5).Value = emp.Qsupplied;
                        worksheet.Cell(currentRow, 6).Value = emp.Ppu;
                        worksheet.Cell(currentRow, 7).Value = emp.Description;
                        worksheet.Cell(currentRow, 8).Value = emp.Branch;
                        sum += (emp.Qsupplied);
                        sum1 += (emp.Qsupplied);
                    }
                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Branch Kgs";
                worksheet.Cell(currentRow, 5).Value = sum1;
                sum1 = 0;

                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Kgs";
                worksheet.Cell(currentRow, 5).Value = sum;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Correction Intake.xlsx");
                }
            }
        }
        public IActionResult BranchIntakeExcel(DateTime DateFrom, DateTime DateTo, string Branch)
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var DateFro = Convert.ToDateTime(DateFrom.ToString());
                var DateT = Convert.ToDateTime(DateTo.ToString());
                var Branchh = Branch;
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Branch Intake Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Qsupplied";
                worksheet.Cell(currentRow, 6).Value = "Price";
                worksheet.Cell(currentRow, 7).Value = "Description";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                decimal sum = 0, sum1 = 0;
                foreach (var branch in branchobj)
                {
                    productIntakeobj = _context.ProductIntake
                        .Where(i => i.SaccoCode == sacco && i.TransDate >= DateFro && i.Qsupplied != 0
                        && i.TransDate <= DateT && i.Branch == branch.Bname && i.Description != "Transport")
                        .OrderByDescending(i => i.Auditdatetime);
                    foreach (var emp in productIntakeobj)
                    {
                        var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                        if (TransporterExist > 0)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = emp.Sno;
                            var TName = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                            foreach (var al in TName)
                                worksheet.Cell(currentRow, 2).Value = al.Names;
                            worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                            worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                            worksheet.Cell(currentRow, 5).Value = emp.Qsupplied;
                            worksheet.Cell(currentRow, 6).Value = emp.Ppu;
                            worksheet.Cell(currentRow, 7).Value = emp.Description;
                            worksheet.Cell(currentRow, 8).Value = emp.Branch;
                            sum += (emp.Qsupplied);
                            sum1 += (emp.Qsupplied);
                        }
                    }
                    currentRow++;
                    worksheet.Cell(currentRow, 4).Value = "Branch Kgs";
                    worksheet.Cell(currentRow, 5).Value = sum1;
                    sum1 = 0;
                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Kgs";
                worksheet.Cell(currentRow, 5).Value = sum;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Branch Intake.xlsx");
                }
            }
        }
        public IActionResult BranchIntakeAuditExcel(DateTime DateFrom, DateTime DateTo, string Branch)
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var DateFro = Convert.ToDateTime(DateFrom.ToString());
                var DateT = Convert.ToDateTime(DateTo.ToString());
                var Branchh = Branch;
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Suppliers Branch Intake Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Qsupplied";
                worksheet.Cell(currentRow, 6).Value = "Price";
                worksheet.Cell(currentRow, 7).Value = "Description";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                worksheet.Cell(currentRow, 9).Value = "User";
                worksheet.Cell(currentRow, 10).Value = "AudiTime";
                decimal sum = 0, sum1 = 0,sum2=0;
                foreach (var branch in branchobj)
                {
                    productIntakeobj = _context.ProductIntake
                        .Where(i => i.SaccoCode == sacco && i.TransDate >= DateFro && i.Qsupplied != 0
                        && i.TransDate <= DateT && i.Branch == branch.Bname && i.Description!= "Transport")
                        .OrderByDescending(i => i.TransDate);
                    var audituser = productIntakeobj.GroupBy(m => m.AuditId).ToList();
                    audituser.ForEach(l => {
                        var branchuser = l.FirstOrDefault();
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = branchuser.AuditId;
                        var groupbydate = productIntakeobj.Where(k => k.AuditId == branchuser.AuditId)
                        .GroupBy(p=>p.TransDate).ToList();
                        groupbydate.ForEach(j => {
                            var TransDate = j.FirstOrDefault();

                            productIntakeobj = productIntakeobj.Where(k=>k.AuditId== branchuser.AuditId && k.TransDate== TransDate.TransDate)
                        .OrderByDescending(i => i.Auditdatetime).ToList();
                        foreach (var emp in productIntakeobj)
                        {
                            var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                            if (TransporterExist > 0)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = emp.Sno;
                                var TName = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                                foreach (var al in TName)
                                    worksheet.Cell(currentRow, 2).Value = al.Names;
                                worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                                worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                                worksheet.Cell(currentRow, 5).Value = emp.Qsupplied;
                                worksheet.Cell(currentRow, 6).Value = emp.Ppu;
                                worksheet.Cell(currentRow, 7).Value = emp.Description;
                                worksheet.Cell(currentRow, 8).Value = emp.Branch;
                                worksheet.Cell(currentRow, 9).Value = emp.AuditId;
                                worksheet.Cell(currentRow, 10).Value = emp.Auditdatetime;
                                sum += (emp.Qsupplied);
                                sum1 += (emp.Qsupplied);
                                sum2 += (emp.Qsupplied);
                            }
                        }
                        });
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = branchuser.AuditId+" "+"Total Kgs";
                        worksheet.Cell(currentRow, 5).Value = sum2;
                        sum2 = 0;
                    });
                    currentRow++;
                    worksheet.Cell(currentRow, 4).Value = "Branch Kgs";
                    worksheet.Cell(currentRow, 5).Value = sum1;
                    sum1 = 0;
                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Kgs";
                worksheet.Cell(currentRow, 5).Value = sum;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "IntakeAuditReport.xlsx");
                }
            }
        }
        public IActionResult TIntakeExcel(DateTime DateFrom, DateTime DateTo)
        {
            var DateFro = Convert.ToDateTime(DateFrom.ToString());
            var DateT = Convert.ToDateTime(DateTo.ToString());
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                worksheet.Cell(currentRow, 2).Value = "Transporter Intake Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "TransCode";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "SNO";
                worksheet.Cell(currentRow, 6).Value = "Qsupplied";
                worksheet.Cell(currentRow, 7).Value = "Price";
                worksheet.Cell(currentRow, 8).Value = "Description";
                decimal sum = 0, totalkg = 0;

                foreach (var emp in transporterobj)
                {
                    var TransporterExist = _context.ProductIntake.Any(u => u.Sno == emp.TransCode && u.TransDate >= DateFro && u.TransDate <= DateT && u.SaccoCode == sacco);
                    if (TransporterExist)
                    {
                        productIntakeobj = _context.ProductIntake.Where(u => u.Sno == emp.TransCode && u.TransDate >= DateFro && u.TransDate <= DateT && u.Qsupplied != 0 && u.SaccoCode == sacco);
                        foreach (var empintake in productIntakeobj)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = empintake.Sno;
                            var transporters = _context.DTransporters.FirstOrDefault(u => u.TransCode == empintake.Sno && u.ParentT == sacco && u.Tbranch == saccoBranch);
                            var intake = _context.ProductIntake.FirstOrDefault(i => i.TransDate == empintake.TransDate
                            && i.TransTime == empintake.TransTime && i.SaccoCode == sacco && i.Branch == saccoBranch
                            && i.Sno != empintake.Sno);
                            worksheet.Cell(currentRow, 2).Value = transporters?.TransName ?? "";
                            worksheet.Cell(currentRow, 3).Value = empintake.TransDate;
                            worksheet.Cell(currentRow, 4).Value = empintake.ProductType;
                            worksheet.Cell(currentRow, 5).Value = intake.Sno;
                            worksheet.Cell(currentRow, 6).Value = empintake.Qsupplied;
                            worksheet.Cell(currentRow, 7).Value = empintake.Ppu;
                            worksheet.Cell(currentRow, 8).Value = empintake.Description;
                            sum += (empintake.Qsupplied);
                            totalkg += (empintake.Qsupplied);
                        }
                        currentRow++;
                        worksheet.Cell(currentRow, 5).Value = "Total Kgs";
                        worksheet.Cell(currentRow, 6).Value = totalkg;
                        totalkg = 0;
                    }
                }
                currentRow++;
                worksheet.Cell(currentRow, 5).Value = "Total Kgs";
                worksheet.Cell(currentRow, 6).Value = sum;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transporter Intake.xlsx");
                }
            }
        }
        public IActionResult DeductionsExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                var enddate = productIntakeobj.FirstOrDefault();
                worksheet.Cell(currentRow, 2).Value = "Suppliers Deductions Report For: "+ enddate.TransDate.ToString("dd/MM/yyy");

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Remarks";
                worksheet.Cell(currentRow, 7).Value = "Station";
                decimal? sum2 = 0;
                productIntakeobj = productIntakeobj.OrderBy(p => p.Branch).ToList();
                var branches = productIntakeobj.GroupBy(b => b.Branch).ToList();
                branches.ForEach(s =>
                {
                    
                    var branchname = s.FirstOrDefault();
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = branchname.Branch;
                    
                    var supplierslist = productIntakeobj.Where(k => k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                    .OrderBy(h => h.Sno).ToList();
                    var dedutciontype = supplierslist.GroupBy(d=>d.ProductType).ToList();
                    dedutciontype.ForEach(w => {
                        var deduction = w.FirstOrDefault();
                        decimal? sum = 0;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = deduction.ProductType;
                        var suppliers = supplierslist.Where(r=>r.ProductType.ToUpper().Equals(deduction.ProductType.ToUpper()));
                        foreach (var emp in suppliers)
                        {
                            var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                            if (TransporterExist > 0)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = emp.Sno;
                                var TName = _context.DSuppliers.FirstOrDefault(u => u.Sno == emp.Sno && u.Scode == sacco);
                                worksheet.Cell(currentRow, 2).Value = TName.Names;
                                worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                                worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                                worksheet.Cell(currentRow, 5).Value = emp.DR;
                                worksheet.Cell(currentRow, 6).Value = emp.Remarks;
                                worksheet.Cell(currentRow, 7).Value = emp.Branch;
                                sum += emp.DR;

                            }
                        }

                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = "Total "+ deduction.ProductType  + " Amount: ";
                        worksheet.Cell(currentRow, 2).Value = sum;
                        sum2 += sum;
                    });
               
            });
            currentRow++;
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = "Grand Total Amount: ";
            worksheet.Cell(currentRow, 2).Value = sum2;
            using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Deductions.xlsx");
                }
            }
        }
        public IActionResult TDeductionsExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var worksheet = workbook.Worksheets.Add("productIntakeobj");
                var currentRow = 1;
                companyobj = _context.DCompanies.Where(u => u.Name == sacco);
                foreach (var emp in companyobj)
                {
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Adress;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Town;
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = emp.Email;
                }
                currentRow = 5;
                var enddate = productIntakeobj.FirstOrDefault();
                worksheet.Cell(currentRow, 2).Value = "Transporters Deductions Report For: "+ enddate.TransDate.ToString("dd/MM/yyy");

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "TCode";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Remarks";
                worksheet.Cell(currentRow, 7).Value = "Station";
                decimal? sum =0;

                productIntakeobj = productIntakeobj.OrderBy(p => p.Branch).ToList();
                var branches = productIntakeobj.GroupBy(b => b.Branch).ToList();
                branches.ForEach(s => {

                    
                    var branchname = s.FirstOrDefault();
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = branchname.Branch;
                    var transporterslist = productIntakeobj.Where(k => k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                    .OrderBy(h => h.Sno).ToList();
                    var dedutciontype = transporterslist.GroupBy(d => d.ProductType).ToList();
                    dedutciontype.ForEach(w =>
                    {
                        decimal? sum1 = 0;
                        var deduction = w.FirstOrDefault();
                        decimal? sum = 0;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = deduction.ProductType;
                        var transporters = transporterslist.Where(r => r.ProductType.ToUpper().Equals(deduction.ProductType.ToUpper()));
                        foreach (var emp in transporters)
                        {
                            var TransporterExist = _context.DTransporters.Where(u => u.TransCode == emp.Sno).Count();
                            if (TransporterExist > 0)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Value = emp.Sno;
                                var TName = _context.DTransporters.FirstOrDefault(u => u.TransCode == emp.Sno && u.ParentT == sacco);
                                worksheet.Cell(currentRow, 2).Value = TName.TransName;
                                worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                                worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                                worksheet.Cell(currentRow, 5).Value = emp.DR;
                                worksheet.Cell(currentRow, 6).Value = emp.Remarks.ToString();
                                worksheet.Cell(currentRow, 7).Value = emp.Branch;
                                sum1 += emp.DR;
                            }
                        }

                        currentRow++;
                        worksheet.Cell(currentRow, 4).Value = "Total " + deduction.ProductType + " Amount: ";
                        worksheet.Cell(currentRow, 5).Value = sum1;
                        sum += sum1;
                    });
                        
                });
                
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Amount";
                worksheet.Cell(currentRow, 5).Value = sum;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Transporters Deductions.xlsx");
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportAllSuppliers(string County)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("suppliersobj");
                var currentRow = 1;
                worksheet.Cell(currentRow, 2).Value = "Coperative Suppliers List";

                currentRow = 2;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "RegDate";
                worksheet.Cell(currentRow, 4).Value = "IDNo";
                worksheet.Cell(currentRow, 5).Value = "Phone";
                worksheet.Cell(currentRow, 6).Value = "Bank";
                worksheet.Cell(currentRow, 7).Value = "AccNo";
                worksheet.Cell(currentRow, 8).Value = "Branch";
                worksheet.Cell(currentRow, 9).Value = "Gender";
                worksheet.Cell(currentRow, 10).Value = "Village";
                worksheet.Cell(currentRow, 11).Value = "Location";
                worksheet.Cell(currentRow, 12).Value = "Ward";
                worksheet.Cell(currentRow, 13).Value = "Sub-County";
                worksheet.Cell(currentRow, 14).Value = "County";
                worksheet.Cell(currentRow, 15).Value = "Active";
                worksheet.Cell(currentRow, 16).Value = "Address";
                worksheet.Cell(currentRow, 17).Value = "Corperatives";

                var dSuppliers = _context.DSuppliers.ToList();
                if (!string.IsNullOrEmpty(County))
                    dSuppliers = dSuppliers.Where(s => s.County.ToUpper().Equals(County.ToUpper())).ToList();
                var suppliers = dSuppliers.GroupBy(s => s.Scode).ToList();
                suppliers.ForEach(s =>
                {
                    var company = _context.DCompanies.FirstOrDefault(c => c.Name == s.Key);
                    if (company != null)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = company.Name;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = company.Adress;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = company.Town;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = company.Email;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = company.PhoneNo;
                    }
                    foreach (var emp in s)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        worksheet.Cell(currentRow, 2).Value = emp.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                        worksheet.Cell(currentRow, 4).Value = emp.IdNo;
                        worksheet.Cell(currentRow, 5).Value = emp.PhoneNo;
                        worksheet.Cell(currentRow, 6).Value = emp.Bcode;
                        worksheet.Cell(currentRow, 7).Value = emp.AccNo;
                        worksheet.Cell(currentRow, 8).Value = emp.Bbranch;
                        worksheet.Cell(currentRow, 9).Value = emp.Type;
                        worksheet.Cell(currentRow, 10).Value = emp.Village;
                        worksheet.Cell(currentRow, 11).Value = emp.Location;
                        worksheet.Cell(currentRow, 12).Value = emp.Division;
                        worksheet.Cell(currentRow, 13).Value = emp.District;
                        worksheet.Cell(currentRow, 14).Value = emp.County;
                        worksheet.Cell(currentRow, 15).Value = emp.Active;
                        worksheet.Cell(currentRow, 16).Value = emp.Address;
                        worksheet.Cell(currentRow, 17).Value = emp.Scode;
                    }
                });

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Suppliers Register.xlsx");
                }
            }
        }
    }
}
