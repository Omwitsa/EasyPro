using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Math;
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
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate == DateTime.Today 
                && i.Branch == saccobranch);

            return View(await receipts.OrderByDescending(s => s.AuditDate).ToListAsync());
        }
        public async Task<IActionResult> PartialIndex()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var receipts = _context.ProductIntake
                .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.TransDate == DateTime.Today && i.Remarks.Contains("Partial Payment"));
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                receipts = receipts.Where(r => r.Branch == saccobranch);

            return View(await receipts.OrderByDescending(s => s.TransDate).ToListAsync());
        }
        private void GetInitialValuesAsync()
        {
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            IQueryable<AgProduct> agProducts = _context.AgProducts;
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;

            var agproducts = agProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.Branch == saccobranch).ToList();
           
            var productNames = agproducts.Select(b => b.PName);
            ViewBag.agproductsall = new SelectList(productNames, "");
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var branches = _context.DBranch.Where(i => i.Bcode.ToUpper().Equals(sacco.ToUpper()))
                .Select(b => b.Bname).ToList();
            ViewBag.branches = new SelectList(branches, "");

            //ViewBag.productintake = productintake;
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
                if (sacco == "USWET UMOJA DAIRIES FCS")
                {
                    var checkamount = intakes.Sum(b => b.DR);
                    if (net < checkamount)
                    {
                        _notyf.Error("Sorry, Farmers NetPay is less than Amount of Product.");
                        return Json("");
                    }
                }

                var products = await _context.AgProducts.Where(p => p.saccocode == sacco && p.Branch == saccobranch).ToListAsync();
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                foreach (var intake in intakes)
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

                    var product = products.FirstOrDefault(p => p.PName.ToUpper().Equals(intake.Description.ToUpper()) );
                    

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

                            _context.EmpDeductions.Add(new EmpDeduction
                            {//EmpNo,Date, DeductionType, Amount, Auditdate, AuditId, SaccoCode,IsstandingOrder
                                EmpNo = intake.Sno,
                                Date = intake.TransDate,
                                DeductionType = "Store",
                                Amount = (decimal)intake.DR,
                                Auditdate = DateTime.Now,
                                AuditId = loggedInUser,
                                SaccoCode = sacco,
                                IsstandingOrder = false,
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
                var checkmessageConfigs = _context.MessageConfigs.FirstOrDefault(n => n.saccocode == sacco && !n.Closed);
                if (checkmessageConfigs != null)
                {
                    if (sms)
                    {
                        var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);
                        var supplierlist = _context.DSuppliers.Where(s => s.Scode == sacco).ToList();

                        var supplier = supplierlist.FirstOrDefault(s => s.Sno == cash);
                        supplier.PhoneNo = supplier.PhoneNo ?? "0";
                        if (supplier != null && supplier.PhoneNo != "0")
                            if (supplier.PhoneNo.Length > 8)
                            {
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
                    }
                }
               

                _context.SaveChanges();
                _notyf.Success("Saved successfully");

                var receiptDetails = await GetReceiptDetails(intakes);
                //PrintP(intakes, RNo);
                var receiptDetails1 = receiptDetails;
                var receipts = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).OrderByDescending(u => u.RId).ToList(); 

                if (StrValues.Slopes == sacco)
                    receipts = receipts.OrderByDescending(m => m.RId).ToList();
                GetInitialValuesAsync();
                var receipt1 = receipts.FirstOrDefault();
                double rno = Convert.ToInt32(receipt1.RNo);
                return Json(new
                {
                    rno = (rno + 1),
                    receiptDetails,
                    receiptDetails1
                });
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
        [HttpPost]
        public async Task<JsonResult> Savepartial([FromBody] ProductIntakeVm productIntake,string drac, string crac)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            if (string.IsNullOrEmpty(productIntake.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.CR == 0)
            {
                _notyf.Error("Sorry, Kindly provide amount to Pay.");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.CR > productIntake.DR)
            {
                _notyf.Error("Sorry, Kindly provide amount less than what you have in store.");
                return Json(new
                {
                    success = false
                });
            }
            IQueryable<DSupplier> dSuppliers = _context.DSuppliers;
            var suppliers = dSuppliers.Where(s => s.Sno.ToUpper().Equals(productIntake.Sno.ToUpper())
            && s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();

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

            productIntake.Branch = saccoBranch;
            productIntake.Sno = productIntake?.Sno ?? "";
            productIntake.Qsupplied = productIntake?.Qsupplied ?? 0;
            productIntake.CR = productIntake?.CR ?? 0;
            productIntake.Auditdatetime = DateTime.Now;
            productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
            productIntake.SaccoCode = sacco;
            productIntake.Zone = supplier?.Zone ?? "";
            productIntake.TransactionType = TransactionType.Deduction;
            productIntake.DrAccNo = drac;
            productIntake.CrAccNo = crac;

            var collection = new ProductIntake
            {
                Sno = productIntake.Sno.Trim().ToUpper(),
                TransDate = productIntake?.TransDate ?? DateTime.Today,
                TransTime = productIntake.TransTime,
                ProductType = "AGROVET",
                Qsupplied = 0,
                Ppu = 0,
                CR = productIntake.CR,
                DR = 0,
                Balance = 0,
                Description = "Store Cash Partial Payment",
                TransactionType = productIntake.TransactionType,
                Remarks = "Store Cash Partial Payment",
                AuditId = loggedInUser,
                Auditdatetime = productIntake.Auditdatetime,
                Branch = saccoBranch,
                SaccoCode = productIntake.SaccoCode,
                DrAccNo = productIntake.CrAccNo,
                CrAccNo = productIntake.DrAccNo,
                Zone = productIntake.Zone,
                MornEvening = productIntake.MornEvening
            };
            _context.ProductIntake.Add(collection);
            
            _context.AgReceipts.Add(new AgReceipt
            {
                RNo = "0",
                PCode = "0",
                TDate = productIntake.TransDate,
                Amount = -1*productIntake.CR,
                SNo = productIntake.Sno,
                Qua = 0,
                SBal = 0,
                UserId = loggedInUser,
                AuditDate = DateTime.Now,
                Cash = true,
                Sno1 = productIntake.Sno,
                Transby = "",
                Idno = "",
                Mobile = "",
                Remarks = "Store Cash Partial Payment",
                Branch = saccoBranch,
                Sprice = 0,
                Bprice = 0,
                Ai = 0,
                Run = 0,
                Paid = 0,
                Completed = 0,
                Salesrep = "",
                saccocode = sacco,
                Zone = "0",
            });
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = (DateTime)productIntake.TransDate,
                Amount = (decimal)productIntake.CR,
                AuditTime = DateTime.Now,
                DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                Source = productIntake.Sno,
                TransDescript = "Store Cash Partial Payment",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = productIntake.CrAccNo,
                CrAccNo = productIntake.DrAccNo,
                Branch = saccoBranch
            });
            _context.SaveChanges();
            _notyf.Success("Partial Payment Made successfully");

            var intake = new ProductIntake
            {
                Sno = productIntake.Sno,
                Qsupplied = (decimal)productIntake.CR,
                MornEvening = productIntake.MornEvening,
                SaccoCode = productIntake.SaccoCode,
                Branch = saccoBranch,

            };
            string cummkgs = string.Format("{0:.###}", productIntake.DR);
            decimal MornEvening = (decimal)(productIntake.DR - productIntake.CR);
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(intake.SaccoCode.ToUpper()));
            var receiptDetails = new
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
                MornEvening,
            };    //= await GetReceiptDetails(intake, loggedInUser);

            return Json(new
            {
                receiptDetails,
                success = true
            });
        }
        //PRODUCT SALES
        private async Task<dynamic> GetReceiptDetails(ProductIntake intake, string loggedInUser)
        {
            intake.Sno = intake?.Sno ?? "";
            var companies = _context.DCompanies.FirstOrDefault(i => i.Name.ToUpper().Equals(intake.SaccoCode.ToUpper()));
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            // cummulative kgs calc
            var intakes = await _context.ProductIntake.Where(o => o.SaccoCode.ToUpper().Equals(intake.SaccoCode.ToUpper()) &&
            o.Sno == intake.Sno &&o.TransDate >= startDate && o.TransDate <= endDate
            && (o.Description == "Intake" || o.Description == "Correction")).ToListAsync();

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(n=>n.Branch == intake.Branch).ToList();

            var cumkg = intakes.Sum(d => d.Qsupplied);
            string cummkgs = string.Format("{0:.###}", cumkg);
            var suppliers = await _context.DSuppliers.Where(s => s.Sno.ToUpper().Equals(intake.Sno)
            && s.Scode == intake.SaccoCode).ToListAsync();

            //if (user.AccessLevel == AccessLevel.Branch)
            //    suppliers = suppliers.Where(s => s.Branch == intake.Branch).ToList();

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
                intake.CR,
                cummkgs,
                loggedInUser,
                MornEvening = intake.MornEvening ?? "Mornnig",
            };
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
            var suppliers = await _context.DSuppliers.Where(s => s.Sno.ToUpper().Equals(sno.ToUpper()) 
            && s.Scode == sacco).ToListAsync();
            //var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            //if (user.AccessLevel == AccessLevel.Branch)
            //    suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

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
            && r.TDate >= startDate && r.TDate <= endDate && r.Branch == saccoBranch).ToListAsync();

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
                var agReceiptsvalues = _context.AgReceipts.Where(k => k.TDate == items.TransDate
                && k.SNo.ToUpper().Equals(items.Sno.ToUpper()) && k.RNo == R).ToList();
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
                if (sno.ToUpper() != "CASH" && sno.ToUpper() != "STAFF")
                {
                    graphics.DrawString("CheckOff Agrovet Receipt".PadRight(15), new Font("Times New Roman", 12), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                    graphics.DrawString("SNo: " + sno, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;

                    string name = supplier.Names.ToString();
                    graphics.DrawString("Name: " + name, font, new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }

                if (sno.ToUpper() == "CASH")
                {
                    graphics.DrawString("Agrovet Cash Sales Receipt".PadRight(15), new Font("Times New Roman", 12), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }

                if (sno.ToUpper() == "STAFF")
                {
                    graphics.DrawString("Agrovet Staff Sales Receipt".PadRight(15), new Font("Times New Roman", 12), new SolidBrush(Color.Black), startX, startY + offset);
                    offset = offset + (int)fontHeight + 5;
                }
                graphics.DrawString("Date" + items.TransDate, new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                string HName = "Name".PadLeft(16);
                string HQnty = "Qnty".PadLeft(17);
                string HAmount = "Amount".PadLeft(18);
                string Heading = HName + HQnty + HAmount;
                graphics.DrawString(Heading, new Font("Times New Roman", 8), new SolidBrush(Color.Black), startX, startY + offset);
                offset = offset + (int)fontHeight + 5;

                agReceiptsvalues.ForEach(i =>
                {
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
                GetInitialValuesAsync();
                if (!intakes.Any())
                {
                    _notyf.Error("Sorry, Kindly provide records");
                    return Json("");
                }
                var cash = intakes.FirstOrDefault()?.Sno ?? "";
                DateTime tdate = intakes.FirstOrDefault().TransDate;
                var checkifthereceiptexist = _context.AgReceipts.Where(n=>n.TDate == tdate && n.RNo == RNo).ToList();
                if (!checkifthereceiptexist.Any())
                {
                    _notyf.Error("Sorry, No Invoice No." + RNo + "On"+ tdate + ", Kindly Try Again.");
                    return Json("");
                }

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

                    var productlist = _context.AgProducts.Where(p => p.PName.ToUpper().Equals(t.Description.ToUpper())
                    && p.saccocode == sacco && p.Branch == saccobranch).ToList();

                    var product = productlist.FirstOrDefault();

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
                            _context.EmpDeductions.Add(new EmpDeduction
                            {//EmpNo,Date, DeductionType, Amount, Auditdate, AuditId, SaccoCode,IsstandingOrder
                                EmpNo = t.Sno,
                                Date = t.TransDate,
                                DeductionType = "Store",
                                Amount = (decimal)t.DR,
                                Auditdate = DateTime.Now,
                                AuditId = loggedInUser,
                                SaccoCode = sacco,
                                IsstandingOrder = false,
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
        public JsonResult SelectedDateIntake(string sno, DateTime date)
        {
            sno = sno ?? "";
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var allnames = _context.DSuppliers.Where(L => L.Sno.ToUpper().Equals(sno.ToUpper())
            && L.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            //if (user.AccessLevel == AccessLevel.Branch)
            //    allnames = allnames.Where(L => L.Branch == saccoBranch).ToList();

            var todaysIntake = allnames.Select(b => b.Names).ToList();
            //if (zone != "null")
            //    todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco && L.Zone == zone).Select(b => b.Names).ToList();
            var productintake = _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.TransDate >= startDate && i.Sno.ToLower().Equals(sno.ToUpper()) && i.TransDate <= enDate).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                productintake = productintake.Where(i => i.Branch == saccoBranch).ToList();

            //ViewBag.productintake = new SelectList(productintake, "");
            var getdetail = productintake.Where(i => i.TransactionType == TransactionType.Intake
           || i.TransactionType == TransactionType.Correction).ToList();

            var deduction = productintake.Where(K => K.TransactionType == TransactionType.Deduction).ToList();

            var gross = getdetail.Sum(n=>n.CR)- getdetail.Sum(n=>n.DR);
            var deductions = deduction.Sum(n => n.DR) - deduction.Sum(n => n.CR); 
            return Json(new { todaysIntake, gross, deductions });
        }
        
        [HttpGet]
        public JsonResult selectedDateTransporterIntake(string sno, DateTime date)
        {
            sno = sno ?? "";
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var allnames = _context.DSuppliers.Where(L => L.Sno.ToUpper().Equals(sno.ToUpper())
             && L.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();

            var transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())
            && s.TransCode.ToUpper().Equals(sno.ToUpper())).ToList();
           
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            //if (user.AccessLevel == AccessLevel.Branch)
            //    transporters = transporters.Where(L => L.Tbranch == saccoBranch).ToList();

            var todaysIntake = transporters.Select(b => b.TransName).ToList();
            //if (zone != "null")
            //    todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco && L.Zone == zone).Select(b => b.Names).ToList();
            var productintake = _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.TransDate >= startDate && i.Sno.ToLower().Equals(sno.ToUpper()) && i.TransDate <= enDate).ToList();
            if (user.AccessLevel == AccessLevel.Branch)
                productintake = productintake.Where(i => i.Branch == saccoBranch).ToList();

            //ViewBag.productintake = new SelectList(productintake, "");

            var gross = productintake.Sum(n => n.CR);
            var deductions = productintake.Sum(n => n.DR);
            return Json(new { todaysIntake, gross, deductions });
        }
        
        [HttpGet] 
        public JsonResult SelectedName2(string sno, DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            if (string.IsNullOrEmpty(sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return Json(new
                {
                    success = false
                });
            }

            IQueryable<DSupplier> dSuppliers = _context.DSuppliers;
            IQueryable<AgReceipt> agReceipts = _context.AgReceipts;
            var getthename = dSuppliers.FirstOrDefault(L => L.Sno.ToUpper().Equals(sno.ToUpper()) && L.Scode == sacco );
            var getsales = agReceipts.Where(n => n.saccocode == sacco && n.Branch == saccoBranch && n.SNo.ToUpper().Equals(sno.ToUpper())
            && n.TDate >= startDate && n.TDate <= endDate);
            
            var getgl = getsales.FirstOrDefault();
            var getgls = _context.Gltransactions.FirstOrDefault(b => b.SaccoCode == sacco && b.TransDate >= startDate && b.TransDate <= endDate
            && b.TransDescript == getgl.Remarks);
            var storeamount = getsales.Sum(s => s.Amount);
            return Json(new { getthename, storeamount, getgls });
        }

        public async Task<IActionResult> CreatePartialPay()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            //await GetInitialValuesAsync();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            //var receipts = await _context.AgReceipts
            //    .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
            //    .OrderByDescending(u => u.RNo).ToListAsync();

            //if (StrValues.Slopes == sacco)
            //    receipts = receipts.OrderByDescending(m => m.RId).ToList();

            //var receiptNo = receipts.FirstOrDefault()?.RNo ?? "0";
            //double num = Convert.ToInt32(receiptNo);
            //var receipt = new AgReceipt
            //{
            //    RNo = "" + (num + 1),
            //    Qua = 0,
            //    Amount = 0,
            //    Sprice = 0,
            //    Bprice = 0,
            //    SBal = 0,
            //    TDate = DateTime.Today,
            //};

            return View();
        }

        // GET: AgReceipts/Create
        public async Task<IActionResult> Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);

            IQueryable<AgReceipt> agReceipts = _context.AgReceipts;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var receipts = await agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.Branch == saccobranch).OrderByDescending(u => u.RId).ToListAsync();

            if (StrValues.Slopes == sacco)
                receipts = receipts.OrderByDescending(m => m.RId).ToList();

            var receiptNo = receipts.FirstOrDefault()?.RNo ?? "0";

            double num = GetRNo(receiptNo); 
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
            IQueryable<AgProduct> agProducts = _context.AgProducts;
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            IQueryable<DSupplier> dSuppliers = _context.DSuppliers;
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            //var transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            //var suppliers = dSuppliers.Where(s => s.Scode == sacco ).ToList();
            var products = agProducts.Where(p => p.saccocode == sacco && p.Branch == saccobranch)
                .OrderBy(p => p.PName).ToList();
            //var intakes = productIntakes.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            //&& u.TransDate >= startDate && u.TransDate <= endDate).ToList();
            var staff = _context.Employees.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                //transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();
                //suppliers = suppliers.Where(s => s.Branch == saccobranch).ToList();
                //products = products.Where(p => p.Branch == saccobranch).ToList();
                //intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }
            GetInitialValuesAsync();
            ViewBag.slopes = StrValues.Slopes == sacco;
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                //DTransporter = transporters,
                //DSuppliers = suppliers,
                AgProductobj = products,
                //ProductIntake = intakes,
                Employees = staff
            };
            return View(agrovetsales);
        }

        private double GetRNo(string receiptNo)
        {
            int receiptNo1 =0;
            try
            {
               receiptNo1 = Convert.ToInt32(receiptNo);
            }
            catch
            {
                receiptNo1 = 0;
            }
           return receiptNo1;
        }

        [HttpPost]
        public JsonResult SuppliedProducts(DateTime date1, DateTime date2,string condition, string product)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var startingdate = date2;
            var startDate = new DateTime(date1.Year, date1.Month, 1);
            var endDate = startDate.AddDays(-1);
            IQueryable<AgReceipt> agReceipts = _context.AgReceipts;

            var products = new List<AgReceiptVM>();

            var agProductsReceive = agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.TDate >= date1 && i.TDate <= date2 && i.Branch == saccobranch).ToList().OrderByDescending(b => b.RId).ToList();

            if (!string.IsNullOrEmpty(product))
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    if (condition == "InvoiceNo")
                    {
                        agProductsReceive = agProductsReceive.Where(i => i.RNo.ToUpper().Contains(product.ToUpper())).ToList();
                    }
                    if (condition == "SNo")
                    {
                        agProductsReceive = agProductsReceive.Where(i => i.SNo.ToUpper().Contains(product.ToUpper())).ToList();
                    }
                    if (condition == "Product Name")
                    {
                        agProductsReceive = agProductsReceive.Where(i => i.Remarks.ToUpper().Contains(product.ToUpper())).ToList();
                    }
                }
            }

            agProductsReceive.ForEach(j =>
            {
                decimal quantity = (decimal)j.Qua;
                if (j.Amount < 0)
                    quantity = quantity * -1;

                products.Add(new AgReceiptVM
                {
                    TDate = j.TDate,
                    PCode = j.PCode,
                    Remarks = j.Remarks,
                    Qua = quantity,
                    Amount = j.Amount,
                    SNo = j.SNo,
                    InvoiceNo = j.RNo
                });
            });
            return Json(products);
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
            GetInitialValuesAsync();
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
            var count = await _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderByDescending(u => u.RId)
                .Select(b => b.RNo).ToListAsync();

            var selectedno = count.FirstOrDefault();
            double num = GetRNo(selectedno);

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
            GetInitialValuesAsync();
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            //var transporters = await _context.DTransporters.Where(s => s.ParentT == sacco).ToListAsync();
            //var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco).ToListAsync();
            var products = await _context.AgProducts.Where(p => p.saccocode == sacco && p.Branch == saccobranch).ToListAsync();
            //var intakes = await _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            //&& u.TransDate >= startDate && u.TransDate <= endDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                //transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();
                //suppliers = suppliers.Where(s => s.Branch == saccobranch).ToList();
                //products = products.Where(p => p.Branch == saccobranch).ToList();
                //intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }

            //var productNames = products.Select(b => b.PName);
            //ViewBag.agproductsall = new SelectList(productNames, "");
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                //DTransporter = transporters,
                //DSuppliers = suppliers,
                AgProductobj = products,
                //ProductIntake = intakes
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
            GetInitialValuesAsync();
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

        [HttpPost]
        public JsonResult getsalesforthis(string InvoiceNo, DateTime TDate)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var salesstatement = _context.AgReceipts.Where(p => p.RNo == InvoiceNo && p.TDate == TDate
            && p.saccocode == sacco && p.Branch == saccobranch).ToList();
            
            return Json(salesstatement);
        }
    }
}
