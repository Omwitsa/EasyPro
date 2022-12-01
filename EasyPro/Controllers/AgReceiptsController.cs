using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using EasyPro.ViewModels.TranssupplyVM;
using EasyPro.ViewModels;
using EasyPro.IProvider;

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
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var receipts = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate == DateTime.Today);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                receipts = receipts.Where(r => r.Branch == saccobranch);

            return View(await receipts.OrderByDescending(s => s.AuditDate).ToListAsync());
        }
        private void GetInitialValues()
        {
            DateTime endmonth = DateTime.Now;
            DateTime startDate = new DateTime(endmonth.Year, endmonth.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var agproducts = _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).Select(b => b.PName).ToList();
            ViewBag.agproductsall = new SelectList(agproducts, "");
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var branches = _context.DBranch.Where(i => i.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.branches = new SelectList(branches, "");
            var productintake = _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && 
            i.Branch == saccobranch && i.TransDate>=startDate && i.TransDate<= enDate).ToList();
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
        public JsonResult Save([FromBody] List<ProductIntake> intakes, string RNo, bool isStaff, bool isCash, bool sms, bool print)
        {
            try
            {
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
                intakes.ForEach(t =>
                {
                    t.Description = t?.Description ?? "";
                    t.SaccoCode = sacco;
                    t.Branch = saccobranch;
                    t.TransactionType = TransactionType.Deduction;
                    t.TransTime = DateTime.Now.TimeOfDay;
                    t.AuditId = loggedInUser;
                    t.Auditdatetime = DateTime.Now;
                    t.DrAccNo = t.DrAccNo;
                    t.CrAccNo = t.CrAccNo;
                    if (t.Sno == "")
                    {
                        t.Sno = "cash";
                    }
                    t.Zone = t.Zone;

                    var cashchecker = false;
                    if (cash == "")
                        cashchecker = true;

                    var product = _context.AgProducts.FirstOrDefault(p => p.PName.ToUpper().Equals(t.Description.ToUpper())
                    && p.saccocode == sacco && p.Branch == saccobranch);
                    if (product != null)
                    {
                        var bal = product.OBal - (double?)t.Qsupplied;
                        _context.AgReceipts.Add(new AgReceipt
                        {
                            RNo = RNo,
                            PCode = product.PCode,
                            TDate = t.TransDate,
                            Amount = t.DR,
                            SNo = t.Sno,
                            Qua = (double?)t.Qsupplied,
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
                            Sprice = product.Sprice,
                            Bprice = product.Pprice,
                            Ai = 0,
                            Run = 0,
                            Paid = 0,
                            Completed = 0,
                            Salesrep = "",
                            saccocode = sacco,
                            Zone = t.Zone,
                        });

                        _context.Gltransactions.Add(new Gltransaction
                        {
                            AuditId = loggedInUser,
                            TransDate = t.TransDate,
                            Amount = (decimal)t.DR,
                            AuditTime = DateTime.Now,
                            DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                            Source = t.Sno,
                            TransDescript = t.Remarks,
                            Transactionno = $"{loggedInUser}{DateTime.Now}",
                            SaccoCode = sacco,
                            DrAccNo = product.Draccno,
                            CrAccNo = product.Craccno,
                        });
                        //NEED TO BE CREATED STAFF DEDUCTION TABLE FOR ALL DEDUCTIONS
                        if (isStaff == true)
                        {
                            _context.EmployeesDed.Add(new EmployeesDed
                            {
                                Empno = t.Sno,
                                Date = t.TransDate,
                                Deduction = "Store",
                                Amount = (decimal)t.DR,
                                Remarks = t.Remarks,
                                AuditId = loggedInUser,
                                saccocode = sacco
                            });
                        }

                        product.Qin = bal;
                        product.Qout = bal;
                        product.OBal = bal;
                    }

                });

                //if (isStaff == false)
                //{
                //    if (cash != "")
                //    {
                //        _context.ProductIntake.AddRange(intakes);
                //    }
                //}

                if (!isStaff && cash != "")
                    _context.ProductIntake.AddRange(intakes);

                if (sms)
                {
                    var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno.ToString() == cash);
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
                if (print)
                    return Json(new
                    {
                        redirectUrl = Url.Action("GetAgSalesReceipt", "PdfReport", new { rno = RNo }),
                        isRedirect = true
                    });

                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        //PRODUCT REVERSAL
        [HttpPost]
        public JsonResult SaveReturns([FromBody] List<ProductIntake> intakes, string RNo, bool isStaff, bool isCash)
        {
            try
            {

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
                    if (t.Sno == "")
                    {
                        t.Sno = "cash";
                    }
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
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                GetInitialValues();
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
            var todaysIntake = _context.DSuppliers.Where(L => L.Sno.ToUpper().Equals(sno.ToUpper()) && L.Scode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Names).ToList();
            //if (zone != "null")
            //    todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco && L.Zone == zone).Select(b => b.Names).ToList();

            return Json(todaysIntake);
        }

        // GET: AgReceipts/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();

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
            var period = DateTime.Today;
            var startDate = new DateTime(period.Year, period.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var transporters = _context.DTransporters.Where(s => s.ParentT == sacco).ToList();
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco).ToList();
            var products = _context.AgProducts.Where(p => p.saccocode == sacco).ToList();
            var intakes = _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && u.TransDate >= startDate && u.TransDate <= endDate).ToList();
            var staff = _context.Employees.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
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
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (ModelState.IsValid)
            {
                _context.Add(agReceipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agReceipt);
        }

        public IActionResult CreateReversal()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();

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

            var period = DateTime.Today;
            var startDate = new DateTime(period.Year, period.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var transporters = _context.DTransporters.Where(s => s.ParentT == sacco).ToList();
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco).ToList();
            var products = _context.AgProducts.Where(p => p.saccocode == sacco).ToList();
            var intakes = _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && u.TransDate >= startDate && u.TransDate <= endDate).ToList();
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
                ProductIntake = intakes
            };
            return View(agrovetsales);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReversal([Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
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
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();

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
            utilities.SetUpPrivileges(this);
            if (id != agReceipt.RId)
            {
                GetInitialValues();
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
            utilities.SetUpPrivileges(this);
            GetInitialValues();
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
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            var agReceipt = await _context.AgReceipts.FindAsync(id);
            _context.AgReceipts.Remove(agReceipt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgReceiptExists(long id)
        {
            GetInitialValues();
            return _context.AgReceipts.Any(e => e.RId == id);
        }
    }
}
