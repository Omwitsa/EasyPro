using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult SuppliersImportIndex()
        {
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            var counties = _context.DCompanies.OrderByDescending(K => K.Id).ToList();
            if (!loggedInUser.ToLower().Equals("psigei"))
                counties = _context.DCompanies.Where(i => i.Name.ToUpper().Equals(sacco.ToUpper())).ToList();

            ViewBag.scode = new SelectList(counties.OrderBy(n => n.Name).ToList().Select(b => b.Name).ToList());

            return View();
        }
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";

            return View();
        }
        public ActionResult RegImport(String scodes)
        {
            utilities.SetUpPrivileges(this);
            string sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            string loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            string branch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            if (loggedInUser.ToLower().Equals("psigei"))
            {
                var getbranch = _context.DBranch.FirstOrDefault(n => n.Bcode == scodes);
                sacco = scodes;
                branch = getbranch?.Bname ?? "MAIN";
            }

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

                    str_excel_grid = utilities.GenerateExcelGridSupReg(sheet, sacco, loggedInUser, branch);
                }
            }
            return this.Content(str_excel_grid);
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
        public ActionResult RegDownload()
        { 
            string Files = "wwwroot/UploadExcel/CoreProgramm_ExcelImportSuppliers.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
            System.IO.File.WriteAllBytes(Files, fileBytes);
            MemoryStream ms = new MemoryStream(fileBytes);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "registration.xlsx");
        }
        public ActionResult Download()
        { 
            string Files = "wwwroot/UploadExcel/CoreProgramm_ExcelImport.xlsx";
            byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
            System.IO.File.WriteAllBytes(Files, fileBytes);
            MemoryStream ms = new MemoryStream(fileBytes);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "collection.xlsx");
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
                var cr = e.Quantity * price.Price;
                var dr = 0M;
                var productIntake = new ProductIntake
                {
                    Sno = e.Sno,
                    TransDate = e.TransDate,
                    TransTime = e.TransDate.TimeOfDay,
                    ProductType = e.ProductType,
                    Qsupplied = e.Quantity,
                    Ppu = price.Price,
                    CR = cr,
                    DR = dr,
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
                if (!transports.Any())
                {
                    transports = _context.DTransports.Where(t => t.TransCode == e.TransCode
                       && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
                       && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()));
                }
                var transport = transports.FirstOrDefault();
                decimal? rate = 1;
                if (transport != null)
                    rate = transport.Rate;
                
                var isMburugu = sacco == StrValues.Mburugu;
                if (!string.IsNullOrEmpty(e.TransCode) || isMburugu)
                {
                    // Debit supplier transport amount
                    cr = 0;
                    dr = (decimal)(e.Quantity * rate);
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = e.Sno,
                        TransDate = e.TransDate,
                        TransTime = e.TransDate.TimeOfDay,
                        ProductType = e.ProductType,
                        Qsupplied = e.Quantity,
                        Ppu = rate,
                        CR = cr,
                        DR = dr,
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
                    cr = (decimal)(e.Quantity * rate);
                    dr = 0;
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = e.TransCode.ToUpper().Trim(),
                        TransDate = e.TransDate,
                        TransTime = e.TransDate.TimeOfDay,
                        ProductType = e.ProductType,
                        Qsupplied = e.Quantity,
                        Ppu = rate,
                        CR = cr,
                        DR = dr,
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

        public IActionResult DeductionUploadForm()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        public ActionResult ImportDeductions()
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

                    str_excel_grid = utilities.GenerateDeductionsExcelGrid(sheet, sacco, loggedInUser, branch);
                }
            }
            return this.Content(str_excel_grid);
        }

        public IActionResult ApproveUploadedDeduction()
        {
            utilities.SetUpPrivileges(this);
            string sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            string loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            string saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            var excelDumps = _context.ExcelDeductionDump.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
            excelDumps.ForEach(e =>
            {
                var productIntake = new ProductIntake
                {
                    Sno = e.Sno,
                    TransDate = e.TransDate,
                    TransTime = e.TransDate.TimeOfDay,
                    ProductType = e.ProductType,
                    Qsupplied = 0,
                    Ppu = 0,
                    CR = 0,
                    DR = e.Amount,
                    Description = "Deductions",
                    TransactionType = TransactionType.Deduction,
                    Paid = false,
                    Remarks = "Upload",
                    AuditId = loggedInUser,
                    Auditdatetime = DateTime.Now,
                    Branch = saccoBranch,
                    Balance = 0,
                    SaccoCode = sacco,
                    CrAccNo = null,
                    DrAccNo = null
                };

                _context.ProductIntake.Add(productIntake);
            });

            if (excelDumps.Any())
            {
                _context.ExcelDeductionDump.RemoveRange(excelDumps);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult RegApprove(string scodes)
        {
            utilities.SetUpPrivileges(this);
            string sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            string saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            string loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            if (loggedInUser.ToLower().Equals("psigei"))
            {
                var getbranch = _context.DBranch.FirstOrDefault(n => n.Bcode == scodes);
                sacco = scodes;
                saccoBranch = getbranch?.Bname ?? "MAIN";
            }


            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var excelDumps = _context.ExcelDumpSupReg.Where(d => d.LoggedInUser == loggedInUser && d.SaccoCode == sacco).ToList();
            var datatoupload = excelDumps.GroupBy(p => p.SNo.ToUpper()).ToList();
            datatoupload.ForEach(e =>
            {
                var getsno = e.FirstOrDefault();
                var productIntake = new DSupplier
                {
                    Sno = getsno.SNo,
                    Regdate = getsno.Reg_date,
                    Names = getsno.Names,
                    PhoneNo = getsno.PhoneNo,
                    IdNo = getsno.IdNo,
                    Dob = getsno.DOB,
                    AccNo = getsno.Acc_Number,
                    Bcode = getsno.Bank_code,
                    Bbranch = getsno.Bank_Branch,
                    Type = getsno.Gender,
                    TransCode = getsno.PaymentMode,
                    Village = getsno.Village,
                    Location = getsno.LOCATION,
                    Division = getsno.WARD,
                    District = getsno.SUB_COUNTY,
                    County = getsno.COUNTY,
                    Trader =false,
                    Active =true,
                    Approval= true,
                    Address="0",
                    Town="",
                    Email = "",
                    AuditId = loggedInUser,
                    Auditdatetime = DateTime.Now,
                    Branch = saccoBranch,
                    Scode = sacco,
                    Loan= false,
                    Compare="0",
                    Isfrate = "0",
                    Frate="0",
                    Rate ="0",
                    Hast= 0,
                    Br= "0",
                    Mno = "0",
                    Branchcode =0,
                    HasNursery ="0", 
                    Notrees =0, 
                    Aarno = "0",
                    Tmd = DateTime.Now,
                    Landsize = 0,
                    Thcpactive = 0,
                    Thcppremium = 0,
                    Status = "0",
                    Status2 = 0,
                    Status3 =0,
                    Status4 = 0,
                    Status5 = 0,
                    Status6 = "0",
                    Types = "0",
                    Freezed = "0",
                    Mass = "0",
                    Status1= 0,
                    Run =0,
                    Zone ="",
                };
                _context.DSuppliers.Add(productIntake);
            });
            
            if (excelDumps.Any())
            {
                _context.ExcelDumpSupReg.RemoveRange(excelDumps);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(SuppliersImportIndex));
        }
    }
}
