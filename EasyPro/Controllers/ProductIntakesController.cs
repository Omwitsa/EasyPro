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
            return View(await _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Intake && i.TransDate == DateTime.Today)
                .ToListAsync());


        }
        public async Task<IActionResult> TDeductionList()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today).ToListAsync();
            var intakes = new List<ProductIntakeVm>();
            foreach (var intake in productIntakes)
            {
                var supplier = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.Sno);
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
            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction && c.Qsupplied == 0 && c.SaccoCode == sacco && c.TransDate == DateTime.Today).ToListAsync();
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
            return View(await _context.ProductIntake
                .Where(c => c.TransactionType == TransactionType.Correction && c.TransDate == DateTime.Today)
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
            var suppliers = _context.DSuppliers.ToList();
            ViewBag.suppliers = suppliers;
            var products = _context.DPrices.ToList();
            ViewBag.products = new SelectList(products, "Products", "Products");
            ViewBag.productPrices = products;
        }

        public IActionResult CreateDeduction()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            Farmersobj = new FarmersVM()
            {
                DSuppliers = _context.DSuppliers,

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
            Farmersobj = new FarmersVM()
            {
                DTransporters = _context.DTransporters,
                ProductIntake = new ProductIntake
                {
                    TransDate = DateTime.Today
                }
            };
            return View(Farmersobj);
        }
        private void GetInitialValues()
        {
            var Descriptionname = _context.DDcodes.Select(b => b.Description).ToList();
            ViewBag.Description = new SelectList(Descriptionname);

            var brances = _context.DBranch.Select(b => b.Bname).ToList();
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
        public async Task<IActionResult> Create([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            productIntake.Description = productIntake?.Description ?? "";
            long.TryParse(productIntake.Sno, out long sno);
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
            if (productIntake.Qsupplied < 1)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(productIntake);
            }
            if (!_context.DSuppliers.Any(s => s.Sno == sno))
            {
                _notyf.Error("Sorry, Supplier does not exist");
                return View(productIntake);
            }
            if (ModelState.IsValid)
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                productIntake.SaccoCode = sacco;
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                productIntake.TransactionType = TransactionType.Intake;
                productIntake.TransDate = DateTime.Today;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Balance = GetBalance(productIntake);
                _context.ProductIntake.Add(new ProductIntake { 
                    Sno = productIntake.Sno.Trim(),
                    TransDate = productIntake.TransDate,
                    TransTime = productIntake.TransTime,
                    ProductType = productIntake.ProductType,
                    Qsupplied = productIntake.Qsupplied,
                    Ppu = productIntake.Ppu,
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
                    SaccoCode = productIntake.SaccoCode
                });

                var transport = _context.DTransports.FirstOrDefault(t => t.Sno == sno && t.Active
                && t.producttype.ToUpper().Equals(productIntake.ProductType.ToUpper())
                && t.saccocode.ToUpper().Equals(productIntake.SaccoCode.ToUpper()));
                if (transport != null)
                {
                    // Debit supplier transport amount
                    productIntake.CR = 0;
                    productIntake.DR = productIntake.Qsupplied * transport.Rate;
                    productIntake.Balance = productIntake.Balance - productIntake.DR;
                    _context.ProductIntake.Add(new ProductIntake
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
                        SaccoCode = productIntake.SaccoCode
                    });

                    // Credit transpoter transport amount
                    productIntake.CR = productIntake.Qsupplied * transport.Rate;
                    productIntake.DR = 0;
                    productIntake.Balance = GetBalance(productIntake); ;
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
                        SaccoCode = productIntake.SaccoCode
                    });
                }
                _context.SaveChanges();
                _notyf.Success("Intake saved successfully");
                return RedirectToAction("GetIntakeReceipt", "PdfReport", new { id = productIntake.Id });
            }
            return View(productIntake);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            utilities.SetUpPrivileges(this);
            long.TryParse(productIntake.Sno, out long sno);
            productIntake.Description = productIntake?.Description ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (!_context.DSuppliers.Any(i => i.Sno == sno && i.Scode == sacco && i.Active == true && i.Approval == true))
            {
                _notyf.Error("Sorry, Farmer Number code does not exist");
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = _context.DSuppliers,
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
                    DSuppliers = _context.DSuppliers,
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
                productIntake.Description = "0";
                productIntake.Balance = 0;
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
            if (!_context.DTransporters.Any(i => i.TransCode == productIntake.Sno && i.Active == true && i.ParentT == sacco))
            {
                _notyf.Error("Sorry, Transporter code does not exist");
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DTransporters = _context.DTransporters,
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
                    DTransporters = _context.DTransporters,
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
                    DTransporters = _context.DTransporters,
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
                productIntake.Description = "0";
                productIntake.TransactionType = TransactionType.Deduction;
                productIntake.Balance = 0;
                productIntake.SaccoCode = sacco;
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
            if (productIntake.Qsupplied == null)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(productIntake);
            }
            if (productIntake.CR < 0)
            {
                productIntake.DR = -productIntake.CR;
                productIntake.CR = 0;
            }
            if (ModelState.IsValid)
            {
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser);
                productIntake.AuditId = auditId ?? "";
                productIntake.Description = "Correction";
                productIntake.TransactionType = TransactionType.Correction;
                productIntake.TransDate = DateTime.Today;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Balance = GetBalance(productIntake);
                _context.Add(productIntake);
                await _context.SaveChangesAsync();
                _notyf.Success("Correction saved successfully");
                return RedirectToAction(nameof(CorrectionList));
            }
            return View(productIntake);
        }

        private decimal? GetBalance(ProductIntake productIntake)
        {
            var latestIntake = _context.ProductIntake.Where(i => i.Sno == productIntake.Sno)
                    .OrderByDescending(i => i.Id).FirstOrDefault();
            if (latestIntake == null)
                latestIntake = new ProductIntake();
            latestIntake.Balance = latestIntake?.Balance ?? 0;
            productIntake.DR = productIntake?.DR ?? 0;
            productIntake.CR = productIntake?.CR ?? 0;
            var balance = latestIntake.Balance + productIntake.CR - productIntake.DR;
            return balance;
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
