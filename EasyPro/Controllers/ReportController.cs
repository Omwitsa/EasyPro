using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Repository;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.TransportersVM;
using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using Syncfusion.EJ2.Diagrams;
using Syncfusion.EJ2.Linq;
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadTransportersBalReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }

        public IActionResult TestReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var webReport = new WebReport();
            webReport.Report.Load(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "report.frx"));

            return View(webReport);
        }

        [HttpGet]//DownloadDispatchBalReport
        public IActionResult DownloadDispatchBalReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadTReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadPReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        [HttpGet]
        public IActionResult DownloadActiveSuppliersPReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        public IActionResult FlmdData(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);

            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(0);
            var endDate = month.AddMonths(1).AddDays(-1);
            string sno = null;
            var flmdData = Gedflmdfarmers(startDate, endDate, sno);
            return View(new FlmdDataVM { farmerdetails = flmdData.OrderBy(m => m.Sno) });
        }
        [HttpGet]
        public JsonResult Selectemaxloan(string sno)
        {
            utilities.SetUpPrivileges(this);

            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(0);
            var endDate = month.AddMonths(1).AddDays(-1);
            var flmdData = Gedflmdfarmers(startDate, endDate, sno);
            return Json(flmdData.Sum(b => b.Total));
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
                        land = (decimal)((double)(flmdland.TotalAcres ?? 0.0 )* 1200000);
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            suppliersobj = _context.DSuppliers
                .Where(p => p.Scode == sacco)
                .OrderBy(s => s.Sno);
            return FlmdDataDownloadExcel();
        }
        public IActionResult FlmdDataDownloadExcel()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
                var flmdData = Gedflmdfarmers(startDate, endDate, sno);
                decimal totals = 0;
                flmdData.ForEach(c =>
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 1).Value = c.Sno;
                    worksheet.Cell(currentRow, 2).Value = c.Name;
                    worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 3).Value = c.Phone;
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        public IActionResult DownloadSalesReport()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            ViewBag.isSlopes = StrValues.Slopes == sacco;
            var zones = _context.Zones.Where(z => z.Code== sacco).Select(z => z.Name).ToList();
            ViewBag.zones = new SelectList(zones);
            var Transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                Transporters = Transporters.Where(i => i.Tbranch == saccoBranch).ToList();
            ViewBag.Transporters = new SelectList(Transporters, "TransName", "TransName");

            var county = _context.DCompanies.ToList();
            ViewBag.county = new SelectList(county.OrderBy(m => m.Province).Select(h => h.Province).Distinct()); 

            var Transportersdetails = _context.DTransporters.Where(i => i.ParentT == sacco).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                Transportersdetails = Transportersdetails.Where(i => i.Tbranch == saccoBranch).ToList();
            ViewBag.Transportersdetails = Transportersdetails;

            var bankNames = _context.DBanks.Where(b => b.BankCode == sacco)
                .OrderBy(b => b.BankName).Select(b => b.BankName);
            ViewBag.bankname = new SelectList(bankNames);

            var deductions = _context.DDcodes.Where(d => d.Dcode == sacco)
                .Select(d => d.Description).ToList();
            ViewBag.deductions = new SelectList(deductions);
        }
        [HttpPost]
        public IActionResult Suppliers([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            suppliersobj = _context.DSuppliers.Where(u => u.Scode == sacco);
            return Excel();
        }
        [HttpPost]
        public IActionResult SuppliersPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            transporterobj = _context.DTransporters.Where(u => u.ParentT == sacco);
            return TransporterExcel();
        }
        [HttpPost]
        public IActionResult TransporterPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            filter.BankName = filter?.BankName ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var dpayrollobj = _context.DPayrolls
                .Where(p => p.EndofPeriod >= filter.DateFrom && p.EndofPeriod <= filter.DateTo 
                && p.SaccoCode == sacco && p.Bank.ToUpper().Equals(filter.BankName.ToUpper())).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                dpayrollobj = dpayrollobj.Where(i => i.Branch == saccoBranch).ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"{filter.BankName} Payroll");
                var currentRow = 1;
                var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
                worksheet.Cell(currentRow, 2).Value = company.Name;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = company.Adress;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = company.Town;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = company.Email;
                currentRow++;

                var EndofPeriod = dpayrollobj.FirstOrDefault()?.EndofPeriod ?? filter.DateTo;
                worksheet.Cell(currentRow, 2).Value = filter.BankName.ToUpper() + " Payroll List For:";
                worksheet.Cell(currentRow, 4).Value = EndofPeriod;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "AccountNumber";
                worksheet.Cell(currentRow, 4).Value = "NPay";

                decimal? SNpay = 0, TNpay = 0;

                var gettransporterspayrolllist = _context.DTransportersPayRolls
                .Where(p => p.SaccoCode == sacco && p.BankName.ToUpper().Equals(filter.BankName.ToUpper()) && p.NetPay > 0 && p.EndPeriod == EndofPeriod)
                .OrderBy(p => p.Code).ToList();

                TNpay = gettransporterspayrolllist.Where(K => K.NetPay > 0).Sum(k => k.NetPay);
                var transporterspayroll = gettransporterspayrolllist.GroupBy(l => l.Code).ToList();

                var suppliers = _context.DSuppliers.Where(u => u.Scode == sacco);
                var transporters = _context.DTransporters.Where(u => u.ParentT == sacco);
                if (user.AccessLevel == AccessLevel.Branch)
                {
                    suppliers = suppliers.Where(p => p.Branch == saccoBranch);
                    transporters = transporters.Where(p => p.Tbranch == saccoBranch);
                }

                SNpay = dpayrollobj.Where(k => k.Npay > 0).Sum(k => k.Npay);
                foreach (var emp in dpayrollobj)
                {
                    if(emp.Npay > 0)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;

                        //long.TryParse(emp.Sno, out long sno);
                        var supplier = suppliers.FirstOrDefault(u => u.Sno == emp.Sno);
                        worksheet.Cell(currentRow, 2).Value = supplier?.Names ?? "";
                        worksheet.Cell(currentRow, 3).Value = "'" + emp.AccountNumber;
                        worksheet.Cell(currentRow, 4).Value = emp.Npay;
                    }
                }

                foreach(var payRolls in transporterspayroll)
                {
                    var transporterdetails = payRolls.FirstOrDefault();
                    if(transporterdetails.NetPay > 0)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = transporterdetails.Code;
                        var transporter = transporters.FirstOrDefault(u => u.TransCode == transporterdetails.Code);
                        worksheet.Cell(currentRow, 2).Value = transporter.TransName;
                        worksheet.Cell(currentRow, 3).Value = "'" + transporterdetails.AccNo;
                        worksheet.Cell(currentRow, 4).Value = transporterdetails.NetPay;
                    }
                }

                var sign1 = "Created By:";
                var sign2 = "Approved By:";
                var sign3 = "Signed By:";
                if(StrValues.Slopes == sacco)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 3).Value = "Signed by:";
                    sign1 = "CHAIRMAN:";
                    sign2 = "TREASURER:";
                    sign3 = "SECRETARY:";
                }

                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total";
                worksheet.Cell(currentRow, 7).Value = SNpay + TNpay;
                currentRow++;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = sign1;
                worksheet.Cell(currentRow, 4).Value = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = sign2;
                worksheet.Cell(currentRow, 4).Value = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = sign3;
                worksheet.Cell(currentRow, 4).Value = "______________________";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{filter.BankName} Payroll List.xlsx");
                }
            }
        }

        [HttpPost]
        public IActionResult PayCombinedSummaryExcel([Bind("DateTo")] FilterVm filter)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var startDate = new DateTime(filter.DateTo.GetValueOrDefault().Year, filter.DateTo.GetValueOrDefault().Month, 1);
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var payrolls = _context.DPayrolls.Where(p => p.EndofPeriod == monthsLastDate && p.SaccoCode == sacco).ToList();
            var dTransportersPayRolls = _context.DTransportersPayRolls.Where(p => p.SaccoCode == sacco && p.EndPeriod == monthsLastDate).ToList();
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
            {
                transporters = transporters.Where(i => i.Tbranch == saccoBranch).ToList();
                payrolls = payrolls.Where(i => i.Branch == saccoBranch).ToList();
                dTransportersPayRolls = dTransportersPayRolls.Where(i => i.Branch == saccoBranch).ToList();
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("dpayrollobj");
                var currentRow = 1;
                var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
                worksheet.Cell(currentRow, 2).Value = company.Name;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = company.Adress;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = company.Town;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = company.Email;
                currentRow = 5;

                worksheet.Cell(currentRow, 2).Value = "Combined Summary Payroll List For:";
                worksheet.Cell(currentRow, 4).Value = monthsLastDate;

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "Description";
                worksheet.Cell(currentRow, 2).Value = "Amount";
                worksheet.Cell(currentRow, 3).Value = "Rate";
                worksheet.Cell(currentRow, 4).Value = "Total";

                var line = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == sacco);
                decimal totalKgs = (decimal)payrolls.Sum(p => p.KgsSupplied);
                var farmersPayables = totalKgs * price.Price;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TOTAL KGS";
                worksheet.Cell(currentRow, 2).Value = totalKgs;
                worksheet.Cell(currentRow, 3).Value = price.Price;
                worksheet.Cell(currentRow, 4).Value = farmersPayables;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                // Transport section
                var transNos = transporters.Where(t => t.TraderRate < 1 && t.CertNo != "KIAMARIGA" && t.CertNo != "KDK 015D")
                    .Select(t => t.TransCode.ToUpper()).ToList();
                var transporterPayroll = dTransportersPayRolls.Where(t => transNos.Contains(t.Code.ToUpper()));
                var transporterAmount = transporterPayroll.Sum(t => t.GrossPay) - transporterPayroll.Sum(t => t.Subsidy);
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TRANSPORTERS";
                worksheet.Cell(currentRow, 2).Value = transporterAmount;

                var tradersNos = transporters.Where(t => t.TraderRate > 0)
                    .Select(t => t.TransCode.ToUpper()).ToList();
                var traderPayroll = dTransportersPayRolls.Where(t => tradersNos.Contains(t.Code.ToUpper()));
                var totalTradersTrans = traderPayroll.Sum(t => t.GrossPay) - traderPayroll.Sum(t => t.Subsidy);
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TRADERS TRANS";
                worksheet.Cell(currentRow, 2).Value = totalTradersTrans;

                var transporter = transporters.FirstOrDefault(t => t.CertNo == "KIAMARIGA");
                var kiamariga = dTransportersPayRolls.Where(p => p.Code.ToUpper().Equals(transporter.TransCode.ToUpper()));
                var kiamarigaAmount = kiamariga.Sum(t => t.GrossPay) - kiamariga.Sum(t => t.Subsidy);
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "KIAMARIGA TRANS";
                worksheet.Cell(currentRow, 2).Value = kiamarigaAmount;

                var tranKdk = transporters.FirstOrDefault(t => t.CertNo == "KDK 015D");
                var tranKdks = dTransportersPayRolls.Where(p => p.Code.ToUpper().Equals(tranKdk.TransCode.ToUpper()));
                var tankerAmount = tranKdks.Sum(t => t.GrossPay) - tranKdks.Sum(t => t.Subsidy);
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "KDK 015D";
                worksheet.Cell(currentRow, 2).Value = tankerAmount;

                var totalTransCost = transporterAmount + totalTradersTrans + kiamarigaAmount
                    + tankerAmount;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TRANSPORT COST";
                worksheet.Cell(currentRow, 4).Value = totalTransCost;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                var totalTradersFee = traderPayroll.Sum(t => t.Subsidy);
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TRADERS FEES";
                worksheet.Cell(currentRow, 4).Value = totalTradersFee;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;
                var farmersSpecialPrice = payrolls.Sum(P => P.Subsidy);
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "FARMERS FEES";
                worksheet.Cell(currentRow, 4).Value = farmersSpecialPrice;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                var totalPayment = farmersPayables + totalTransCost + totalTradersFee + farmersSpecialPrice;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TOTAL PAYMENT";
                worksheet.Cell(currentRow, 4).Value = totalPayment;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                var novPay = payrolls.Sum(t => t.NOV_OVPMNT);
               if(novPay != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "NOV_PAYMENT";
                    worksheet.Cell(currentRow, 2).Value = novPay;
                }

                var societyShares = payrolls.Sum(t => t.Hshares) + dTransportersPayRolls.Sum(t => t.Hshares); ;
                if(societyShares != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "SOCIETY SHARES";
                    worksheet.Cell(currentRow, 2).Value = societyShares;
                }

                var saccoShares = payrolls.Sum(t => t.SACCO_SHARES) + dTransportersPayRolls.Sum(t => t.SACCO_SHARES);
                if(saccoShares != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "SACCO SHARES";
                    worksheet.Cell(currentRow, 2).Value = saccoShares;
                }

                var saccoSavings = payrolls.Sum(t => t.SACCO_SAVINGS) + dTransportersPayRolls.Sum(t => t.SACCO_SAVINGS);
                if(saccoSavings != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "SACCO SAVINGS";
                    worksheet.Cell(currentRow, 2).Value = saccoSavings;
                }

                var store = payrolls.Sum(t => t.Agrovet) + dTransportersPayRolls.Sum(t => t.Agrovet);
                if(store != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "STORES";
                    worksheet.Cell(currentRow, 2).Value = store;
                }

                var societyAdvance = payrolls.Sum(t => t.Advance) + dTransportersPayRolls.Sum(t => t.Advance);
                if(societyAdvance != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "ADVANCE";
                    worksheet.Cell(currentRow, 2).Value = societyAdvance;
                }

                var instantAdvance = payrolls.Sum(t => t.INST_ADVANCE) + dTransportersPayRolls.Sum(t => t.INST_ADVANCE);
                if(instantAdvance != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "INSTANT ADVANCE";
                    worksheet.Cell(currentRow, 2).Value = instantAdvance;
                }

                var normalLoan = payrolls.Sum(t => t.Fsa) + dTransportersPayRolls.Sum(t => t.Fsa);
                if(normalLoan != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "NORMAL LOAN";
                    worksheet.Cell(currentRow, 2).Value = normalLoan;
                }

                var ai = payrolls.Sum(t => t.AI) + dTransportersPayRolls.Sum(t => t.AI);
                if(ai != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "AI";
                    worksheet.Cell(currentRow, 2).Value = ai;
                }

                decimal? kiinga = payrolls.Sum(t => t.KIIGA);
                if(kiinga != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "KIIGA";
                    worksheet.Cell(currentRow, 2).Value = kiinga;
                }

                decimal? milkRecovery = payrolls.Sum(t => t.MILK_RECOVERY) + dTransportersPayRolls.Sum(t => t.MILK_RECOVERY); ;  
                if(milkRecovery != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "MILK RECOVERY";
                    worksheet.Cell(currentRow, 2).Value = milkRecovery;
                }

                var broughtForward = payrolls.Sum(t => t.CurryForward) + dTransportersPayRolls.Sum(t => t.CurryForward);
                if(broughtForward != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Brought Forward";
                    worksheet.Cell(currentRow, 2).Value = broughtForward;
                }

                var others = payrolls.Sum(t => t.Others) + dTransportersPayRolls.Sum(t => t.Others);
                if(others != 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Others";
                    worksheet.Cell(currentRow, 2).Value = others;
                }

                var kiroha = payrolls.Sum(t => t.KIROHA);
                var deductions = novPay + saccoShares + saccoSavings + store + societyAdvance + instantAdvance 
                    + normalLoan + ai + kiinga + kiroha + societyShares + milkRecovery + broughtForward + others;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "KIROHA DAIRY";
                worksheet.Cell(currentRow, 2).Value = kiroha;
                worksheet.Cell(currentRow, 4).Value = deductions;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                var netPay = totalPayment - deductions;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "NET PAY";
                worksheet.Cell(currentRow, 4).Value = netPay;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                // Payment Section
                var bankGroupedPayroll = payrolls.Where(p => p.Npay > 0).GroupBy(p => p.Bank).ToList();
                var combinedSummaries = new List<CombinedSummary>();
                bankGroupedPayroll.ForEach(p =>
                {
                    combinedSummaries.Add(new CombinedSummary
                    {
                        Name = p.Key,
                        Amount = p.Sum(b => b.Npay)
                    });
                });
              
                var bankGroupedTransporterPayroll = dTransportersPayRolls.Where(p => p.NetPay > 0).GroupBy(p => p.BankName).ToList();
                bankGroupedTransporterPayroll.ForEach(p =>
                {
                    combinedSummaries.Add(new CombinedSummary
                    {
                        Name = p.Key,
                        Amount = p.Sum(b => b.NetPay)
                    });
                });

                var summaries = combinedSummaries.GroupBy(s => s.Name);
                decimal? banksAmount = 0;
                summaries.ForEach(d =>
                {
                    var amount = d.Sum(b => b.Amount);
                    banksAmount += amount;
                    if (amount > 0)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = d.Key;
                        worksheet.Cell(currentRow, 2).Value = amount;
                    }
                });
                worksheet.Cell(currentRow, 4).Value = banksAmount;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                currentRow++;
                worksheet.Cell(currentRow, 4).Value = netPay - banksAmount;

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = line;
                worksheet.Cell(currentRow, 2).Value = line;
                worksheet.Cell(currentRow, 3).Value = line;
                worksheet.Cell(currentRow, 4).Value = line;

                var sign1 = "Created By:";
                var sign2 = "Approved By:";
                var sign3 = "Signed By:";
                if (StrValues.Slopes == sacco)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 3).Value = "Signed by:";
                    sign1 = "CHAIRMAN:";
                    sign2 = "TREASURER:";
                    sign3 = "SECRETARY:";
                }

                currentRow++;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = sign1;
                worksheet.Cell(currentRow, 4).Value = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = sign2;
                worksheet.Cell(currentRow, 4).Value = "______________________";
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = sign3;
                worksheet.Cell(currentRow, 4).Value = "______________________";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Combined Summary.xlsx");
                }
            }
        }

        [HttpPost]
        public JsonResult PayCombinedSummaryPdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("PayCombinedSummaryPdf", new { dateTo = filter.DateTo }),
                isRedirect = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> PayCombinedSummaryPdf(DateTime? dateTo)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = "Combined Summary";
            var pdfFile = _reportProvider.PayCombinedSummary(company, title, loggedInUser, saccoBranch, dateTo);
            return File(pdfFile, "application/pdf");
        }


        [HttpPost]
        public JsonResult BankPayrollPdf([FromBody] FilterVm filter)
        {
            return Json(new
            {
                redirectUrl = Url.Action("BankPayrollPdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo, bank = filter.BankName }),
                isRedirect = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> BankPayrollPdf(DateTime? dateFrom, DateTime? dateTo, string bank)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var dpayrollobj = await _context.DPayrolls
                .Where(p => p.EndofPeriod >= dateFrom && p.Bank == bank && p.Npay > 0 && p.EndofPeriod <= dateTo && p.SaccoCode == sacco)
                .OrderBy(s => s.Sno).ToListAsync();

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = $"Payment Report ({dateTo})";
            var pdfFile = _reportProvider.GetBankPayroll(dpayrollobj, company, title, loggedInUser, saccoBranch);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public async Task<IActionResult> SuppliersPayrollExcel([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var dpayrollobj = await _context.DPayrolls
                .Where(p => p.EndofPeriod >= filter.DateFrom && p.EndofPeriod <= filter.DateTo && p.SaccoCode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                dpayrollobj = dpayrollobj.Where(i => i.Branch == saccoBranch).ToList();

            var startDate = new DateTime(filter.DateTo.GetValueOrDefault().Year, filter.DateTo.GetValueOrDefault().Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await suppliersPayrollExcel(startDate, endDate, dpayrollobj);
        }
        public async Task<IActionResult> suppliersPayrollExcel(DateTime startDate, DateTime endDate, List<DPayroll> dpayrollobj)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Farmers Payroll");

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
                worksheet.Cell(currentRow, 2).Value = "Farmers Payroll Report";
                worksheet.Cell(currentRow, 4).Value = dpayrollobj.FirstOrDefault().EndofPeriod;
                
                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "PhoneNo";
                worksheet.Cell(currentRow, 4).Value = "IdNo";
                worksheet.Cell(currentRow, 5).Value = "Transport";
                worksheet.Cell(currentRow, 6).Value = "Agrovet";
                worksheet.Cell(currentRow, 7).Value = "Bonus";
                worksheet.Cell(currentRow, 8).Value = "Shares";
                worksheet.Cell(currentRow, 9).Value = "Advance";
                worksheet.Cell(currentRow, 10).Value = "MidPay";
                worksheet.Cell(currentRow, 11).Value = "midmonth";
                worksheet.Cell(currentRow, 12).Value = "Others";
                worksheet.Cell(currentRow, 13).Value = "Tractor";
                worksheet.Cell(currentRow, 14).Value = "Clinical";
                worksheet.Cell(currentRow, 15).Value = "Extension";
                worksheet.Cell(currentRow, 16).Value = "AI";
                worksheet.Cell(currentRow, 17).Value = "CurryForward";
                worksheet.Cell(currentRow, 18).Value = "SMS";
                worksheet.Cell(currentRow, 19).Value = "Loan";
                worksheet.Cell(currentRow, 20).Value = "Registration";
                worksheet.Cell(currentRow, 21).Value = "Maendeleo";
                worksheet.Cell(currentRow, 22).Value = "ECLOF";
                worksheet.Cell(currentRow, 23).Value = "TDeductions";
                worksheet.Cell(currentRow, 24).Value = "KgsSupplied";
                worksheet.Cell(currentRow, 25).Value = "GPay";
                worksheet.Cell(currentRow, 26).Value = "NPay";
                worksheet.Cell(currentRow, 27).Value = "Bank";
                worksheet.Cell(currentRow, 28).Value = "AccountNumber";
                worksheet.Cell(currentRow, 29).Value = "BBranch";
                worksheet.Cell(currentRow, 30).Value = "Station";

                decimal? Transport = 0;
                decimal? Agrovet = 0;
                decimal? Bonus = 0;
                decimal? Hshares = 0;
                decimal? Advance = 0;
                decimal? Midmonth = 0;
                decimal? MIDPAY = 0;
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
                decimal? Subsidy = 0;
                decimal? Npay = 0;
                decimal? loans = 0;
                decimal? Registration = 0;
                decimal? ECLOF = 0;
                decimal? saccoDed = 0;

                var payrollData = await Gedpayrolldata(startDate, endDate);
                Transport = payrollData.Sum(k => k.Transport);
                Agrovet = payrollData.Sum(k => k.Agrovet);
                Bonus = payrollData.Sum(k => k.Bonus);
                Hshares = payrollData.Sum(k => k.Hshares);
                Advance = payrollData.Sum(k => k.Advance);
                Midmonth = payrollData.Sum(k => k.Midmonth);
                MIDPAY = payrollData.Sum(k => k.MIDPAY);
                Others = payrollData.Sum(k => k.Others);
                Tractor = payrollData.Sum(k => k.Tractor);
                CLINICAL = payrollData.Sum(k => k.CLINICAL);
                extension = payrollData.Sum(k => k.extension);
                AI = payrollData.Sum(k => k.AI);
                CurryForward = payrollData.Sum(k => k.CurryForward);
                SMS = payrollData.Sum(k => k.SMS);
                Tdeductions = payrollData.Sum(k => k.Tdeductions);
                KgsSupplied = payrollData.Sum(k => k.KgsSupplied);
                Gpay = payrollData.Sum(k => k.Gpay);
                Subsidy = payrollData.Sum(k => k.Subsidy);
                Npay = payrollData.Sum(k => k.Npay);
                loans = payrollData.Sum(k => k.Fsa);
                ECLOF = payrollData.Sum(k => k.ECLOF);
                saccoDed = payrollData.Sum(k => k.saccoDed);

                payrollData.ForEach(c =>
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 1).Value = c.Sno;
                    worksheet.Cell(currentRow, 2).Value = c.Name;
                    worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 3).Value = c.PhoneNo;
                    worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 4).Value = c.IdNo;
                    worksheet.Cell(currentRow, 5).Value = c.Transport;
                    worksheet.Cell(currentRow, 6).Value = c.Agrovet;
                    worksheet.Cell(currentRow, 7).Value = c.Bonus;
                    worksheet.Cell(currentRow, 8).Value = c.Hshares;
                    worksheet.Cell(currentRow, 9).Value = c.Advance;
                    worksheet.Cell(currentRow, 10).Value = c.MIDPAY;
                    worksheet.Cell(currentRow, 11).Value = c.Midmonth;
                    worksheet.Cell(currentRow, 12).Value = c.Others;
                    worksheet.Cell(currentRow, 13).Value = c.Tractor;
                    worksheet.Cell(currentRow, 14).Value = c.CLINICAL;
                    worksheet.Cell(currentRow, 15).Value = c.extension;
                    worksheet.Cell(currentRow, 16).Value = c.AI;
                    worksheet.Cell(currentRow, 17).Value = c.CurryForward;
                    worksheet.Cell(currentRow, 18).Value = c.SMS;
                    worksheet.Cell(currentRow, 19).Value = c.Fsa;
                    worksheet.Cell(currentRow, 20).Value = c.Registration;
                    worksheet.Cell(currentRow, 21).Value = c.saccoDed;
                    worksheet.Cell(currentRow, 22).Value = c.ECLOF;
                    worksheet.Cell(currentRow, 23).Value = c.Tdeductions;
                    worksheet.Cell(currentRow, 24).Value = c.KgsSupplied;
                    worksheet.Cell(currentRow, 25).Value = c.Gpay;
                    worksheet.Cell(currentRow, 26).Value = c.Npay;
                    worksheet.Cell(currentRow, 27).Value = c.Bank;
                    worksheet.Cell(currentRow, 28).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 28).Value = c.AccountNumber;
                    worksheet.Cell(currentRow, 29).Value = c.Bbranch;
                    worksheet.Cell(currentRow, 30).Value = c.Branch;
                });

                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total";
                worksheet.Cell(currentRow, 4).Value = Transport;
                worksheet.Cell(currentRow, 5).Value = Agrovet;
                worksheet.Cell(currentRow, 6).Value = Bonus;
                worksheet.Cell(currentRow, 7).Value = Hshares;
                worksheet.Cell(currentRow, 8).Value = Advance;
                worksheet.Cell(currentRow, 9).Value = MIDPAY;
                worksheet.Cell(currentRow, 10).Value = Midmonth;
                worksheet.Cell(currentRow, 11).Value = Others;
                worksheet.Cell(currentRow, 12).Value = Tractor;
                worksheet.Cell(currentRow, 13).Value = CLINICAL;
                worksheet.Cell(currentRow, 14).Value = extension;
                worksheet.Cell(currentRow, 15).Value = AI;
                worksheet.Cell(currentRow, 16).Value = CurryForward;
                worksheet.Cell(currentRow, 17).Value = SMS;
                worksheet.Cell(currentRow, 18).Value = loans;
                worksheet.Cell(currentRow, 19).Value = Registration;
                worksheet.Cell(currentRow, 20).Value = Tdeductions;
                worksheet.Cell(currentRow, 21).Value = KgsSupplied;
                worksheet.Cell(currentRow, 22).Value = Gpay;
                worksheet.Cell(currentRow, 23).Value = Subsidy;
                worksheet.Cell(currentRow, 24).Value = Npay;

                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Farmers Payroll Report.xlsx");
                }
            }
        }

    private async Task<List<payrolldetail>> Gedpayrolldata(DateTime startDate, DateTime endDate)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        var dPayrolls = await _context.DPayrolls.Where(m => m.SaccoCode == sacco
        && (m.EndofPeriod >= startDate && m.EndofPeriod <= endDate)).ToListAsync();
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch)
            dPayrolls = dPayrolls.Where(i => i.Branch == saccoBranch).ToList();
        var payrolllist = dPayrolls.GroupBy(s => s.Sno).ToList();
        var payrollData = new List<payrolldetail>();
        var suppliers = await _context.DSuppliers.Where(m => m.Scode == sacco).ToListAsync();
        if (user.AccessLevel == AccessLevel.Branch)
            suppliers = suppliers.Where(i => i.Branch == saccoBranch).ToList();
            // m.Sno.ToUpper().Equals(val.Sno.ToUpper())
        payrolllist.ForEach(d =>
        {
            var val = d.FirstOrDefault();
            var supplier = suppliers.FirstOrDefault(m => m.Sno.ToUpper().Equals(val.Sno.ToUpper()));
            payrollData.Add(new payrolldetail
            {
                Sno = val.Sno,
                Name = supplier.Names,
                PhoneNo = supplier.PhoneNo,
                IdNo = val.IdNo,
                Transport=val.Transport,
                Agrovet= val.Agrovet,
                Bonus=val.Bonus,
                Hshares=val.Hshares,
                Advance=val.Advance,
                Midmonth=val.Midmonth,
                MIDPAY=val.MIDPAY,
                Others=val.Others,
                Tractor=val.Tractor,
                CLINICAL=val.CLINICAL,
                Registration = val.Registration,
                extension=val.extension,
                AI=val.AI,
                CurryForward=val.CurryForward,
                SMS=val.SMS,
                Tdeductions=val.Tdeductions,
                KgsSupplied=val.KgsSupplied,
                Gpay=val.Gpay,
                Npay=val.Npay,
                Bank=val.Bank,
                AccountNumber=val.AccountNumber,
                Bbranch=val.Bbranch,
                Branch=val.Branch,
                ECLOF = val.ECLOF,
                saccoDed = val.saccoDed
            });
        });
        return (payrollData);
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var dpayrollobj = _context.DPayrolls
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        return SuppliersActiveExcel(filter);
    }

    [HttpPost]
    public IActionResult SuppliersInActive([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
        sacco = sacco ?? "";
        var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= filter.DateFrom && i.TransDate <= filter.DateTo
        && i.SaccoCode == sacco && i.Branch == saccoBranch)
            .Select(i => i.Sno.ToUpper());
        suppliersobj = _context.DSuppliers
            .Where(u => u.Scode == sacco && !activeSuppliers.Contains(u.Sno.ToUpper()))
            .OrderBy(u => u.Sno.ToUpper());
        return SuppliersInActiveExcel(filter);
    }

    [HttpPost]
    public JsonResult 
            Pdf([FromBody] FilterVm filter)
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        sacco = sacco ?? "";
        var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= dateFrom && i.TransDate <= dateTo)
            .Select(i => i.Sno.ToUpper());
        suppliersobj = _context.DSuppliers
            .Where(u => u.Scode == sacco && activeSuppliers.Contains(u.Sno.ToUpper()))
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
        sacco = sacco ?? "";
        var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= dateFrom
        && i.TransDate <= dateTo && i.SaccoCode == sacco && i.Branch == saccoBranch).Select(i => i.Sno.ToUpper());
        suppliersobj = _context.DSuppliers
            .Where(u => u.Scode == sacco && !activeSuppliers.Contains(u.Sno.ToUpper()))
            .OrderBy(u => u.Sno.ToUpper());

        var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
        var title = "In Active Supplier";
        var pdfFile = _reportProvider.GetSuppliersReport(suppliersobj, company, title);
        return File(pdfFile, "application/pdf");
    }

    [HttpPost]
    public IActionResult BranchIntake([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
    public async Task<IActionResult> ZonesIntakeReport([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
       
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Zones");
            var currentRow = 1;
            var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
            worksheet.Cell(currentRow, 2).Value = company.Name;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Adress;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Town;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Email;
            currentRow = 5;
            worksheet.Cell(currentRow, 2).Value = "Zones Report";

            currentRow = 6;
            var zones = await _context.Zones.Where(z => z.Code == sacco).OrderBy(z => z.Name).ToListAsync();
            var currentColumn = 1;
            worksheet.Cell(currentRow, currentColumn).Value = "Date";
            foreach(var zone in zones)
            {
                currentColumn++;
                worksheet.Cell(currentRow, currentColumn).Value = zone.Name;
            }

            var productIntakes = await _context.ProductIntake.Where(i => i.TransDate >= filter.DateFrom
                && i.TransDate <= filter.DateTo && i.SaccoCode == sacco && i.Qsupplied != 0 && i.Description != "Transport")
                .OrderBy(i => i.TransDate).ToListAsync();

            var intakes = productIntakes.GroupBy(i=> i.TransDate).ToList();
            foreach(var intake in intakes)
            {
                currentRow++;
                currentColumn = 1;
                worksheet.Cell(currentRow, currentColumn).Value = intake.Key;
                foreach (var zone in zones)
                {
                    currentColumn++;
                    worksheet.Cell(currentRow, currentColumn).Value = intake.Where(i => i.Zone == zone.Name).Sum(i => i.Qsupplied);
                }
            }
           
            currentRow++;
            currentColumn = 1;
            worksheet.Cell(currentRow, currentColumn).Value = "Total Kgs";
            foreach (var zone in zones)
            {
                currentColumn++;
                worksheet.Cell(currentRow, currentColumn).Value = productIntakes.Where(i => i.Zone == zone.Name).Sum(i => i.Qsupplied);
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Zones Report.xlsx");
            }
        }
    }

    [HttpPost]
    public JsonResult ZoneIntakePdf([FromBody] FilterVm filter)
    {
        return Json(new
        {
            redirectUrl = Url.Action("ZoneIntakePdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo}),
            isRedirect = true
        });
    }

    [HttpGet]
    public async Task<IActionResult> ZoneIntakePdf(DateTime? dateFrom, DateTime? dateTo)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var productIntakes = await _context.ProductIntake.Where(i => i.TransDate >= dateFrom
                && i.TransDate <= dateTo && i.SaccoCode == sacco && i.Qsupplied != 0 && i.Description != "Transport")
                .OrderBy(i => i.TransDate).ToListAsync();
            
        var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
        var title = "Zones Report";
        var pdfFile =  _reportProvider.GetZonesIntakePdf(productIntakes, company, title);
        return File(pdfFile, "application/pdf");
    }

    [HttpPost]
    public IActionResult BranchIntakeAudit([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        sacco = sacco ?? "";
       
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
        return BranchIntakeAuditExcel(filter.DateFrom, filter.DateTo, filter.Branch);
    }

    [HttpPost]
    public IActionResult correctionIntake([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
    public async Task<IActionResult> SalesReport([Bind("DateFrom,DateTo,Debtor")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        decimal totalAmount = 0, totalQuantity = 0;
        using (var workbook = new XLWorkbook())
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var worksheet = workbook.Worksheets.Add("Gltransaction");
            var currentRow = 1;
            var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
            worksheet.Cell(currentRow, 2).Value = company.Name;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Adress;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Town;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Email;

            currentRow = 5;
            worksheet.Cell(currentRow, 2).Value = $"{filter.Debtor} Sales Report";

            currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "TransDate";
            worksheet.Cell(currentRow, 2).Value = "Qsupplied";
            worksheet.Cell(currentRow, 3).Value = "Price";
            worksheet.Cell(currentRow, 4).Value = "Amount";

            var dispatches = await _context.Dispatch.Where(d => d.DName.ToUpper().Equals(filter.Debtor.ToUpper())
                && d.Dcode.ToUpper().Equals(sacco.ToUpper()) && d.Transdate >= filter.DateFrom
                && d.Transdate <= filter.DateTo).ToListAsync();
            var debtors = await _context.DDebtors.Where(d => d.Dcode == sacco).ToListAsync();
            foreach (var dispatch in dispatches)
            {
                var debtor = debtors.FirstOrDefault(d => d.Dname.ToUpper().Equals(dispatch.DName.ToUpper()));
                var salesAmount = dispatch.Dispatchkgs * debtor.Price;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = dispatch.Transdate;
                worksheet.Cell(currentRow, 2).Value = dispatch.Dispatchkgs;
                worksheet.Cell(currentRow, 3).Value = debtor.Price;
                worksheet.Cell(currentRow, 4).Value = salesAmount;

                totalAmount += dispatch.Dispatchkgs;
                totalQuantity += salesAmount;
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        sacco = sacco ?? "";
        var endDate = Convert.ToDateTime(filter.DateTo.ToString());
        var DateFrom = new DateTime(endDate.Year, endDate.Month, 1);
        var DateTo = DateFrom.AddMonths(1).AddDays(-1);
        var Branch = filter.Branch;
        transporterobj = await _context.DTransporters.Where(u => u.ParentT == sacco).ToListAsync();
            if (!string.IsNullOrEmpty(filter.Transporter))
                transporterobj = transporterobj.Where(u => u.TransCode.ToUpper().Equals(filter.Transporter.ToUpper())).ToList();

        return await TransportersBalancingExcel(DateFrom, DateTo, filter.Transporter);
    }
    [HttpPost]
    public async Task<IActionResult> DownloadDispatchBalReport([Bind("DateFrom,DateTo,Branch,Transporter")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
        var transporterpayrollobj = _context.DTransportersPayRolls
                    .Where(p => p.SaccoCode == sacco && p.EndPeriod >= filter.DateFrom && p.EndPeriod <= filter.DateTo)
                    .OrderBy(p => p.Code);
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
            worksheet.Cell(currentRow, 3).Value = "PhoneNo";
            worksheet.Cell(currentRow, 4).Value = "IdNo";
            worksheet.Cell(currentRow, 5).Value = "Kgs";
            worksheet.Cell(currentRow, 6).Value = "Amount";
            worksheet.Cell(currentRow, 7).Value = "Subsidy";
            worksheet.Cell(currentRow, 8).Value = "GrossPay";
            worksheet.Cell(currentRow, 9).Value = "Agrovet";
            worksheet.Cell(currentRow, 10).Value = "Shares";
            worksheet.Cell(currentRow, 11).Value = "Advance";
            worksheet.Cell(currentRow, 12).Value = "Tractor";
            worksheet.Cell(currentRow, 13).Value = "CLINICAL";
            worksheet.Cell(currentRow, 14).Value = "VARIANCE";
            worksheet.Cell(currentRow, 15).Value = "CurryForward";
            worksheet.Cell(currentRow, 16).Value = "extension";
            worksheet.Cell(currentRow, 17).Value = "AI";
            worksheet.Cell(currentRow, 18).Value = "MidPay";
            worksheet.Cell(currentRow, 19).Value = "Others";
            worksheet.Cell(currentRow, 20).Value = "MAENDELEO";
            worksheet.Cell(currentRow, 21).Value = "ECLOF";
            worksheet.Cell(currentRow, 22).Value = "Totaldeductions";
            worksheet.Cell(currentRow, 23).Value = "NPay";
            worksheet.Cell(currentRow, 24).Value = "Bank";
            worksheet.Cell(currentRow, 25).Value = "AccountNumber";
            worksheet.Cell(currentRow, 26).Value = "Branch";
            

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
            decimal? MidPay = 0;
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
                MidPay = transporterpayrollobj.Sum(k => k.MIDPAY);
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
                var TName = _context.DTransporters.FirstOrDefault(u => u.TransCode == emp.Code && u.ParentT == sacco && u.Tbranch == emp.Branch);
                worksheet.Cell(currentRow, 2).Value = TName.TransName;
                worksheet.Cell(currentRow, 3).Value = emp.PhoneNo;
                worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "@";
                worksheet.Cell(currentRow, 4).Value = TName.CertNo;
                worksheet.Cell(currentRow, 5).Value = emp.QntySup;
                worksheet.Cell(currentRow, 6).Value = emp.Amnt;
                worksheet.Cell(currentRow, 7).Value = emp.Subsidy;
                worksheet.Cell(currentRow, 8).Value = emp.GrossPay;
                worksheet.Cell(currentRow, 9).Value = emp.Agrovet;
                worksheet.Cell(currentRow, 10).Value = emp.Hshares;
                worksheet.Cell(currentRow, 11).Value = emp.Advance;
                worksheet.Cell(currentRow, 12).Value = emp.Tractor;
                worksheet.Cell(currentRow, 13).Value = emp.CLINICAL;
                worksheet.Cell(currentRow, 14).Value = emp.VARIANCE;
                worksheet.Cell(currentRow, 15).Value = emp.CurryForward;
                worksheet.Cell(currentRow, 16).Value = emp.extension;
                worksheet.Cell(currentRow, 17).Value = emp.AI;
                worksheet.Cell(currentRow, 18).Value = emp.MIDPAY;
                worksheet.Cell(currentRow, 19).Value = emp.Others;
                worksheet.Cell(currentRow, 20).Value = emp.saccoDed;
                worksheet.Cell(currentRow, 21).Value = emp.ECLOF;
                worksheet.Cell(currentRow, 22).Value = emp.Totaldeductions;
                worksheet.Cell(currentRow, 23).Value = emp.NetPay;
                worksheet.Cell(currentRow, 24).Value = emp.BankName;
                worksheet.Cell(currentRow, 25).Style.NumberFormat.Format = "@";
                worksheet.Cell(currentRow, 25).Value = emp.AccNo;
                worksheet.Cell(currentRow, 26).Value = emp.Branch;
                
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
            worksheet.Cell(currentRow, 17).Value = MidPay;
            worksheet.Cell(currentRow, 18).Value = Others;
            worksheet.Cell(currentRow, 19).Value = Totaldeductions;
            worksheet.Cell(currentRow, 20).Value = Npay;
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        sacco = sacco ?? "";
        var transporterpayrollobj = _context.DTransportersPayRolls
            .Where(p => p.SaccoCode == sacco && p.EndPeriod >= dateFrom && p.EndPeriod <= dateTo)
            .OrderBy(p => p.Code);

        var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
        var title = $"Transporters Payroll Reports ({dateTo})";
        var pdfFile = _reportProvider.GetTransportersPayroll(transporterpayrollobj, company, title);
        return File(pdfFile, "application/pdf");
    }

    [HttpPost]
    public async Task<IActionResult> Deductions([Bind("DateFrom,DateTo,Code")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        filter.Code = filter?.Code ?? "";
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        if(StrValues.Slopes == sacco)
            filter.Code = filter.Code == "Store" ? "AGROVET" : filter.Code;
        IQueryable<ProductIntake> productIntakes = _context.ProductIntake.Where(n => n.SaccoCode == sacco && n.Description != "Transport" 
        && n.TransactionType == TransactionType.Deduction && n.TransDate >= filter.DateFrom && n.TransDate <= filter.DateTo 
        && n.ProductType.ToUpper().Equals(filter.Code.ToUpper()));
        var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco).ToListAsync();
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch) {
            productIntakes = productIntakes.Where(i => i.Branch == saccoBranch);
            suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();
        }

        var intakes = await productIntakes.OrderBy(i => i.Sno).ToListAsync();
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("productIntakeobj");
            var currentRow = 1;
            var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
            worksheet.Cell(currentRow, 2).Value = company.Name;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Adress;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Town;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Email;

            currentRow = 5;
            worksheet.Cell(currentRow, 2).Value =$"Suppliers {filter.Code} Deductions Report For: {filter.DateTo.GetValueOrDefault().ToString("dd/MM/yyy")}";

            currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "SNo";
            worksheet.Cell(currentRow, 2).Value = "Name";
            worksheet.Cell(currentRow, 3).Value = "TransDate";
            worksheet.Cell(currentRow, 4).Value = "ProductType";
            worksheet.Cell(currentRow, 5).Value = "Amount";
            worksheet.Cell(currentRow, 6).Value = "Remarks";
            worksheet.Cell(currentRow, 7).Value = "Station";
            decimal? totalDeductions = 0;
            intakes.ForEach(s =>
            {
                var supplier = suppliers.FirstOrDefault(i => i.Sno.ToUpper().Equals(s.Sno.ToUpper()));
                if(supplier != null)
                {
                    currentRow++;
                    totalDeductions += s.DR;
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 1).Value = s.Sno;
                    worksheet.Cell(currentRow, 2).Value = supplier?.Names ?? "";
                    worksheet.Cell(currentRow, 3).Value = s.TransDate;
                    worksheet.Cell(currentRow, 4).Value = s.ProductType;
                    worksheet.Cell(currentRow, 5).Value = s.DR;
                    worksheet.Cell(currentRow, 6).Value = s.Remarks;
                    worksheet.Cell(currentRow, 7).Value = s.Branch;
                }
            });
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = "Total Amount: ";
            worksheet.Cell(currentRow, 5).Value = totalDeductions;
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

    [HttpPost]
    public IActionResult SharesDeductions([Bind("County")] FilterVm filter)
    {
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        sacco = sacco ?? "";

        productIntakeobj = _context.ProductIntake.Where(b => (b.ProductType.ToLower().Contains("share") || b.ProductType.ToLower().Contains("shares")))
            .ToList();

        return DeductionsPerCountyExcel(filter);
    }
        
    public IActionResult DeductionsPerCountyExcel(FilterVm filter)
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
            worksheet.Cell(currentRow, 2).Value = "Suppliers Shares  Contribution Report: ";

            var pos = _context.DCompanies.Where(n => n.Province == filter.County).ToList()
                .Select(m=>m.Name).ToList();
            foreach(var po in pos)
            {
                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "Shares Type";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Remarks";
                worksheet.Cell(currentRow, 7).Value = "Station";
                decimal? sum2 = 0;
                //var checkshares = _context.DShares.Where(c=>c.SaccoCode== po 
                //&& (c.Type.ToLower().Contains("share") || c.Type.ToLower().Contains("shares"))).ToList();
                productIntakeobj = productIntakeobj.Where(b => b.SaccoCode == po
                && (b.ProductType.ToLower().Contains("share") || b.ProductType.ToLower().Contains("shares"))
                && (b.Branch != "" || b.Branch != null)).OrderBy(p => p.Branch).ToList();
                var branches = productIntakeobj.GroupBy(b => b.Branch).ToList();
                //var branches = checkshares.GroupBy(b => b.Branch).ToList();
                branches.ForEach(s =>
                {
                    var branchname = s.FirstOrDefault();
                    currentRow++;
                    if (branchname.Branch != "") { 
                    worksheet.Cell(currentRow, 1).Value = branchname.Branch;

                    //var supplierslist = productIntakeobj.Where(k =>k.SaccoCode==po && k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                    //.OrderBy(h => h.Sno).ToList();
                    var dedutciontype = _context.DDcodes.Where(b => b.Dcode == po 
                    && (b.Description.ToLower().Contains("share") || b.Description.ToLower().Contains("shares"))).ToList()
                        .GroupBy(d => d.Description).ToList();

                    dedutciontype.ForEach(w =>
                    {
                        var deduction = w.FirstOrDefault();
                        decimal? sum = 0;
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = deduction.Description;

                        decimal totalded = 0;

                        var sharescalc = _context.DShares.Where(d=>d.SaccoCode == po && d.Type.ToUpper().Equals(deduction.Description.ToUpper()))
                        .Select(f=>f.Sno.ToUpper()).Distinct().ToList();

                        var productin = _context.ProductIntake.Where(d => d.SaccoCode == po && d.ProductType.ToUpper().Equals(deduction.Description.ToUpper()))
                        .Select(f => f.Sno.ToUpper()).Distinct().ToList();

                        var suppliers = _context.DSuppliers.Where(v=>v.Scode == po && (sharescalc.Contains(v.Sno.ToUpper())) 
                        && (productin.Contains(v.Sno.ToUpper()))).Distinct().ToList()
                        .GroupBy(n => n.Sno).ToList();

                        //var suppliers = supplierslist
                        //.Where(r => r.ProductType.ToUpper().Equals(deduction.Description.ToUpper()))
                        //.GroupBy(n=>n.Sno).ToList();
                        suppliers.ForEach(v =>
                        {
                            var emp = v.FirstOrDefault();
                            var sharesamount = _context.DShares.Where(d => d.SaccoCode == po
                            && d.Type.ToUpper().Equals(deduction.Description.ToUpper()) && d.Sno.ToUpper().Equals(emp.Sno.ToUpper())).ToList().Sum(n => n.Amount);

                            var productinamount = _context.ProductIntake.Where(d => d.SaccoCode == po 
                            && d.ProductType.ToUpper().Equals(deduction.Description.ToUpper()) && d.Sno.ToUpper().Equals(emp.Sno.ToUpper())).ToList().Sum(n => n.DR);

                            totalded = (decimal)(productinamount + sharesamount);

                            var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                            if (TransporterExist > 0)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                                worksheet.Cell(currentRow, 1).Value = emp.Sno;
                                var TName = _context.DSuppliers.FirstOrDefault(u => u.Sno == emp.Sno && u.Scode == po);
                                worksheet.Cell(currentRow, 2).Value = TName?.Names ?? "";
                                worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                                worksheet.Cell(currentRow, 4).Value = deduction.Description.ToUpper();
                                worksheet.Cell(currentRow, 5).Value = totalded;
                                worksheet.Cell(currentRow, 6).Value = deduction.Description.ToUpper();
                                worksheet.Cell(currentRow, 7).Value = emp.Branch;
                                sum += totalded;

                            }
                        });

                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = "Total " + deduction.Description + " Amount: ";
                        worksheet.Cell(currentRow, 2).Value = sum;
                        sum2 += sum;
                    });
                    }
                });
                
                currentRow++;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Grand Total Amount: ";
                worksheet.Cell(currentRow, 2).Value = sum2;
            }
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
    public async Task<IActionResult> DeductionsPdf(DateTime? dateFrom, DateTime? dateTo)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        productIntakeobj = await _context.ProductIntake.Where(u => u.TransDate >= dateFrom && u.TransDate <= dateTo && u.Qsupplied == 0 && u.SaccoCode == sacco).ToListAsync();
        if (user.AccessLevel == AccessLevel.Branch)
            productIntakeobj = productIntakeobj.Where(i => i.Branch == saccoBranch).ToList();
        var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
        var title = "Suppliers Deductions";
        var pdfFile = await _reportProvider.GetIntakesPdf(productIntakeobj, company, title, TransactionType.Deduction, loggedInUser, saccoBranch);
        return File(pdfFile, "application/pdf");
    }

    [HttpPost]
    public IActionResult TDeductions([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        sacco = sacco ?? "";
        var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
        var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
        var ifTransporterExist = _context.DTransporters.Where(m => m.ParentT == sacco).ToList().Select(n=>n.TransCode.ToUpper());
        productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo
        && ifTransporterExist.Contains(u.Sno.ToUpper()) && u.Qsupplied == 0 && u.SaccoCode == sacco).OrderBy(n => n.TransDate);
        return TDeductionsExcel();
    }

    [HttpPost]
    public async Task<IActionResult> Intake([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        
        var startDate = new DateTime(filter.DateTo.GetValueOrDefault().Year, filter.DateTo.GetValueOrDefault().Month, 1);
        var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
        var transporters = await _context.DTransporters.Where(t => t.ParentT == sacco).ToListAsync();
        var  suppliers = await _context.DSuppliers.Where(u => u.Scode == sacco).ToListAsync();
        IQueryable<ProductIntake> intakes = _context.ProductIntake.Where(u => u.Qsupplied != 0 && u.SaccoCode == sacco && u.TransDate >= startDate && u.TransDate <= monthsLastDate);
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch) { 
            intakes = intakes.Where(t => t.Branch == saccobranch);
            suppliers = suppliers.Where(t => t.Branch == saccobranch).ToList();
            transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();
        }

        IQueryable<ProductIntake> productIntakeobj = intakes.Where(u => u.TransDate >= filter.DateFrom && u.TransDate <= filter.DateTo && u.Description != "Transport");
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
            worksheet.Cell(currentRow, 2).Value = "Suppliers Intake Report";

            currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "SNo";
            worksheet.Cell(currentRow, 2).Value = "Name";
            worksheet.Cell(currentRow, 3).Value = "TransDate";
            worksheet.Cell(currentRow, 4).Value = "ProductType";
            worksheet.Cell(currentRow, 5).Value = "Qsupplied";
            worksheet.Cell(currentRow, 6).Value = "Price";
            worksheet.Cell(currentRow, 7).Value = "Description";
            worksheet.Cell(currentRow, 8).Value = "Vehicle";
            worksheet.Cell(currentRow, 9).Value = "Receipt No.";
            worksheet.Cell(currentRow, 10).Value = "Cumlative";
            worksheet.Cell(currentRow, 11).Value = "Invoice No.";

            var farmerIntakes = await intakes.Where(i => i.Description != "Transport").ToListAsync();
            var transporterIntakes = await intakes.Where(i => i.Sno.ToUpper().Contains("T")).ToListAsync();

            decimal sum = 0;
            var productIntakes = await productIntakeobj.OrderBy(i => i.Auditdatetime).ToListAsync();
            foreach (var intake in productIntakes)
            {
                var transcode = transporterIntakes.FirstOrDefault(i => i.Auditdatetime == intake.Auditdatetime)?.Sno ?? "";
                var transporter = transporters.FirstOrDefault(t => t.TransCode.ToUpper().Equals(transcode.ToUpper()));
                var supplier = suppliers.FirstOrDefault(u => u.Sno == intake.Sno);
                var remarks = StrValues.Slopes == sacco ? intake.Remarks : "";
                if (supplier != null)
                {
                    var cumlative = farmerIntakes.Where(i => i.Sno.ToUpper().Equals(supplier.Sno.ToUpper())).Sum(i => i.Qsupplied);
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 1).Value = intake.Sno;
                    worksheet.Cell(currentRow, 2).Value = supplier?.Names ?? "";
                    worksheet.Cell(currentRow, 3).Value = intake.TransDate;
                    worksheet.Cell(currentRow, 4).Value = intake.ProductType;
                    worksheet.Cell(currentRow, 5).Value = intake.Qsupplied;
                    worksheet.Cell(currentRow, 6).Value = intake.Ppu;
                    worksheet.Cell(currentRow, 7).Value = intake.Description;
                    worksheet.Cell(currentRow, 8).Value = transporter?.CertNo ?? "";
                    worksheet.Cell(currentRow, 9).Value = intake.Id;
                    worksheet.Cell(currentRow, 10).Value = cumlative;
                    worksheet.Cell(currentRow, 11).Value = remarks;

                    sum += (intake.Qsupplied);
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

    [HttpPost]
    public async Task<IActionResult> DailySummary([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

        var productIntakes = await GetIntakes(filter);
        var intakes = productIntakes.OrderBy(i => i.TransDate).GroupBy(i => i.TransDate).ToList();
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Daily Summary");
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
            worksheet.Cell(currentRow, 2).Value = "Daily Summary";

            currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "Date";
            worksheet.Cell(currentRow, 2).Value = "Qsupplied";
            decimal sum = 0;
            
            foreach (var intake in intakes)
            {
                var qnty = intake.Sum(i => i.Qsupplied);
               currentRow++;
                worksheet.Cell(currentRow, 1).Value = intake.Key;
                worksheet.Cell(currentRow, 2).Value = qnty;
                sum += qnty;
            }
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = "Total Kgs";
            worksheet.Cell(currentRow, 2).Value = sum;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Daily Summary.xlsx");
            }
        }
    }

    [HttpPost]
    public JsonResult DailySummaryPdf([FromBody] FilterVm filter)
    {
        return Json(new
        {
            redirectUrl = Url.Action("DailySummaryPdf", new { dateFrom = filter.DateFrom, dateTo = filter.DateTo }),
            isRedirect = true
        });
    }

    [HttpGet]
    public async Task<IActionResult> DailySummaryPdf(DateTime? dateFrom, DateTime? dateTo)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        
        var productIntakes = await GetIntakes(new FilterVm
        {
            DateFrom = dateFrom,
            DateTo = dateTo
        });
        var intakes = productIntakes.OrderBy(i => i.TransDate).GroupBy(i => i.TransDate).ToList();
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
        var title = "Daily Summary";
        var pdfFile = _reportProvider.GetDailySummary(intakes, company, title);
        return File(pdfFile, "application/pdf");
    }

    private async Task<List<ProductIntake>> GetIntakes(FilterVm filter)
    {
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

        var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
        var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
        var intakes = await _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo
        && u.Qsupplied != 0 && u.SaccoCode == sacco && u.Description != "Transport").ToListAsync();

        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch)
            intakes = intakes.Where(t => t.Branch == saccobranch).ToList();

        return intakes;
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
    public async Task<IActionResult> IntakePdf(DateTime? dateFrom, DateTime? dateTo)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        productIntakeobj = await _context.ProductIntake.Where(u => u.TransDate >= dateFrom && u.TransDate <= dateTo && u.Qsupplied != 0 && u.SaccoCode == sacco).ToListAsync();
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch)
            productIntakeobj = productIntakeobj.Where(i => i.Branch == saccoBranch).ToList();
        var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
        var title = "Intakes";
        var pdfFile = await _reportProvider.GetIntakesPdf(productIntakeobj, company, title, TransactionType.Intake, loggedInUser, saccoBranch);
        return File(pdfFile, "application/pdf");
    }

    [HttpPost]
    public async Task<IActionResult> TIntake([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        transporterobj = await _context.DTransporters.Where(u => u.ParentT == sacco).ToListAsync();
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch)
            transporterobj = transporterobj.Where(i => i.Tbranch == saccobranch);

        return await TIntakeExcel(filter.DateFrom, filter.DateTo);
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
            worksheet.Cell(currentRow, 4).Value = "CertNo";
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
    
    public IActionResult SuppliersActiveExcel(FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
        //var DateFro = Convert.ToDateTime(DateFrom.ToString());
        //var DateT = Convert.ToDateTime(DateTo.ToString());
        var activeSuppliers = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= filter.DateFrom && i.TransDate <= filter.DateTo)
            .Select(i => i.Sno.ToUpper());
       

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
            int sum, sum2 = 0;

                // suppliersobj; 
            IQueryable<DSupplier> supplierslist = _context.DSuppliers;
            var supplierobj = supplierslist.Where(u => u.Scode == sacco && activeSuppliers.Contains(u.Sno.ToUpper())).ToList();
            sum2 = supplierobj.Count();
            supplierobj = supplierobj.OrderBy(p => p.Branch).ToList();
            var branches = supplierobj.GroupBy(b => b.Branch).ToList();
            branches.ForEach(s =>
            {
                var branchname = s.FirstOrDefault();
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = branchname.Branch;
                var suppliers = supplierobj.Where(k => k.Branch.ToUpper().Equals(branchname.Branch.ToUpper()))
                .OrderBy(h => h.Sno).ToList();
                sum = suppliers.Count();
                foreach (var emp in suppliers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 1).Value = emp.Sno;
                    worksheet.Cell(currentRow, 2).Value = emp.Names;
                    worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                    worksheet.Cell(currentRow, 4).Value = emp.IdNo;
                    worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "@";
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
                    "Active Suppliers Register.xlsx");
            }
        }
    }
    public IActionResult SuppliersInActiveExcel(FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(currentRow, 1).Value = emp.Sno;
                    worksheet.Cell(currentRow, 2).Value = emp.Names;
                    worksheet.Cell(currentRow, 3).Value = emp.Regdate;
                    worksheet.Cell(currentRow, 4).Value = emp.IdNo;
                    worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "@";
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
                    "InActive Suppliers Register.xlsx");
            }
        }

    }

    public IActionResult Excel()
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
  
    public async Task<IActionResult> DSumarryIntakeExcel(DateTime DateFrom, DateTime DateTo, string Branch)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
                            worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
                .OrderBy(i => i.Sno.ToUpper());
            foreach (var emp in productIntakeobj)
            {
                var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                if (TransporterExist > 0)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
                        worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
    public IActionResult BranchIntakeAuditExcel(DateTime? DateFrom, DateTime? DateTo, string Branch)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
            decimal sum = 0, sum1 = 0, sum2 = 0;
            foreach (var branch in branchobj)
            {
                productIntakeobj = _context.ProductIntake
                    .Where(i => i.SaccoCode == sacco && i.TransDate >= DateFro && i.Qsupplied != 0
                    && i.TransDate <= DateT && i.Branch == branch.Bname && i.Description != "Transport")
                    .OrderByDescending(i => i.TransDate);
                var audituser = productIntakeobj.GroupBy(m => m.AuditId).ToList();
                audituser.ForEach(l =>
                {
                    var branchuser = l.FirstOrDefault();
                    currentRow++;
                    worksheet.Cell(currentRow, 2).Value = branchuser.AuditId;
                    var groupbydate = productIntakeobj.Where(k => k.AuditId == branchuser.AuditId)
                    .GroupBy(p => p.TransDate).ToList();
                    groupbydate.ForEach(j =>
                    {
                        var TransDate = j.FirstOrDefault();

                        productIntakeobj = productIntakeobj.Where(k => k.AuditId == branchuser.AuditId && k.TransDate == TransDate.TransDate)
                    .OrderByDescending(i => i.Auditdatetime).ToList();
                        foreach (var emp in productIntakeobj)
                        {
                            var TransporterExist = _context.DSuppliers.Where(u => u.Sno == emp.Sno).Count();
                            if (TransporterExist > 0)
                            {
                                currentRow++;
                                worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
                    worksheet.Cell(currentRow, 2).Value = branchuser.AuditId + " " + "Total Kgs";
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
    public async Task<IActionResult> TIntakeExcel(DateTime? DateFrom, DateTime? DateTo)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
            worksheet.Cell(currentRow, 5).Value = "Remarks";
            worksheet.Cell(currentRow, 6).Value = "Qsupplied";
            worksheet.Cell(currentRow, 7).Value = "Price";
            worksheet.Cell(currentRow, 8).Value = "Description";
            worksheet.Cell(currentRow, 9).Value = "CertNo";
            decimal sum = 0, totalkg = 0;

            var intakes = await _context.ProductIntake.Where(i => i.TransDate >= DateFrom && i.TransDate <= DateTo && i.SaccoCode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch).ToList();
            foreach (var emp in transporterobj)
            {
                productIntakeobj = intakes.Where(u => u.Sno == emp.TransCode && u.Qsupplied != 0);
                foreach (var empintake in productIntakeobj)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = empintake.Sno;
                    var transporters = transporterobj.FirstOrDefault(u => u.TransCode == empintake.Sno);

                    worksheet.Cell(currentRow, 2).Value = transporters?.TransName ?? "";
                    worksheet.Cell(currentRow, 3).Value = empintake.TransDate;
                    worksheet.Cell(currentRow, 4).Value = empintake.ProductType;
                    worksheet.Cell(currentRow, 5).Value = empintake.Remarks;
                    worksheet.Cell(currentRow, 6).Value = empintake.Qsupplied;
                    worksheet.Cell(currentRow, 7).Value = empintake.Ppu;
                    worksheet.Cell(currentRow, 8).Value = empintake.Description;
                    worksheet.Cell(currentRow, 9).Value = transporters?.CertNo ?? "";
                    sum += (empintake.Qsupplied);
                    totalkg += (empintake.Qsupplied);
                    }
                currentRow++;
                worksheet.Cell(currentRow, 5).Value = "Total Kgs";
                worksheet.Cell(currentRow, 6).Value = totalkg;
                totalkg = 0;
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

    public IActionResult TDeductionsExcel()
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
            worksheet.Cell(currentRow, 2).Value = "Transporters Deductions Report For: " + enddate.TransDate.ToString("dd/MM/yyy");

            currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "TCode";
            worksheet.Cell(currentRow, 2).Value = "Name";
            worksheet.Cell(currentRow, 3).Value = "TransDate";
            worksheet.Cell(currentRow, 4).Value = "ProductType";
            worksheet.Cell(currentRow, 5).Value = "Amount";
            worksheet.Cell(currentRow, 6).Value = "Remarks";
            worksheet.Cell(currentRow, 7).Value = "Station";
            decimal? sum = 0;

            productIntakeobj = productIntakeobj.OrderBy(p => p.Branch).ToList();
            var branches = productIntakeobj.GroupBy(b => b.Branch).ToList();
            branches.ForEach(s =>
            {


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
                            worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
    public async Task<IActionResult> DeductionSummary([Bind("DateFrom,DateTo")] FilterVm filter)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
        var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
        IQueryable<ProductIntake> productIntakes = _context.ProductIntake.Where(n => n.SaccoCode == sacco && n.Description != "Transport" 
        && n.TransactionType == TransactionType.Deduction && n.TransDate >= filter.DateFrom && n.TransDate <= filter.DateTo);
        var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco).ToListAsync();
        var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
        if (user.AccessLevel == AccessLevel.Branch) {
            productIntakes = productIntakes.Where(i => i.Branch == saccoBranch);
            suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();
        }

        var intakes =await productIntakes.OrderBy(i => i.Sno).ToListAsync();
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("productIntakeobj");
            var currentRow = 1;
            var company = _context.DCompanies.FirstOrDefault(u => u.Name == sacco);
            worksheet.Cell(currentRow, 2).Value = company.Name;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Adress;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Town;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = company.Email;

            currentRow = 5;
            worksheet.Cell(currentRow, 2).Value =$"Suppliers {filter.Code} Deductions Report For: {filter.DateTo.GetValueOrDefault().ToString("dd/MM/yyy")}";

            currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "SNo";
            worksheet.Cell(currentRow, 2).Value = "Name";
            worksheet.Cell(currentRow, 3).Value = "TransDate";
            worksheet.Cell(currentRow, 4).Value = "ProductType";
            worksheet.Cell(currentRow, 5).Value = "Amount";
            worksheet.Cell(currentRow, 6).Value = "Station";
            decimal? totalDeductions = 0;
            
            var groupedDeductionsByType = productIntakes.ToList().GroupBy(i => i.ProductType).ToList();
            groupedDeductionsByType.ForEach(d =>
            {
                var groupeddeductionsByFarmer = d.GroupBy(i => i.Sno).ToList();
                decimal? typeDeductions = 0;
                currentRow++;
                worksheet.Cell(currentRow, 2).Value = d.Key;
                groupeddeductionsByFarmer.ForEach(i =>
                {
                    var supplier = suppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(i.Key.ToUpper()));
                    if (supplier != null)
                    {
                        var deduction = i.FirstOrDefault();
                        currentRow++;
                        typeDeductions += i.Sum(s => s.DR);
                        worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
                        worksheet.Cell(currentRow, 1).Value = i.Key;
                        worksheet.Cell(currentRow, 2).Value = supplier?.Names ?? "";
                        worksheet.Cell(currentRow, 3).Value = filter.DateTo;
                        worksheet.Cell(currentRow, 4).Value = deduction.ProductType;
                        worksheet.Cell(currentRow, 5).Value = i.Sum(s => s.DR);
                        worksheet.Cell(currentRow, 6).Value = deduction.Branch;
                    }
                });

                currentRow++;
                worksheet.Cell(currentRow, 2).Value = "Totals ";
                worksheet.Cell(currentRow, 5).Value = typeDeductions;
                totalDeductions += typeDeductions;
            });

            currentRow++;
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = "Grand Total";
            worksheet.Cell(currentRow, 5).Value = totalDeductions;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Suppliers Deductions Summary.xlsx");
            }
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExportAllSuppliers(string County)
    {
        var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
        if (string.IsNullOrEmpty(loggedInUser))
            return Redirect("~/");
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
                    worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "@";
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
