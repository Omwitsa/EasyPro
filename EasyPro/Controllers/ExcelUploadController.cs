using AspNetCoreHero.ToastNotification.Abstractions;
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

                    str_excel_grid = utilities.GenerateExcelGrid(sheet, sacco, loggedInUser);
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

            var excelDumps = _context.ExcelDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
            excelDumps.ForEach(e =>
            {
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
                    Branch = "",
                    SaccoCode = sacco,
                    CrAccNo = price.CrAccNo,
                    DrAccNo = price.DrAccNo
                };

                var balance = utilities.GetBalance(productIntake);
                productIntake.Balance = balance;

                _context.ProductIntake.Add(productIntake);
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
