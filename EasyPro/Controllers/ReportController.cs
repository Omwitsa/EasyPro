using ClosedXML.Excel;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Repository;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        readonly IReporting _IReporting;
        private Utilities utilities;

        public ReportController(IReporting iReporting, MORINGAContext context)
        {
            _IReporting = iReporting;
            utilities = new Utilities(context);
            _context = context;
        }
        public IEnumerable<DSupplier> suppliersobj { get; set; }
        public IEnumerable<ProductIntake> productIntakeobj { get; set; }
        public IEnumerable<DTransporter> transporterobj { get; set; }
        public IEnumerable<DPayroll> dpayrollobj { get; set; }
        public IEnumerable<DCompany> companyobj { get; set; }

        [HttpGet]
        public IActionResult DownloadReport()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }
        [HttpGet]
        public IActionResult DownloadTReport()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }
        [HttpGet]
        public IActionResult DownloadPReport()
        {
            utilities.SetUpPrivileges(this);
            return View();
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
        public IActionResult SuppliersPayrollExcel([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            dpayrollobj = _context.DPayrolls; //.Where(u => u.ParentT == sacco);
            return suppliersPayrollExcel();
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
        public IActionResult TIntake([Bind("DateFrom,DateTo")] FilterVm filter)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            //dSupplier.Scode = sacco;

            var DateFrom = Convert.ToDateTime(filter.DateFrom.ToString());
            var DateTo = Convert.ToDateTime(filter.DateTo.ToString());
            //productIntakeobj = _context.ProductIntake.Where(u => u.TransDate >= DateFrom && u.TransDate <= DateTo && u.Qsupplied != 0 && u.SaccoCode == sacco);
            return TIntakeExcel(DateFrom, DateTo);
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
                worksheet.Cell(currentRow, 10).Value = "TDeductions";
                worksheet.Cell(currentRow, 11).Value = "KgsSupplied";
                worksheet.Cell(currentRow, 12).Value = "GPay";
                worksheet.Cell(currentRow, 13).Value = "NPay";
                worksheet.Cell(currentRow, 14).Value = "Bank";
                worksheet.Cell(currentRow, 15).Value = "AccountNumber";
                worksheet.Cell(currentRow, 16).Value = "BBranch";

                decimal? Transport = 0;
                decimal? Agrovet = 0;
                decimal? Bonus = 0;
                decimal? Hshares = 0;
                decimal? Advance = 0;
                decimal? Midmonth = 0;
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
                    worksheet.Cell(currentRow, 10).Value = emp.Tdeductions;
                    Tdeductions += (emp.Tdeductions);
                    worksheet.Cell(currentRow, 11).Value = emp.KgsSupplied;
                    KgsSupplied += (emp.KgsSupplied);
                    worksheet.Cell(currentRow, 12).Value = emp.Gpay;
                    Gpay += (emp.Gpay);
                    worksheet.Cell(currentRow, 13).Value = emp.Npay;
                    Npay += (emp.Npay);
                    worksheet.Cell(currentRow, 14).Value = emp.Bank;
                    worksheet.Cell(currentRow, 15).Value = emp.AccountNumber;
                    worksheet.Cell(currentRow, 16).Value = emp.Bbranch;
                }
                currentRow++;
                worksheet.Cell(currentRow, 3).Value = "Total";
                worksheet.Cell(currentRow, 4).Value = Transport;
                worksheet.Cell(currentRow, 5).Value = Agrovet;
                worksheet.Cell(currentRow, 6).Value = Bonus;
                worksheet.Cell(currentRow, 7).Value = Hshares;
                worksheet.Cell(currentRow, 8).Value = Advance;
                worksheet.Cell(currentRow, 9).Value = Midmonth;
                worksheet.Cell(currentRow, 10).Value = Tdeductions;
                worksheet.Cell(currentRow, 11).Value = KgsSupplied;
                worksheet.Cell(currentRow, 12).Value = Gpay;
                worksheet.Cell(currentRow, 13).Value = Npay;
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
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Sno;
                    long.TryParse(emp.Sno, out long sno);
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
                decimal sum = 0,totalkg=0;
                var AllTransporters = _context.DTransporters;
                foreach (var empt in AllTransporters)
                {
                    transporterobj = _context.DTransporters.Where(u => u.TransCode == empt.TransCode && u.ParentT == sacco);
                    foreach (var emp in transporterobj)
                    {
                        var TransporterExist = _context.ProductIntake.Where(u => u.Sno == empt.TransCode && u.SaccoCode == sacco).Count();
                        if (TransporterExist > 0)
                        {
                            productIntakeobj = _context.ProductIntake.Where(u => u.Sno== empt.TransCode && u.TransDate >= DateFro && u.TransDate <= DateT && u.Qsupplied != 0 && u.SaccoCode == sacco);
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
