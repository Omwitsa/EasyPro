using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyPro.Controllers
{
    public class ExcelUploadController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public ExcelUploadController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            return View();
        }

        public ActionResult Import()
        {
            utilities.SetUpPrivileges(this);
            string sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            string loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            string branch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string str_excel_grid = "";
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }

                    str_excel_grid = utilities.GenerateExcelGrid(sheet, sacco, loggedInUser, branch);
                }
            }
            return this.Content(str_excel_grid);
        }

        public ActionResult Download()
        {
            string Files = "wwwroot/UploadExcel/CoreProgramm_ExcelImport.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
            System.IO.File.WriteAllBytes(Files, fileBytes);
            MemoryStream ms = new MemoryStream(fileBytes);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "employee.xlsx");
        }

        public IActionResult Approve()
        {
            utilities.SetUpPrivileges(this);
            string sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            string loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            string saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var excelDumps = _context.ExcelDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();          
            excelDumps.ForEach(e =>
            {
                e.TransCode = e?.TransCode ?? "";
                var price = _context.DPrices.FirstOrDefault(p => p.Products.ToUpper().Equals(e.ProductType.ToUpper()) && p.SaccoCode == sacco);
                var productIntake = new ProductIntake
                {
                    Sno = e.Sno,
                    TransDate = e.TransDate,
                    TransTime = e.TransDate.TimeOfDay,
                    ProductType = e.ProductType,
                    Qsupplied = e.Quantity,
                    Ppu = price.Price,
                    CR = e.Quantity * price.Price,
                    DR = 0,
                    Description = "Intake",
                    TransactionType = TransactionType.Intake,
                    Paid = false,
                    Remarks = "",
                    AuditId = loggedInUser,
                    Auditdatetime = DateTime.Now,
                    Branch = saccoBranch,
                    Balance = 0,
                    SaccoCode = sacco,
                    CrAccNo = price.CrAccNo,
                    DrAccNo = price.DrAccNo
                };

                _context.ProductIntake.Add(productIntake);

                var transports = _context.DTransports.Where(t => t.Sno == e.Sno && t.Active
               && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
               && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    transports = transports.Where(t => t.Branch == saccoBranch);
                var transport = transports.FirstOrDefault();
                decimal? rate = 0;
                if (transport != null)
                {
                    transport.TransCode = transport?.TransCode ?? "";
                    if (transport.TransCode.Trim().ToUpper().Equals(e.TransCode.Trim().ToUpper()))
                    {
                        rate = transport.Rate;
                    }
                }

                if (!string.IsNullOrEmpty(e.TransCode))
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR = productIntake.Qsupplied * rate;
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = e.Sno,
                        TransDate = e.TransDate,
                        TransTime = e.TransDate.TimeOfDay,
                        ProductType = e.ProductType,
                        Qsupplied = e.Quantity,
                        Ppu = rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = 0,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = productIntake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = productIntake.DrAccNo,
                        CrAccNo = productIntake.CrAccNo,
                        Zone = productIntake.Zone
                    });


                    // Credit transpoter transport amount
                    productIntake.CR = productIntake.Qsupplied * rate;
                    productIntake.DR = 0;
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = e.TransCode.ToUpper().Trim(),
                        TransDate = e.TransDate,
                        TransTime = e.TransDate.TimeOfDay,
                        ProductType = e.ProductType,
                        Qsupplied = e.Quantity,
                        Ppu = rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = 0,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = productIntake.Remarks,
                        AuditId = loggedInUser,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = price.TransportDrAccNo,
                        CrAccNo = price.TransportCrAccNo,
                        Zone = productIntake.Zone
                    });
                }
            });

            if (excelDumps.Any())
            {
                _context.ExcelDump.RemoveRange(excelDumps);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
