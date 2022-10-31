using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using System;

namespace EasyPro.Controllers
{
    public class DSupplierDeducsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DSupplierDeducsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);

        }

        // GET: DSupplierDeducs
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.DSupplierDeducs.ToListAsync());
        }

        // GET: DSupplierDeducs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dSupplierDeduc = await _context.DSupplierDeducs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplierDeduc == null)
            {
                return NotFound();
            }

            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
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
        // POST: DSupplierDeducs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,DateDeduc,Description,Amount,Period,StartDate,EndDate,Auditid,Auditdatetime,Yyear,Remarks,Branch,Bonus,Status1,Status2,Status3,Status4,Status5,Status6,Branchcode")] DSupplierDeduc dSupplierDeduc)
        {
            utilities.SetUpPrivileges(this);
            var dSupplier1 = _context.DSupplierDeducs.Where(i => i.Sno == dSupplierDeduc.Sno).Count();
            if (dSupplier1 != 0)
            {
                GetInitialValues();
                _notyf.Error("Transporter code does not exist");
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Add(dSupplierDeduc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }
            GetInitialValues();
            var dSupplierDeduc = await _context.DSupplierDeducs.FindAsync(id);
            if (dSupplierDeduc == null)
            {
                return NotFound();
            }
            return View(dSupplierDeduc);
        }

        // POST: DSupplierDeducs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,DateDeduc,Description,Amount,Period,StartDate,EndDate,Auditid,Auditdatetime,Yyear,Remarks,Branch,Bonus,Status1,Status2,Status3,Status4,Status5,Status6,Branchcode")] DSupplierDeduc dSupplierDeduc)
        {
            utilities.SetUpPrivileges(this);
            if (id != dSupplierDeduc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dSupplierDeduc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DSupplierDeducExists(dSupplierDeduc.Id))
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
            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dSupplierDeduc = await _context.DSupplierDeducs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplierDeduc == null)
            {
                return NotFound();
            }

            return View(dSupplierDeduc);
        }

        // POST: DSupplierDeducs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dSupplierDeduc = await _context.DSupplierDeducs.FindAsync(id);
            _context.DSupplierDeducs.Remove(dSupplierDeduc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DSupplierDeducExists(long id)
        {
            return _context.DSupplierDeducs.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetStandingOrder()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.StandingOrder.Where(o => o.SaccoCode == sacco).ToListAsync());
        }

        public IActionResult StandingOrder()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var suppliers = _context.DSuppliers
                .Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.suppliers = suppliers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StandingOrder([Bind("Id,Sno,TransDate,StartDate,EndDate,Duration,Amount,Description,AuditId,Auditdatetime,SaccoCode,Zone")] StandingOrder standingOrder)
        {
            utilities.SetUpPrivileges(this);
            if (string.IsNullOrEmpty(standingOrder.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No");
                return View(standingOrder);
            }

            if(standingOrder.Amount == null || standingOrder.Amount < 1)
            {
                _notyf.Error("Sorry, Kindly provide standing order amount");
                return View(standingOrder);
            }

            if(standingOrder.StartDate == null || standingOrder.StartDate < DateTime.Today)
            {
                _notyf.Error("Sorry, Start date must be greater than today");
                return View(standingOrder);
            }

            if(standingOrder.Duration == null || standingOrder.Duration < 1)
            {
                _notyf.Error("Sorry, Kindly provide duration");
                return View(standingOrder);
            }

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            standingOrder.TransDate = DateTime.Today;
            standingOrder.EndDate = standingOrder.StartDate.GetValueOrDefault().AddMonths((int)standingOrder.Duration);
            standingOrder.SaccoCode = sacco;
            standingOrder.AuditId = auditId;
            standingOrder.Zone = standingOrder.Zone;

            _context.StandingOrder.Add(standingOrder);
            _context.SaveChanges();

            _notyf.Success("Standing order saved successfully");
            return RedirectToAction(nameof(GetStandingOrder));
        }

        public IActionResult ProcessStandingOrder()
        {
            utilities.SetUpPrivileges(this);
            var activeorders = _context.StandingOrder.Where(o => o.StartDate <= DateTime.Today && o.EndDate >= DateTime.Today).ToList();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            activeorders.ForEach(o =>
            {
                var exists = _context.ProductIntake.Any(i => i.Sno == o.Sno && i.TransDate <= DateTime.Today 
                && o.EndDate >= DateTime.Today && i.Remarks == "Standing Order" 
                && i.Description == o.Description && i.SaccoCode == sacco);
                if (!exists)
                {
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = o.Sno,
                        TransDate = DateTime.Today,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = o.Description,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = 0,
                        DR = o.Amount,
                        Balance = 0,
                        Description = o.Description,
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Standing Order",
                        AuditId = auditId,
                        Auditdatetime = DateTime.Now,
                        Branch = "",
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                }
            });

            _context.SaveChanges();
            _notyf.Success("Standing order processed successfully");
            return RedirectToAction(nameof(GetStandingOrder));
        }
    }
}
