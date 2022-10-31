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
using System.Text;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class ProductIntakesController : Controller
    {
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
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            //return View(await _context.ProductIntake
            //    .Where(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) 
            //    && i.TransDate == DateTime.Today && i.Branch == saccoBranch)
            //    .ToListAsync());

            return View(await _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && i.TransDate == DateTime.Today && i.Branch== saccoBranch)
                .ToListAsync());

        }
        
        public async Task<IActionResult> TDeductionList()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction 
            && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today
            && c.Branch == saccoBranch).ToListAsync();
            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                var supplier = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.Sno && i.ParentT == sacco && i.Tbranch == saccoBranch);
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction 
            && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today
            && c.Branch == saccoBranch).ToListAsync();
            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                long.TryParse(intake.Sno, out long sno);
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == sno && i.Scode == sacco && i.Branch == saccoBranch);
                if (supplier != null)
                {
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
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.ProductIntake
                .Where(c => c.TransactionType == TransactionType.Correction && c.SaccoCode.ToUpper().Equals(sacco.ToUpper()) 
                && c.TransDate == DateTime.Today && c.Branch == saccoBranch)
                .ToListAsync());
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
                        Name = emploeyeename.Surname+" "+ emploeyeename.Othernames,
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
            var Todayskg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today).Sum(p => p.Qsupplied);
            var TodaysBranchkg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == DateTime.Today && s.Branch==saccoBranch).Sum(p => p.Qsupplied);
            return View(new ProductIntakeVm { 
                Todaykgs= Todayskg,
                TodayBranchkgs= TodaysBranchkg
            });
        }

        private void SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var suppliers = _context.DSuppliers
                .Where(s=>s.Scode.ToUpper().Equals(sacco.ToUpper()) && s.Branch == saccoBranch).ToList();
            ViewBag.suppliers = suppliers;
            var products = _context.DPrices.Where(s=>s.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.products = new SelectList(products, "Products", "Products");
            ViewBag.productPrices = products;

            var Branch = _context.DBranch.Where(i=>i.Bcode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.Branch = new SelectList(Branch, "BName", "BName");
            ViewBag.Branch = Branch;

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

        }
        
        public IActionResult CreateDeduction()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            Farmersobj = new FarmersVM()
            {
                DSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco && s.Branch == saccoBranch),
                ProductIntake = new ProductIntake
                {
                    TransDate = DateTime.Today
                }
            };
            //return Json(new { data = Farmersobj });
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var Descriptionname = _context.DDcodes.Where(d => d.Dcode == sacco).Select(b => b.Description).ToList();
            ViewBag.Description = new SelectList(Descriptionname);

            var brances = _context.DBranch.Where(b => b.Bcode == sacco).Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

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
            employees.ForEach(e => {
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
            var Todayskg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) &&( s.Description == "Intake" || s.Description== "Correction") && s.TransDate == DateTime.Today).Sum(p => p.Qsupplied);
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
        public async Task<IActionResult> Create([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch,DrAccNo,CrAccNo,Print,SMS,Zone")] ProductIntakeVm productIntake)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            productIntake.Branch = saccoBranch;
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.Description = productIntake?.Description ?? "";
            long.TryParse(productIntake.Sno, out long sno);
            if (sno < 1)
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return RedirectToAction(nameof(Create));
            }
            if (!_context.DSuppliers.Any(s => s.Sno == sno && s.Scode.ToUpper().Equals(sacco.ToUpper())
            && s.Zone== productIntake.Zone && s.Branch == saccoBranch))
            {
                _notyf.Error("Sorry, Supplier No. not found");
                return RedirectToAction(nameof(Create));
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return RedirectToAction(nameof(Create));
            }
            if (productIntake.Qsupplied < 1)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return RedirectToAction(nameof(Create));
            }
            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno && s.Scode.ToUpper().Equals(sacco.ToUpper())
            && s.Zone == productIntake.Zone && s.Branch == saccoBranch);
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return RedirectToAction(nameof(Create));
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return RedirectToAction(nameof(Create));
            }
            if (ModelState.IsValid)
            {
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                productIntake.SaccoCode = sacco;
                productIntake.TransactionType = TransactionType.Intake;
                var price = _context.DPrices
                    .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                    && p.Products.ToUpper().Equals(productIntake.ProductType.ToUpper()));

                var collection = new ProductIntake
                {
                    Sno = productIntake.Sno.Trim(),
                    TransDate = DateTime.Today,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productIntake.ProductType,
                    Qsupplied = (decimal)productIntake.Qsupplied,
                    Ppu = price.Price,
                    CR = productIntake.CR,
                    DR = productIntake.DR,
                    Balance = productIntake.Balance,
                    Description = "Intake",
                    TransactionType = productIntake.TransactionType,
                    Remarks = productIntake.Remarks,
                    AuditId = auditId,
                    Auditdatetime = productIntake.Auditdatetime,
                    Branch = productIntake.Branch,
                    SaccoCode = productIntake.SaccoCode,
                    DrAccNo = productIntake.DrAccNo,
                    CrAccNo = productIntake.CrAccNo,
                    Zone = productIntake.Zone
                };
                _context.ProductIntake.Add(collection);

                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == sno && t.Active
                && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
                && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()) && t.Branch==saccoBranch);
               
                if (transport != null)
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR = productIntake.Qsupplied * transport.Rate;
                    productIntake.Balance = productIntake.Balance - productIntake.DR;
                    collection = new ProductIntake
                    {
                        Sno = productIntake.Sno.Trim(),
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
                        AuditId = auditId,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = productIntake.DrAccNo,
                        CrAccNo = productIntake.CrAccNo,
                        Zone=productIntake.Zone
                    };
                    _context.ProductIntake.Add(collection);

                    // Credit transpoter transport amount
                    productIntake.CR = productIntake.Qsupplied * transport.Rate;
                    productIntake.DR = 0;
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = transport.TransCode.Trim(),
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
                        AuditId = auditId,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = price.TransportDrAccNo,
                        CrAccNo = price.TransportCrAccNo,
                        Zone=productIntake.Zone
                    });
                }

                if (productIntake.SMS)
                {
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var commulated = _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                    && s.TransDate >= startDate && s.TransDate <= endDate && s.Branch == saccoBranch && s.Zone==productIntake.Zone).Sum(s => s.Qsupplied);
                    var note = "";
                    if (productIntake.ProductType.ToLower().Equals("milk"))
                        note = "Kindly observe withdrawal period after cow treatment";
                    _context.Messages.Add(new Message
                    {
                        Telephone = supplier.PhoneNo,
                        Content = $"You have supplied {productIntake.Qsupplied} kgs to {sacco}. Your commulated {commulated + productIntake.Qsupplied}\n {note}",
                        ProcessTime = DateTime.Now.ToString(),
                        MsgType = "Outbox",
                        Replied = false,
                        DateReceived = DateTime.Now,
                        Source = auditId,
                        Code = sacco
                    });
                }

                _context.SaveChanges();
                _notyf.Success("Intake saved successfully");
                //PrintReceiptForTransaction();
                if (productIntake.Print)
                    return RedirectToAction("GetIntakeReceipt", "PdfReport", new { id = collection.Id });
            }

            return RedirectToAction(nameof(Create));
        }
        //public IActionResult PrintReceiptForTransaction()
        //{
        //    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
        //    PrintDocument recordDoc = new PrintDocument();
        //    recordDoc.DocumentName = "Customer Receipt";
        //   // PrintPageEventHandler ReceiptPrinter = null;
        //    //recordDoc.PrintPage += new PrintPageEventHandler(ReceiptPrinter); // function below
        //    recordDoc.PrintController = new StandardPrintController(); // hides status dialog popup
        //                                                               // Comment if debugging 
        //    PrinterSettings ps = new PrinterSettings();
        //    ps.PrinterName = "E-PoS 80mm Thermal Printer";
        //    recordDoc.PrinterSettings = ps;
        //    Encoding enc = Encoding.ASCII;
        //    string GS = Convert.ToString((char)29);
        //    string ESC = Convert.ToString((char)27);
        //    string COMMAND = "";
        //    COMMAND = ESC + "@";
        //    COMMAND += GS + "V" + (char)1;
        //    //byte[] bse = 
        //    char[] bse = COMMAND.ToCharArray();
        //    byte[] paperCut = enc.GetBytes(bse);
        //    // Line feed hexadecimal values
        //    byte[] bEsc = new byte[4];
        //    // Sends an ESC/POS command to the printer to cut the paper
        //    string t = ("                  " + sacco.ToUpper() + "\r\n");
        //    t = t + ("----------------------------------------\r\n");
        //    t = t + ("Table:  Table-C           BillNo: 120 \r\n");
        //    t = t + ("----------------------------------------\r\n");
        //    t = t + ("Date :2022/01/21  Order: Sylvia \r\n");
        //    t = t + ("=======================================\r\n");
        //    t = t + ("\r\n");
        //    t = t + (" SN. 1   Item: MoMo         Qty: 2   \r\n");
        //    char[] array = t.ToCharArray();
        //    byte[] byData = enc.GetBytes(array);
        //    recordDoc.Print();
        //    return Ok(200);
        //    // --------------------------------------
        //    //Socket clientSock = new Socket(
        //    //    AddressFamily.InterNetwork,
        //    //    SocketType.Stream,
        //    //    ProtocolType.Tcp
        //    //    );
        //    //IPAddress ip = IPAddress.Parse("192.168.100.200");
        //    //IPEndPoint name = new IPEndPoint(ip, 4730);
        //    //clientSock.Connect(name);
        //    //if (!clientSock.Connected)
        //    //{
        //    //    return BadRequest("Printer is not connected");
        //    //}
            
        //    //clientSock.Send(byData);
        //    //clientSock.Send(paperCut);
        //    ////clientSock.DuplicateAndClose(2);
        //    //clientSock.Close();
        //    //return Ok(200);

        //    // --------------------------------------

        //}

        //private static void PrintReceiptPage(object sender, PrintPageEventArgs e)
        //{
        //    float x = 10;
        //    float y = 5;
        //    float width = 270.0F; // max width I found through trial and error
        //    float height = 0F;

        //    Font drawFontArial12Bold = new Font("Arial", 12, FontStyle.Bold);
        //    Font drawFontArial10Regular = new Font("Arial", 10, FontStyle.Regular);
        //    SolidBrush drawBrush = new SolidBrush(Color.Black);

        //    // Set format of string.
        //    StringFormat drawFormatCenter = new StringFormat();
        //    drawFormatCenter.Alignment = StringAlignment.Center;
        //    StringFormat drawFormatLeft = new StringFormat();
        //    drawFormatLeft.Alignment = StringAlignment.Near;
        //    StringFormat drawFormatRight = new StringFormat();
        //    drawFormatRight.Alignment = StringAlignment.Far;

        //    // Draw string to screen.
        //    string text = "Company Name";
        //    e.Graphics.DrawString(text, drawFontArial12Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
        //    y += e.Graphics.MeasureString(text, drawFontArial12Bold).Height;

        //    text = "Address";
        //    e.Graphics.DrawString(text, drawFontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
        //    y += e.Graphics.MeasureString(text, drawFontArial10Regular).Height;

        //    // ... and so on
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch,Zone")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            long.TryParse(productIntake.Sno, out long sno);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            DateTime startdate = new DateTime(productIntake.TransDate.Year, productIntake.TransDate.Month, 1);
            DateTime enddate = startdate.AddMonths(1).AddDays(-1);
            productIntake.Description = productIntake?.Description ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (!_context.DSuppliers.Any(i => i.Sno == sno && i.Scode == sacco && i.Active == true && i.Approval == true
            && i.Branch == saccoBranch && i.Zone== productIntake.Zone))
            {
                _notyf.Error("Sorry, Supplier Number code does not exist");
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = _context.DSuppliers,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (!_context.ProductIntake.Any(i => i.Sno == productIntake.Sno && i.SaccoCode == sacco && i.Qsupplied!= 0 
            && i.TransDate >= startdate && i.TransDate <= enddate && i.Branch == saccoBranch && i.Zone == productIntake.Zone))
            {
                _notyf.Error("Sorry, Supplier has not deliver any product for this month"+" "+ startdate+ "To " + " "+ enddate );
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco && s.Branch ==saccoBranch),
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (sno == 0)
            {
                GetInitialValues();
                _notyf.Error("Sorry, Farmer code cannot be zero");
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco && s.Branch == saccoBranch),
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
                productIntake.TransDate = DateTime.Today;
                productIntake.SaccoCode = sacco;
                productIntake.Qsupplied = 0;
                productIntake.CR = 0;
                productIntake.Description = productIntake.Remarks;
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
                productIntake.Description = productIntake.Remarks;
                productIntake.TransactionType = TransactionType.Deduction;
                productIntake.SaccoCode = sacco;
                productIntake.Balance = utilities.GetBalance(productIntake);
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
            if (!_context.Employees.Any(i => i.EmpNo == employeesDed.Empno && i.SaccoCode == sacco ))
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
        public async Task<IActionResult> CreateCorrection([Bind("Id, Sno, TransDate, ProductType, Qsupplied, Ppu, CR, DR, Balance, Description, Remarks, AuditId, Auditdatetime, Branch, DrAccNo, CrAccNo, Print, SMS,Zone")] ProductIntakeVm productIntake)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            productIntake.SaccoCode = sacco;
            //var branch = _context.DBranch.FirstOrDefault(b => b.Bcode.ToUpper().Equals(sacco.ToUpper()));
            productIntake.Branch = saccoBranch;
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.Description = productIntake?.Description ?? "";
            productIntake.DR = 0;
            long.TryParse(productIntake.Sno, out long sno);
            
            if (sno < 1)
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return RedirectToAction(nameof(CreateCorrection));
            }
            if (!_context.DSuppliers.Any(s => s.Sno == sno && s.Scode == sacco && s.Zone==productIntake.Zone && s.Branch == saccoBranch))
            {
                _notyf.Error("Sorry, Supplier No. not found");
                return RedirectToAction(nameof(CreateCorrection));
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return RedirectToAction(nameof(CreateCorrection));
            }

            if (productIntake.Qsupplied == 0)
            {
                //_notyf.Error("Sorry, Kindly provide quantity");
                return RedirectToAction(nameof(CreateCorrection));
            }
            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno && s.Scode == sacco && s.Zone == productIntake.Zone && s.Branch == saccoBranch);
            if (supplier == null) 
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return RedirectToAction(nameof(CreateCorrection));
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return RedirectToAction(nameof(CreateCorrection));
            }
            double ch = 0;
            if (productIntake.CR < 0)
            {
                productIntake.DR = (productIntake.CR)*-1;
                ch = (double)productIntake.CR;
                productIntake.CR = 0;
            }
            if (productIntake.DrAccNo == null)
            {
                productIntake.DrAccNo = "0";
            }
            if (productIntake.CrAccNo == null)
            {
                productIntake.CrAccNo = "0";
            }

           

            if (ModelState.IsValid)
            { 
                productIntake.AuditId = auditId ?? "";
                productIntake.Description = "Correction";
                productIntake.TransactionType = TransactionType.Correction;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Auditdatetime = DateTime.Now;
                //productIntake.Balance = utilities.GetBalance(productIntake);
                //_context.Add(productIntake);


                var price = _context.DPrices
                    .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                    && p.Products.ToUpper().Equals(productIntake.ProductType.ToUpper()));
                var collection = new ProductIntake
                {
                    Sno = productIntake.Sno.Trim(),
                    TransDate = (DateTime)productIntake.TransDate,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productIntake.ProductType,
                    Qsupplied = (decimal)productIntake.Qsupplied,
                    Ppu = price.Price,
                    CR = productIntake.CR,
                    DR = productIntake.DR,
                    Balance = productIntake.Balance,
                    Description = productIntake.Description,
                    TransactionType = productIntake.TransactionType,
                    Remarks = productIntake.Remarks,
                    AuditId = auditId,
                    Auditdatetime = productIntake.Auditdatetime,
                    Branch = productIntake.Branch,
                    SaccoCode = productIntake.SaccoCode,
                    DrAccNo = productIntake.DrAccNo,
                    CrAccNo = productIntake.CrAccNo,
                    Zone=productIntake.Zone,
                };
                _context.ProductIntake.Add(collection);

                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == sno && t.Active
               && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
               && t.saccocode.ToUpper().Equals(sacco.ToUpper()));

                if (transport != null)
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR  = productIntake.Qsupplied * transport.Rate;
                   

                    if (ch < 0)
                    {
                        productIntake.CR = (decimal?)((productIntake.Qsupplied * transport.Rate)*-1);
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
                        AuditId = auditId,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = productIntake.DrAccNo,
                        CrAccNo = productIntake.CrAccNo,
                        Zone = productIntake.Zone,
                    };
                    _context.ProductIntake.Add(collection);

                    // Credit transpoter transport amount
                    productIntake.CR  = productIntake.Qsupplied * transport.Rate;
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
                        AuditId = auditId,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = price.TransportDrAccNo,
                        CrAccNo = price.TransportCrAccNo,
                        Zone = productIntake.Zone,
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

                if (productIntake.SMS)
                {
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var commulated = _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                    && s.TransDate >= startDate && s.TransDate <= endDate && s.Branch == saccoBranch).Sum(s => s.Qsupplied);
                    _context.Messages.Add(new Message
                    {
                        Telephone = supplier.PhoneNo,
                        Content = $"You have supplied {productIntake.Qsupplied} kgs to {sacco}. Your commulated {commulated + productIntake.Qsupplied}",
                        ProcessTime = DateTime.Now.ToString(),
                        MsgType = "Outbox",
                        Replied = false,
                        DateReceived = DateTime.Now,
                        Source = auditId,
                        Code = sacco
                    });
                }

                _context.SaveChanges();
                _notyf.Success("Correction saved successfully");
                SetIntakeInitialValues();

                var Todayskg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == (DateTime)productIntake.TransDate).Sum(p => p.Qsupplied);
                var TodaysBranchkg = _context.ProductIntake.Where(s => s.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && (s.Description == "Intake" || s.Description == "Correction") && s.TransDate == (DateTime)productIntake.TransDate && s.Branch == saccoBranch).Sum(p => p.Qsupplied);
                return View(new ProductIntakeVm
                {
                    Todaykgs = Todayskg,
                    TodayBranchkgs = TodaysBranchkg
                });
                //return RedirectToAction(nameof(CreateCorrection));
            }

            return View(productIntake);
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
}
