using ClosedXML.Excel;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Repository;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public ReportController(IReporting iReporting, IReportProvider reportProvider, 
            MORINGAContext context)
        {
            _IReporting = iReporting;
            _reportProvider = reportProvider;
            utilities = new Utilities(context);
            _context = context;
        }

        public IEnumerable<DSupplier> suppliersobj { get; set; }
        public IEnumerable<ProductIntake> productIntakeobj { get; set; }
        public IEnumerable<DTransporter> transporterobj { get; set; }
        public IEnumerable<DTransportersPayRoll> transporterpayrollobj { get; set; }
        public IEnumerable<DPayroll> dpayrollobj { get; set; }
        public IEnumerable<DCompany> companyobj { get; set; }
        public IEnumerable<DBranch> branchobj { get; set; }

        [HttpGet]
        public IActionResult DownloadReport()
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
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var brances = _context.DBranch.Where(i => i.Bcode == sacco).Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);
        }

        [HttpPost]
        public IActionResult Suppliers([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            //var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            //var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
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
            //var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            //var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
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
        public IActionResult SuppliersPayrollPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            dpayrollobj = _context.DPayrolls
                .Where(p => p.EndofPeriod >= filter.DateFrom && p.EndofPeriod <= filter.DateTo && p.SaccoCode == sacco)
                .OrderBy(s => s.Sno);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = $"Suppliers Payroll Reports ({filter.DateTo})";
            var pdfFile = _reportProvider.GetSuppliersPayroll(dpayrollobj, company, title);
            return File(pdfFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult SuppliersActive([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= filter.DateFrom && i.TransDate <= filter.DateTo)
                .Select(i => i.Sno);
            suppliersobj = _context.DSuppliers
                .Where(u => u.Scode == sacco && activeSuppliers.Contains(u.Sno.ToString()))
                .OrderBy(u => u.Sno);
            return SuppliersActiveExcel(filter);
        }

        [HttpPost]
        public IActionResult SuppliersActivePdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var activeSuppliers = _context.ProductIntake.Where(i => i.TransDate >= filter.DateFrom && i.TransDate <= filter.DateTo)
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
        public IActionResult BranchIntakePdf([Bind("DateFrom,DateTo,Branch")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
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
        public IActionResult TransportersPayrollPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            transporterpayrollobj = _context.DTransportersPayRolls
                .Where(p => p.SaccoCode == sacco && p.EndPeriod >= filter.DateFrom && p.EndPeriod <= filter.DateTo)
                .OrderBy(p => p.Code);

            var company = _context.DCompanies.FirstOrDefault(c => c.Name == sacco);
            var title = $"Transporters Payroll Reports ({filter.DateTo})";
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
        public IActionResult DeductionsPdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied == 0 && u.SaccoCode == sacco);

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

            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied == 0 && u.SaccoCode == sacco);
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
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied != 0 && u.SaccoCode == sacco);
            return IntakeExcel();
        }

        [HttpPost]
        public IActionResult IntakePdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            //dSupplier.Scode = sacco;

            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied != 0 && u.SaccoCode == sacco);

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
        public IActionResult TIntakePdf([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
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
                foreach (var emp in transporterpayrollobj)
                    worksheet.Cell(currentRow, 4).Value = emp.EndPeriod;
                // SNo, Transport, Agrovet, Bonus,, HShares, Advance, TDeductions
                // , KgsSupplied, GPay,
                //Bank, AccountNumber, BBranch
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
                worksheet.Cell(currentRow, 11).Value = "Others";
                worksheet.Cell(currentRow, 12).Value = "Totaldeductions";
                worksheet.Cell(currentRow, 13).Value = "NPay";
                worksheet.Cell(currentRow, 14).Value = "Bank";
                worksheet.Cell(currentRow, 15).Value = "AccountNumber";
                worksheet.Cell(currentRow, 16).Value = "Branch";

                double? QntySup = 0;
                decimal? Subsidy = 0;
                decimal? GrossPay = 0;
                decimal? Hshares = 0;
                decimal? Advance = 0;
                decimal? Amnt = 0;
                decimal? Agrovet = 0;
                decimal? Others = 0;
                decimal? Totaldeductions = 0;
                decimal? Gpay = 0;
                decimal? Npay = 0;
                foreach (var emp in transporterpayrollobj)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Code;
                    //long.TryParse(emp.Sno, out long sno);
                    var TName = _context.DTransporters.Where(u => u.TransCode == emp.Code && u.ParentT == sacco);
                    foreach (var al in TName)
                    {
                        worksheet.Cell(currentRow, 2).Value = al.TransName;
                        worksheet.Cell(currentRow, 3).Value = al.CertNo;
                    }
                    worksheet.Cell(currentRow, 4).Value = emp.QntySup;
                    QntySup += (emp.QntySup);
                    worksheet.Cell(currentRow, 5).Value = emp.Amnt;
                    Amnt += (emp.Amnt);
                    worksheet.Cell(currentRow, 6).Value = emp.Subsidy;
                    Subsidy += (emp.Subsidy);
                    worksheet.Cell(currentRow, 7).Value = emp.GrossPay;
                    GrossPay += (emp.GrossPay);
                    worksheet.Cell(currentRow, 8).Value = emp.Agrovet;
                    Agrovet += (emp.Agrovet);
                    worksheet.Cell(currentRow, 9).Value = emp.Hshares;
                    Hshares += (emp.Hshares);
                    worksheet.Cell(currentRow, 10).Value = emp.Advance;
                    Advance += (emp.Advance);
                    worksheet.Cell(currentRow, 11).Value = emp.Others;
                    Others += (emp.Others);
                    worksheet.Cell(currentRow, 12).Value = emp.Totaldeductions;
                    Totaldeductions += (emp.Totaldeductions);
                    worksheet.Cell(currentRow, 13).Value = emp.NetPay;
                    Npay += (emp.NetPay);
                    worksheet.Cell(currentRow, 14).Value = emp.BankName;
                    worksheet.Cell(currentRow, 15).Value = emp.AccNo;
                    worksheet.Cell(currentRow, 16).Value = emp.Branch;
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
                worksheet.Cell(currentRow, 11).Value = Others;
                worksheet.Cell(currentRow, 12).Value = Totaldeductions;
                worksheet.Cell(currentRow, 13).Value = Npay;
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
                foreach (var emp in dpayrollobj)
                    worksheet.Cell(currentRow, 4).Value = emp.EndofPeriod;
                // SNo, Transport, Agrovet, Bonus,, HShares, Advance, TDeductions, KgsSupplied, GPay,
                //Bank, AccountNumber, BBranch
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
                worksheet.Cell(currentRow, 11).Value = "TDeductions";
                worksheet.Cell(currentRow, 12).Value = "KgsSupplied";
                worksheet.Cell(currentRow, 13).Value = "GPay";
                worksheet.Cell(currentRow, 14).Value = "NPay";
                worksheet.Cell(currentRow, 15).Value = "Bank";
                worksheet.Cell(currentRow, 16).Value = "AccountNumber";
                worksheet.Cell(currentRow, 17).Value = "BBranch";

                decimal? Transport = 0;
                decimal? Agrovet = 0;
                decimal? Bonus = 0;
                decimal? Hshares = 0;
                decimal? Advance = 0;
                decimal? Midmonth = 0;
                decimal? Others = 0;
                decimal? Tdeductions = 0;
                double? KgsSupplied = 0;
                decimal? Gpay = 0;
                decimal? Npay = 0;
                foreach (var emp in dpayrollobj)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Sno;
                    //long.TryParse(emp.Sno, out long sno);
                    var TName = _context.DSuppliers.Where(u => u.Sno == emp.Sno && u.Scode == sacco);
                    foreach (var al in TName)
                        worksheet.Cell(currentRow, 2).Value = al.Names;
                    worksheet.Cell(currentRow, 3).Value = emp.IdNo;
                    worksheet.Cell(currentRow, 4).Value = emp.Transport;
                    Transport += (emp.Transport);
                    worksheet.Cell(currentRow, 5).Value = emp.Agrovet;
                    Agrovet += (emp.Agrovet);
                    worksheet.Cell(currentRow, 6).Value = emp.Bonus;
                    Bonus += (emp.Bonus);
                    worksheet.Cell(currentRow, 7).Value = emp.Hshares;
                    Hshares += (emp.Hshares);
                    worksheet.Cell(currentRow, 8).Value = emp.Advance;
                    Advance += (emp.Advance);
                    worksheet.Cell(currentRow, 9).Value = emp.Midmonth;
                    Midmonth += (emp.Midmonth);
                    worksheet.Cell(currentRow, 10).Value = emp.Others;
                    Others += (emp.Others);
                    worksheet.Cell(currentRow, 11).Value = emp.Tdeductions;
                    Tdeductions += (emp.Tdeductions);
                    worksheet.Cell(currentRow, 12).Value = emp.KgsSupplied;
                    KgsSupplied += (emp.KgsSupplied);
                    worksheet.Cell(currentRow, 13).Value = emp.Gpay;
                    Gpay += (emp.Gpay);
                    worksheet.Cell(currentRow, 14).Value = emp.Npay;
                    Npay += (emp.Npay);
                    worksheet.Cell(currentRow, 15).Value = emp.Bank;
                    worksheet.Cell(currentRow, 16).Value = emp.AccountNumber;
                    worksheet.Cell(currentRow, 17).Value = emp.Bbranch;
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
                worksheet.Cell(currentRow, 11).Value = Tdeductions;
                worksheet.Cell(currentRow, 12).Value = KgsSupplied;
                worksheet.Cell(currentRow, 13).Value = Gpay;
                worksheet.Cell(currentRow, 14).Value = Npay;
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
                var Total = 0;
                foreach (var emp in suppliersobj)
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
                    Total += 1;
                }
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "No Of Suppliers";
                worksheet.Cell(currentRow, 2).Value = Total;
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

                foreach (var emp in suppliersobj)
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

                }
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
                    long.TryParse(emp.Sno, out long sno);
                    var checkifexist = _context.DSuppliers.Where(u => u.Sno == sno && u.Scode == sacco);
                    if (checkifexist.Any())
                    {


                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;

                        var TName = _context.DSuppliers.Where(u => u.Sno == sno && u.Scode == sacco);
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
                        && i.Qsupplied != 0 && i.TransDate <= DateT && i.Branch == branch.Bname);
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
                            && i.Qsupplied != 0 && i.TransDate <= datereceived && i.Branch == branch.Bname).Sum(s => s.Qsupplied);
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
                        && i.TransDate <= DateT && i.Branch == branch.Bname)
                        .OrderBy(i => i.Sno);
                    foreach (var emp in productIntakeobj)
                    {
                        long.TryParse(emp.Sno, out long sno);
                        var TransporterExist = _context.DSuppliers.Where(u => u.Sno == sno).Count();
                        if (TransporterExist > 0)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = emp.Sno;
                            var TName = _context.DSuppliers.Where(u => u.Sno == sno && u.Scode == sacco);
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
        public IActionResult TIntakeExcel(DateTime DateFrom, DateTime DateTo)
        {
            var DateFro = Convert.ToDateTime(DateFrom.ToString());
            var DateT = Convert.ToDateTime(DateTo.ToString());
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
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
                worksheet.Cell(currentRow, 5).Value = "Qsupplied";
                worksheet.Cell(currentRow, 6).Value = "Price";
                worksheet.Cell(currentRow, 7).Value = "Description";
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
                            var TName = _context.DTransporters.Where(u => u.TransCode == empintake.Sno && u.ParentT == sacco);
                            foreach (var ann in TName)
                                worksheet.Cell(currentRow, 2).Value = ann.TransName;
                            worksheet.Cell(currentRow, 3).Value = empintake.TransDate;
                            worksheet.Cell(currentRow, 4).Value = empintake.ProductType;
                            worksheet.Cell(currentRow, 5).Value = empintake.Qsupplied;
                            worksheet.Cell(currentRow, 6).Value = empintake.Ppu;
                            worksheet.Cell(currentRow, 7).Value = empintake.Description;
                            sum += (empintake.Qsupplied);
                            totalkg += (empintake.Qsupplied);
                        }
                        currentRow++;
                        worksheet.Cell(currentRow, 4).Value = "Total Kgs";
                        worksheet.Cell(currentRow, 5).Value = totalkg;
                        totalkg = 0;
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
                worksheet.Cell(currentRow, 2).Value = "Suppliers Deductions Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "SNo";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Remarks";
                decimal? sum = 0;
                foreach (var emp in productIntakeobj)
                {
                    long.TryParse(emp.Sno, out long sno);
                    var TransporterExist = _context.DSuppliers.Where(u => u.Sno == sno).Count();
                    if (TransporterExist > 0)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        var TName = _context.DSuppliers.Where(u => u.Sno == sno && u.Scode == sacco);
                        foreach (var ann in TName)
                            worksheet.Cell(currentRow, 2).Value = ann.Names;
                        worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                        worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                        worksheet.Cell(currentRow, 5).Value = emp.DR;
                        worksheet.Cell(currentRow, 6).Value = emp.Remarks;
                        sum += (emp.DR);
                    }
                }
                currentRow++;
                worksheet.Cell(currentRow, 4).Value = "Total Amount";
                worksheet.Cell(currentRow, 5).Value = sum;
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
                worksheet.Cell(currentRow, 2).Value = "Transporters Deductions Report";

                currentRow = 6;
                worksheet.Cell(currentRow, 1).Value = "TCode";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "TransDate";
                worksheet.Cell(currentRow, 4).Value = "ProductType";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Remarks";
                decimal? sum = 0;
                foreach (var emp in productIntakeobj)
                {
                    var TransporterExist = _context.DTransporters.Where(u => u.TransCode == emp.Sno).Count();
                    if (TransporterExist > 0)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = emp.Sno;
                        var TName = _context.DTransporters.Where(u => u.TransCode == emp.Sno && u.ParentT == sacco);
                        foreach (var ann in TName)
                            worksheet.Cell(currentRow, 2).Value = ann.TransName;
                        worksheet.Cell(currentRow, 3).Value = emp.TransDate;
                        worksheet.Cell(currentRow, 4).Value = emp.ProductType;
                        worksheet.Cell(currentRow, 5).Value = emp.DR;
                        worksheet.Cell(currentRow, 6).Value = emp.Remarks.ToString();
                        sum += (emp.DR);
                    }
                }
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
    }
}
