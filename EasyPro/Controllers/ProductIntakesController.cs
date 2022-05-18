﻿using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.ViewModels;
using EasyPro.ViewModels.FarmersVM;
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

        public FarmersVM Farmersobj { get; private set; }

        public ProductIntakesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        // GET: ProductIntakes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductIntake
                .Where(i => i.TransactionType == TransactionType.Intake)
                .OrderByDescending(i => i.Id)
                .ToListAsync());
        }
        public async Task<IActionResult> DeductionList()
        {
            var today = DateTime.Now;
            var month = new DateTime(today.Year, today.Month, 1);
            var startdate = month;
            var enddate = startdate.AddMonths(1).AddDays(-1);

            var productIntakes = await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Deduction && c.TransDate >= startdate && c.TransDate <= enddate).ToListAsync();
            var intakes = new List<ProductIntakeVm>();
            foreach(var intake in productIntakes)
            {
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == intake.Sno);
                intakes.Add(new ProductIntakeVm { 
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
            return View(intakes);
        }

        public async Task<IActionResult> CorrectionList()
        {
            return View(await _context.ProductIntake
                .Where(c => c.TransactionType == TransactionType.Correction)
                .OrderByDescending(i => i.Id)
                .ToListAsync());
        }

        // GET: ProductIntakes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
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
            GetInitialValues();
            Farmersobj = new FarmersVM()
            {
                DSuppliers = _context.DSuppliers,
                ProductIntake= new Models.ProductIntake()
            };
            //return Json(new { data = Farmersobj });
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
            if (productIntake.Sno < 1)
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return View(productIntake);
            }
            if(!_context.DSuppliers.Any(s => s.Sno == productIntake.Sno))
            {
                _notyf.Error("Sorry, Supplier No. not found");
                return View(productIntake);
            }
            if (string.IsNullOrEmpty(productIntake.ProductType))
            {
                _notyf.Error("Sorry, Kindly select product type");
                return View(productIntake);
            }
            if (productIntake.Qsupplied == null || productIntake.Qsupplied < 1)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(productIntake);
            }
            if (ModelState.IsValid)
            {
                productIntake.Description = "Intake";
                productIntake.TransactionType = TransactionType.Intake;
                productIntake.TransDate = DateTime.Today;
                productIntake.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                productIntake.Balance = GetBalance(productIntake);
                _context.Add(productIntake);
                await _context.SaveChangesAsync();
                _notyf.Success("Intake saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(productIntake);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeduction([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            var Supplier = _context.DSuppliers.Where(i => i.Sno == productIntake.Sno && i.Active == true).Count();
            if (Supplier == 0)
            {
                GetInitialValues();
                Farmersobj = new FarmersVM()
                {
                    DSuppliers = _context.DSuppliers,
                    ProductIntake = new Models.ProductIntake()
                };
                //return Json(new { data = Farmersobj });
                return View(Farmersobj);
            }
            if (productIntake.Sno == 0)
            {
                GetInitialValues();
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
                productIntake.TransactionType = TransactionType.Deduction; 
                _context.Add(productIntake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DeductionList));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCorrection([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            if (productIntake.Sno < 1)
            {
                _notyf.Error("Sorry, Kindly provide supplier No.");
                return View(productIntake);
            }
            if (!_context.DSuppliers.Any(s => s.Sno == productIntake.Sno))
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
            if(productIntake.CR < 0)
            {
                productIntake.DR = -productIntake.CR;
                productIntake.CR = 0;
            }
            if (ModelState.IsValid)
            {
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
