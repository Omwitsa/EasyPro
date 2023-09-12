using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.TranssupplyVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class AgReceiptsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        private readonly IReportProvider _reportProvider;
        public AgReceiptsController(MORINGAContext context, INotyfService notyf, IReportProvider reportProvider)
        {
            _context = context;
            _notyf = notyf;
            _reportProvider = reportProvider;
            utilities = new Utilities(context);
        }

        // GET: AgReceipts
        public async Task<IActionResult> Index()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var receipts = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate == DateTime.Today);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                receipts = receipts.Where(r => r.Branch == saccobranch);

            return View(await receipts.OrderByDescending(s => s.AuditDate).ToListAsync());
        }
        private async Task GetInitialValuesAsync()
        {
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var productintake = await _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.TransDate >= startDate && i.TransDate <= enDate).ToListAsync();
            var agproducts = await _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                agproducts = agproducts.Where(t => t.Branch == saccobranch).ToList();
                productintake = productintake.Where(i => i.Branch== saccobranch).ToList();
            }
                
            var productNames = agproducts.Select(b => b.PName);
            ViewBag.agproductsall = new SelectList(productNames, "");
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var branches = _context.DBranch.Where(i => i.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.branches = new SelectList(branches, "");

            ViewBag.productintake = productintake;
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

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);
            if (zones.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;
        }
        //PRODUCT SALES
        [HttpPost]
        public async Task<JsonResult> Save([FromBody] List<ProductIntake> intakes, string RNo, bool isStaff, bool isCash, bool sms, bool print, decimal net)
        {
            try
            {
                DateTime Now = DateTime.Today;
                DateTime startD = new DateTime(Now.Year, Now.Month, 1);
                DateTime enDate = startD.AddMonths(1).AddDays(-1);

                if (!intakes.Any())
                {
                    _notyf.Error("Sorry, Kindly provide records");
                    return Json("");
                }
                var cash = intakes.FirstOrDefault()?.Sno ?? "";
                if (!isCash && cash == "")
                {
                    _notyf.Error("Sorry, Kindly Farmers Number");
                    return Json("");
                }
                
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch);

                if(sacco == "USWET UMOJA DAIRIES FCS")
                {
                    var checkamount = intakes.Sum(b => b.DR);
                   if(net< checkamount)
                    {
                        _notyf.Error("Sorry, Farmers NetPay is less than Amount of Product.");
                        return Json("");
                    }
                }

                var products = _context.AgProducts.Where(p => p.saccocode == sacco);
                foreach(var intake in intakes)
                {
                    intake.Sno = intake?.Sno ?? "";
                    intake.Description = intake?.Description ?? "";
                    intake.SaccoCode = sacco;
                    intake.Branch = saccobranch;
                    intake.TransactionType = TransactionType.Deduction;
                    intake.TransTime = DateTime.Now.TimeOfDay;
                    intake.AuditId = loggedInUser;
                    intake.Auditdatetime = DateTime.Now;
                    intake.DrAccNo = intake.DrAccNo;
                    intake.CrAccNo = intake.CrAccNo;
                    if (string.IsNullOrEmpty(intake.Sno))
                        intake.Sno = "cash";
                    intake.Zone = intake.Zone;

                    var cashchecker = false;
                    if (string.IsNullOrEmpty(cash))
                        cashchecker = true;

                    var product = products.FirstOrDefault(p => p.PName.ToUpper().Equals(intake.Description.ToUpper()));
                    if (product != null)
                    {
                        var bal = product.OBal - (double?)intake.Qsupplied;
                        _context.AgReceipts.Add(new AgReceipt
                        {
                            RNo = RNo,
                            PCode = product.PCode,
                            TDate = intake.TransDate,
                            Amount = intake.DR,
                            SNo = intake.Sno,
                            Qua = (double?)intake.Qsupplied,
                            SBal = bal,
                            UserId = loggedInUser,
                            AuditDate = DateTime.Now,
                            Cash = cashchecker,
                            Sno1 = intake.Sno,
                            Transby = "",
                            Idno = "",
                            Mobile = "",
                            Remarks = intake.Description,
                            Branch = intake.Branch,
                            Sprice = product.Sprice,
                            Bprice = product.Pprice,
                            Ai = 0,
                            Run = 0,
                            Paid = 0,
                            Completed = 0,
                            Salesrep = "",
                            saccocode = sacco,
                            Zone = intake.Zone,
                        });

                        _context.Gltransactions.Add(new Gltransaction
                        {
                            AuditId = loggedInUser,
                            TransDate = intake.TransDate,
                            Amount = (decimal)intake.DR,
                            AuditTime = DateTime.Now,
                            DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                            Source = intake.Sno,
                            TransDescript = intake.Remarks,
                            Transactionno = $"{loggedInUser}{DateTime.Now}",
                            SaccoCode = sacco,
                            DrAccNo = product.Draccno,
                            CrAccNo = product.Craccno,
                            Branch = saccobranch
                        });
                        //NEED TO BE CREATED STAFF DEDUCTION TABLE FOR ALL DEDUCTIONS
                        if (isStaff == true)
                        {
                            _context.EmployeesDed.Add(new EmployeesDed
                            {
                                Empno = intake.Sno,
                                Date = intake.TransDate,
                                Deduction = "Store",
                                Amount = (decimal)intake.DR,
                                Remarks = intake.Remarks,
                                AuditId = loggedInUser,
                                saccocode = sacco
                            });
                        }

                        product.Qin = bal;
                        product.Qout = bal;
                        product.OBal = bal;
                    }

                    intake.Qsupplied = 0;
                    intake.Ppu = 0;
                }

                if (!isStaff && cash != "")
                    _context.ProductIntake.AddRange(intakes);

                if (sms)
                {
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == cash);
                    if (supplier != null)
                        _context.Messages.Add(new Message
                        {
                            Telephone = supplier.PhoneNo,
                            Content = $"Dear {supplier.Names}, You have bought goods worth {intakes.Sum(t => t.DR)} From our store",
                            ProcessTime = DateTime.Now.ToString(),
                            MsgType = "Outbox",
                            Replied = false,
                            DateReceived = DateTime.Now,
                            Source = loggedInUser,
                            Code = sacco
                        });
                }

                _context.SaveChanges();
                _notyf.Success("Saved successfully");

                var receiptDetails = await GetReceiptDetails(intakes);
                //PrintP(intakes, RNo);


                var receipts = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => u.RNo);
                var receipt1 = receipts.FirstOrDefault();
                double rno = Convert.ToInt32(receipt1.RNo);
                return Json(new
                {
                    rno = (rno + 1),
                    receiptDetails
                });
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        private async Task<dynamic> GetReceiptDetails(List<ProductIntake> intakes)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(sacco.ToUpper()));
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            // cummulative kgs calc

            var sno = intakes.FirstOrDefault()?.Sno ?? "";
            var suppliers = await _context.DSuppliers.Where(s => s.Sno.ToUpper().Equals(sno.ToUpper()) && s.Scode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

            var supplier = suppliers.FirstOrDefault();
            if (supplier == null)
                supplier = new DSupplier();
            //string pname = string.Format("{0:.###}", i.Remarks).PadLeft(8);
            //string BQnty = string.Format("{0:.###}", i.Qua).PadLeft(12);
            //string Amt = string.Format("{0:.###}", i.Amount).PadLeft(14);
            //string Body = pname + BQnty + BQnty + Amt;

            // var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(intake.Sno)
            //&& s.Scode == intake.SaccoCode && s.Branch == intake.Branch);

            var receipts = await _context.AgReceipts.Where(r => r.SNo.ToUpper().Equals(sno.ToUpper()) && r.saccocode == sacco
            && r.TDate >= startDate && r.TDate <= endDate).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                receipts = receipts.Where(s => s.Branch == saccoBranch).ToList();

            var cummAmount = receipts.Sum(r => r.Amount);
            return new
            {
                companies.Name,
                companies.Adress,
                companies.Town,
                companies.PhoneNo,
                saccoBranch,
                supplier.Sno,
                supName = supplier.Names,
                cummAmount,
                loggedInUser,
            };
        }

        //start
        private IActionResult PrintP(List<ProductIntake> intakes, string RNo)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, args) => printDocument_PrintPage(intakes, RNo, args);
            printDocument.Print();
            return Ok(200);
        }

        private void printDocument_PrintPage(object sender, string rNo, PrintPageEventArgs e)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(sacco.ToUpper()));

            ProductIntake items = sender as ProductIntake;
            var R = rNo;
            if (items != null)
            {
                // Start printing your items.
                var agReceiptsvalues = _context.AgReceipts.Where(k=>k.TDate== items.TransDate 
                && k.SNo.ToUpper().Equals(items.Sno.ToUpper()) && k.RNo==R).ToList();
                DateTime startDate = new DateTime(items.TransDate.Year, items.TransDate.Month, 1);
                DateTime enDate = startDate.AddMonths(1).AddDays(-1);

                var productIntakes = _context.ProductIntake.FirstOrDefault(u => u.Id == items.Id);

                var supplier = _context.DSuppliers.FirstOrDefault(u => u.Scode.ToUpper().Equals(sacco.ToUpper()) &&
                u.Sno.ToString() == items.Sno && u.Branch.ToUpper().Equals(saccoBranch.ToUpper()));



                Graphics graphics = e.Graphics;
                Font font = new Font("Times New Roman", 8);
                float fontHeight = font.GetHeight();

                int startX = 10;
                int startY = -40;
                int offset = 40;


                graphics.DrawString(companies.Name, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString(companies.Adress.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString(companies.Town.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Tell: " + companies.PhoneNo.PadLeft(10), font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Branch: " + saccoBranch, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string line = "---------------------------------------------";
                graphics.DrawString(line, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                var datet = items.TransDate.ToString("dd/MM/yyy");
                string sno = items.Sno.PadRight(10);
                if(sno.ToUpper()!="CASH" && sno.ToUpper() != "STAFF")
                {
                    graphics.DrawString("CheckOff Agrovet Receipt".PadRight(15), new Font("Times New Roman", 12), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                    graphics.DrawString("SNo: " + sno, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;

                    string name = supplier.Names.ToString();
                    graphics.DrawString("Name: " + name, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }

                if (sno.ToUpper() == "CASH" )
                {
                    graphics.DrawString("Agrovet Cash Sales Receipt".PadRight(15), new Font("Times New Roman", 12), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }

                if (sno.ToUpper() == "STAFF")
                {
                    graphics.DrawString("Agrovet Staff Sales Receipt".PadRight(15), new Font("Times New Roman", 12), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }
                graphics.DrawString("Date"+ items.TransDate, new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string HName = "Name".PadLeft(16);
                string HQnty = "Qnty".PadLeft(17);
                string HAmount = "Amount".PadLeft(18);
                string Heading = HName + HQnty + HAmount;
                graphics.DrawString(Heading, new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                agReceiptsvalues.ForEach(i => {
                    string pname = string.Format("{0:.###}", i.Remarks).PadLeft(8);
                    string BQnty = string.Format("{0:.###}", i.Qua).PadLeft(12);
                    string Amt = string.Format("{0:.###}", i.Amount).PadLeft(14);
                    string Body = pname + BQnty + BQnty + Amt;

                    graphics.DrawString(Body, new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                });

                string line1 = "---------------------------------------------";
                graphics.DrawString(line1, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Served By: " + loggedInUser, font, new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                graphics.DrawString("Date: " + DateTime.Now, font, new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 5;

                string line2 = "---------------------------------------------";
                graphics.DrawString(line2, font, new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 5;
                startY = startY + 20;
                string dev = "DEVELOP BY: AMTECH TECHNOLOGIES LIMITED";
                graphics.DrawString(dev.PadRight(13), new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 5;
                startY = startY + 20;
                string dev1 = " ";
                graphics.DrawString(dev1.PadRight(13), new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;
                graphics.DrawString(line1, font, new SolidBrush(Color.Black), startX, startY + offset);


            }
        }


        //PRODUCT REVERSAL
        [HttpPost]
        public JsonResult SaveReturns([FromBody] List<ProductIntake> intakes, string RNo, bool isStaff, bool isCash)
        {
            try
            {
                DateTime Now = DateTime.Today;
                DateTime startD = new DateTime(Now.Year, Now.Month, 1);
                DateTime enDate = startD.AddMonths(1).AddDays(-1);

                if (!intakes.Any())
                {
                    _notyf.Error("Sorry, Kindly provide records");
                    return Json("");
                }
                var cash = intakes.FirstOrDefault()?.Sno ?? "";
                DateTime tdate = intakes.FirstOrDefault().TransDate;
                if (!isCash && cash == "")
                {
                    _notyf.Error("Sorry, Kindly Farmers Number");
                    return Json("");
                }

                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                intakes.ForEach(t =>
                {
                    t.Description = t?.Description ?? "";
                    t.SaccoCode = sacco;
                    t.Branch = saccobranch;
                    t.TransactionType = TransactionType.Deduction;
                    t.TransTime = DateTime.Now.TimeOfDay;
                    t.AuditId = loggedInUser;
                    t.Qsupplied = t.Qsupplied * -1;
                    t.CR = t.DR;
                    t.DR = 0;
                    t.Auditdatetime = DateTime.Now;
                    t.DrAccNo = t.DrAccNo;
                    t.CrAccNo = t.CrAccNo;
                    if (string.IsNullOrEmpty(t.Sno))
                        t.Sno = "cash";
                    
                    var cashchecker = false;
                    if (cash == "")
                        cashchecker = true;

                    var product = _context.AgProducts.FirstOrDefault(p => p.PName.ToUpper().Equals(t.Description.ToUpper())
                    && p.saccocode == sacco && p.Branch == saccobranch);
                    if (product != null)
                    {
                        var bal = product.OBal + (double?)t.Qsupplied * -1;
                        _context.AgReceipts.Add(new AgReceipt
                        {
                            RNo = RNo,
                            PCode = product.PCode,
                            TDate = t.TransDate,
                            Amount = t.CR * -1,
                            SNo = t.Sno,
                            Qua = (double?)t.Qsupplied * -1,
                            SBal = bal,
                            UserId = loggedInUser,
                            AuditDate = DateTime.Now,
                            Cash = cashchecker,
                            Sno1 = t.Sno,
                            Transby = "",
                            Idno = "",
                            Mobile = "",
                            Remarks = t.Description,
                            Branch = t.Branch,
                            Sprice = product.Sprice * -1,
                            Bprice = product.Pprice * -1,
                            Ai = 0,
                            Run = 0,
                            Paid = 0,
                            Completed = 0,
                            Salesrep = "",
                            saccocode = sacco
                        });

                        _context.Gltransactions.Add(new Gltransaction
                        {
                            AuditId = loggedInUser,
                            TransDate = t.TransDate,
                            Amount = (decimal)t.CR,
                            AuditTime = DateTime.Now,
                            DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                            Source = t.Sno,
                            TransDescript = t.Remarks,
                            Transactionno = $"{loggedInUser}{DateTime.Now}",
                            SaccoCode = sacco,
                            CrAccNo = product.Draccno,
                            DrAccNo = product.Craccno,
                        });

                        if (isStaff == true)
                        {
                            _context.EmployeesDed.Add(new EmployeesDed
                            {
                                Empno = t.Sno,
                                Date = t.TransDate,
                                Deduction = "Store",
                                Amount = (decimal)t.CR * -1,
                                Remarks = t.Remarks,
                                AuditId = loggedInUser,
                                saccocode = sacco
                            });
                        }

                        product.Qin = bal;
                        product.Qout = bal;
                        product.OBal = bal;
                        t.Qsupplied = 0;
                        t.Ppu = 0;
                    }
                });

                if (!isStaff && cash != "")
                {
                    _context.ProductIntake.AddRange(intakes);
                }

                _context.SaveChanges();
                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        // GET: AgReceipts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                GetInitialValuesAsync();
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts
                .FirstOrDefaultAsync(m => m.RId == id);
            if (agReceipt == null)
            {
                return NotFound();
            }

            return View(agReceipt);
        }

        [HttpGet]
        public JsonResult SelectedDateIntake(string sno)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var todaysIntake = _context.DSuppliers.Where(L => L.Sno.ToUpper().Equals(sno.ToUpper()) && L.Scode.ToUpper().Equals(sacco.ToUpper()) && L.Branch == saccoBranch).Select(b => b.Names).ToList();
            //if (zone != "null")
            //    todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco && L.Zone == zone).Select(b => b.Names).ToList();

            return Json(todaysIntake);
        }

        // GET: AgReceipts/Create
        public async Task<IActionResult> CreateAsync()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await GetInitialValuesAsync();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var receipts = await _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => u.RNo).ToListAsync();
            var receiptNo = receipts.FirstOrDefault()?.RNo ?? "0";
            double num = Convert.ToInt32(receiptNo);
            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var transporters = await _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco).ToListAsync();
            var products = await _context.AgProducts.Where(p => p.saccocode == sacco)
                .OrderBy(p => p.PName).ToListAsync();
            var intakes = await _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && u.TransDate >= startDate && u.TransDate <= endDate).ToListAsync();
            var staff = await _context.Employees.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();
                suppliers = suppliers.Where(s => s.Branch == saccobranch).ToList();
                products = products.Where(p => p.Branch == saccobranch).ToList();
                intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                DTransporter = transporters,
                DSuppliers = suppliers,
                AgProductobj = products,
                ProductIntake = intakes,
                Employees = staff
            };
            return View(agrovetsales);
        }

        // POST: AgReceipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await GetInitialValuesAsync();
            if (ModelState.IsValid)
            {
                _context.Add(agReceipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agReceipt);
        }

        public async Task<IActionResult> CreateReversal()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);

            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };

            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var transporters = await _context.DTransporters.Where(s => s.ParentT == sacco).ToListAsync();
            var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco).ToListAsync();
            var products = await _context.AgProducts.Where(p => p.saccocode == sacco).ToListAsync();
            var intakes = await _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && u.TransDate >= startDate && u.TransDate <= endDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();
                suppliers = suppliers.Where(s => s.Branch == saccobranch).ToList();
                products = products.Where(p => p.Branch == saccobranch).ToList();
                intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }

            var productNames = products.Select(b => b.PName);
            ViewBag.agproductsall = new SelectList(productNames, "");
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                DTransporter = transporters,
                DSuppliers = suppliers,
                AgProductobj = products,
                ProductIntake = intakes
            };
            return View(agrovetsales);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReversal([Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await GetInitialValuesAsync();
            if (ModelState.IsValid)
            {
                _context.Add(agReceipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agReceipt);
        }

        // GET: AgReceipts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValuesAsync();

            if (id == null)
            {
                return NotFound();
            }

            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };

            var agReceipt = await _context.AgReceipts.FindAsync(id);
            if (agReceipt == null)
            {
                return NotFound();
            }
            return View(agReceipt);
        }

        // POST: AgReceipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != agReceipt.RId)
            {
                GetInitialValuesAsync();
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agReceipt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgReceiptExists(agReceipt.RId))
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
            return View(agReceipt);
        }

        // GET: AgReceipts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValuesAsync();
            if (id == null)
            {
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts
                .FirstOrDefaultAsync(m => m.RId == id);
            if (agReceipt == null)
            {
                return NotFound();
            }

            return View(agReceipt);
        }

        // POST: AgReceipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValuesAsync();
            var agReceipt = await _context.AgReceipts.FindAsync(id);
            _context.AgReceipts.Remove(agReceipt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgReceiptExists(long id)
        {
            GetInitialValuesAsync();
            return _context.AgReceipts.Any(e => e.RId == id);
        }

        [HttpGet]
        public JsonResult GetProductBal(string pname)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var product = _context.AgProducts.FirstOrDefault(p => p.PName == pname && p.saccocode == sacco && p.Branch == saccobranch);
            if (product == null)
                product = new AgProduct { OBal = 0 };
            return Json(product);
        }
    }
}
