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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class ProductIntakesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

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
            return View(await _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Intake && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) 
                && i.TransDate == DateTime.Today)
                .ToListAsync());
        }
        public async Task<IActionResult> TDeductionList()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction 
            && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today).ToListAsync();
            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                var supplier = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.Sno && i.ParentT == sacco);
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
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction 
            && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today).ToListAsync();
            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                long.TryParse(intake.Sno, out long sno);
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == sno && i.Scode == sacco);
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.ProductIntake
                .Where(c => c.TransactionType == TransactionType.Correction && c.SaccoCode.ToUpper().Equals(sacco.ToUpper()) 
                && c.TransDate == DateTime.Today)
                .ToListAsync());
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
            return View();
        }

        private void SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var suppliers = _context.DSuppliers
                .Where(s=>s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.suppliers = suppliers;
            var products = _context.DPrices.Where(s=>s.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.products = new SelectList(products, "Products", "Products");
            ViewBag.productPrices = products;
        }

        public IActionResult CreateDeduction()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            Farmersobj = new FarmersVM()
            {
                DSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco),
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
            Farmersobj = new FarmersVM()
            {
                DTransporters = _context.DTransporters.Where(t => t.ParentT == sacco),
                ProductIntake = new ProductIntake
                {
                    TransDate = DateTime.Today
                }
            };
            return View(Farmersobj);
        }
        private void GetInitialValues()
        {
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
        }
        public IActionResult CreateCorrection()
        {
            utilities.SetUpPrivileges(this);
            SetIntakeInitialValues();
            return View();
        }

        // POST: ProductIntakes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch,DrAccNo,CrAccNo")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            productIntake.Description = productIntake?.Description ?? "";
            long.TryParse(productIntake.Sno, out long sno);
            if (sno < 1)
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return View(productIntake);
            }
            if (!_context.DSuppliers.Any(s => s.Sno == sno && s.Scode.ToUpper().Equals(sacco.ToUpper())))
            {
                _notyf.Error("Sorry, Supplier No. not found");
                return View(productIntake);
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return View(productIntake);
            }
            if (productIntake.Qsupplied < 1)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(productIntake);
            }
            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno && s.Scode.ToUpper().Equals(sacco.ToUpper()));
            if (supplier == null)
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return View(productIntake);
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return View(productIntake);
            }
            if (ModelState.IsValid)
            {
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var branch = _context.DBranch.FirstOrDefault(b => b.Bcode.ToUpper().Equals(sacco.ToUpper()));
                productIntake.Branch = branch.Bname;
                productIntake.SaccoCode = sacco;
                productIntake.TransactionType = TransactionType.Intake;
                productIntake.TransDate = DateTime.Today;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Balance = utilities.GetBalance(productIntake);
                var price = _context.DPrices
                    .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                    && p.Products.ToUpper().Equals(productIntake.ProductType.ToUpper()));
                _context.ProductIntake.Add(new ProductIntake { 
                    Sno = productIntake.Sno.Trim(),
                    TransDate = productIntake.TransDate,
                    TransTime = productIntake.TransTime,
                    ProductType = productIntake.ProductType,
                    Qsupplied = productIntake.Qsupplied,
                    Ppu = price.Price,
                    CR = productIntake.CR,
                    DR = productIntake.DR,
                    Balance = productIntake.Balance,
                    Description = "Intake",
                    TransactionType = productIntake.TransactionType,
                    Paid = productIntake.Paid,
                    Remarks = productIntake.Remarks,
                    AuditId = auditId,
                    Auditdatetime = productIntake.Auditdatetime,
                    Branch = productIntake.Branch,
                    SaccoCode = productIntake.SaccoCode,
                    DrAccNo = productIntake.DrAccNo,
                    CrAccNo = productIntake.CrAccNo,
                });

                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == sno && t.Active
                && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
                && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()));
                var collection = new ProductIntake();
                if (transport != null)
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR = productIntake.Qsupplied * transport.Rate;
                    productIntake.Balance = productIntake.Balance - productIntake.DR;
                    collection = new ProductIntake
                    {
                        Sno = productIntake.Sno.Trim(),
                        TransDate = productIntake.TransDate,
                        TransTime = productIntake.TransTime,
                        ProductType = productIntake.ProductType,
                        Qsupplied = productIntake.Qsupplied,
                        Ppu = transport.Rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = productIntake.Balance,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Paid = productIntake.Paid,
                        Remarks = productIntake.Remarks,
                        AuditId = auditId,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = productIntake.DrAccNo,
                        CrAccNo = productIntake.CrAccNo
                    };
                    _context.ProductIntake.Add(collection);

                    // Credit transpoter transport amount
                    productIntake.CR = productIntake.Qsupplied * transport.Rate;
                    productIntake.DR = 0;
                    productIntake.Balance = utilities.GetBalance(productIntake);
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = transport.TransCode.Trim(),
                        TransDate = productIntake.TransDate,
                        TransTime = productIntake.TransTime,
                        ProductType = productIntake.ProductType,
                        Qsupplied = productIntake.Qsupplied,
                        Ppu = transport.Rate,
                        CR = productIntake.CR,
                        DR = productIntake.DR,
                        Balance = productIntake.Balance,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Paid = productIntake.Paid,
                        Remarks = productIntake.Remarks,
                        AuditId = auditId,
                        Auditdatetime = productIntake.Auditdatetime,
                        Branch = productIntake.Branch,
                        SaccoCode = productIntake.SaccoCode,
                        DrAccNo = price.TransportDrAccNo,
                        CrAccNo = price.TransportCrAccNo
                    });
                }

                var startDate = new DateTime(productIntake.TransDate.Year, productIntake.TransDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var commulated = _context.ProductIntake.Where(s => s.Sno == productIntake.Sno && s.SaccoCode == sacco
                && s.TransDate >= startDate && s.TransDate <= endDate).Sum(s => s.Qsupplied);
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

                _context.Gltransactions.Add(new Gltransaction
                {
                    AuditId = auditId,
                    TransDate = DateTime.Today,
                    Amount = (decimal)productIntake.CR,
                    AuditTime = DateTime.Now,
                    Source = productIntake.Sno,
                    TransDescript = "Intake",
                    Transactionno = $"{auditId}{DateTime.Now}",
                    SaccoCode = sacco,
                    DrAccNo = price.DrAccNo,
                    CrAccNo = price.CrAccNo,
                });
                
                _context.SaveChanges();
                _notyf.Success("Intake saved successfully");
                return RedirectToAction("GetIntakeReceipt", "PdfReport", new { id = collection.Id });
            }
            return View(productIntake);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            long.TryParse(productIntake.Sno, out long sno);
            DateTime startdate = new DateTime(productIntake.TransDate.Year, productIntake.TransDate.Month, 1);
            DateTime enddate = startdate.AddMonths(1).AddDays(-1);
            productIntake.Description = productIntake?.Description ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (!_context.DSuppliers.Any(i => i.Sno == sno && i.Scode == sacco && i.Active == true && i.Approval == true))
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
            && i.TransDate >= startdate && i.TransDate <= enddate))
            {
                _notyf.Error("Sorry, Supplier has not deliver any product for this month"+" "+ startdate+ "To " + " "+ enddate );
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco),
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
                    DSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco),
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
            //long.TryParse(, out long sno);
            productIntake.Remarks = productIntake?.Remarks ?? "";
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco);
            if (!_context.DTransporters.Any(i => i.TransCode == productIntake.Sno && i.Active == true && i.ParentT == sacco))
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
        public async Task<IActionResult> CreateCorrection([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            productIntake.SaccoCode = sacco;
            var branch = _context.DBranch.FirstOrDefault(b => b.Bcode.ToUpper().Equals(sacco.ToUpper()));
            productIntake.Branch = branch.Bname;
            productIntake.DR = productIntake?.DR ?? 0;
            productIntake.CR = productIntake?.CR ?? 0;
            long.TryParse(productIntake.Sno, out long sno);
            productIntake.Description = productIntake?.Description ?? "";
            if (sno < 1)
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return View(productIntake);
            }
            if (!_context.DSuppliers.Any(s => s.Sno == sno))
            {
                _notyf.Error("Sorry, Supplier No. not found");
                return View(productIntake);
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return View(productIntake);
            }
            if (productIntake.Qsupplied == 0)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(productIntake);
            }
            var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno && s.Scode == sacco);
            if (supplier == null) 
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return View(productIntake);
            }
            if (!supplier.Active || !supplier.Approval)
            {
                _notyf.Error("Sorry, Supplier must be approved and active");
                return View(productIntake);
            }
            if (productIntake.CR < 0)
            {
                productIntake.DR = productIntake.CR;
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

            var amount = productIntake.DR > 0 ? productIntake.DR : productIntake.DR;
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = auditId,
                TransDate = DateTime.Today,
                Amount = (decimal)amount,
                AuditTime = DateTime.Now,
                Source = productIntake.Sno,
                TransDescript = "Correction",
                Transactionno = $"{auditId}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = productIntake.DrAccNo,
                CrAccNo = productIntake.CrAccNo,
            });

            if (ModelState.IsValid)
            { 
                productIntake.AuditId = auditId ?? "";
                productIntake.Description = "Correction";
                productIntake.TransactionType = TransactionType.Correction;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Balance = utilities.GetBalance(productIntake);
                _context.Add(productIntake);
                await _context.SaveChangesAsync();
                _notyf.Success("Correction saved successfully");
                return RedirectToAction(nameof(CorrectionList));
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
