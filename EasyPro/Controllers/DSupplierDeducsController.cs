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
using Syncfusion.EJ2.Spreadsheet;
using Syncfusion.EJ2.Linq;

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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var deductions = _context.DSupplierDeducs.Where(m=>m.Branchcode == sacco).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                deductions = deductions.Where(b=>b.Branch== saccobranch).ToList();

                return View(deductions);
        }

        // GET: DSupplierDeducs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";

            var Descriptionname = _context.DDcodes.Where(d => d.Dcode == sacco).Select(b => b.Description).ToList();
            ViewBag.Description = new SelectList(Descriptionname);


            var brances = _context.DBranch.Where(m=>m.Bcode == sacco).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                brances = brances.Where(n=>n.Bname==saccoBranch).ToList();

            ViewBag.brances = new SelectList(brances.Select(b => b.Bname).ToList());

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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dSupplier1 = _context.DSupplierDeducs.Where(i => i.Sno == dSupplierDeduc.Sno ).Count();
            if (dSupplier1 != 0)
            {
                GetInitialValues();
                _notyf.Error("Transporter code does not exist");
                return View();
            }

            if (ModelState.IsValid)
            {
                dSupplierDeduc.Branch = saccobranch;
                dSupplierDeduc.Auditid = loggedInUser;
                _context.Add(dSupplierDeduc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dSupplierDeduc);
        }

        // GET: DSupplierDeducs/Edit/5 
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.StandingOrder.Where(o => o.SaccoCode == sacco && !o.Status).OrderByDescending(m=>m.Id).ToListAsync());
        }

        public async Task<IActionResult> GetStandingOrderStop()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.StandingOrder.Where(o => o.SaccoCode == sacco && o.Status).OrderByDescending(m => m.Id).ToListAsync());
        }
        public async Task<IActionResult> Stop(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dPreSet = await _context.StandingOrder.FindAsync(id);
            if (id != dPreSet.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    dPreSet.Status = true;
                    _context.Update(dPreSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPreSetExists(dPreSet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetStandingOrder));
            }
            return View(dPreSet);
        }

        public async Task<IActionResult> ResumeStop(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var StandingOrder = await _context.StandingOrder.FindAsync(id);
            if (id != StandingOrder.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    StandingOrder.Status = false;
                    _context.Update(StandingOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPreSetExists(StandingOrder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetStandingOrderStop));
            }
            return View(StandingOrder);

        }
        private bool DPreSetExists(long id)
        {
            return _context.StandingOrder.Any(e => e.Id == id);
        }

        public IActionResult StandingOrder()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetStandingOrderValues();
            return View();
        }

        private void SetStandingOrderValues()
        {
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;

            var Descriptionname = _context.DDcodes.Where(d => d.Dcode == sacco).Select(b => b.Description).ToList();
            ViewBag.Description = new SelectList(Descriptionname);

            var suppliers = _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper()));
            //var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            //if (user.AccessLevel == AccessLevel.Branch)
            //    suppliers = suppliers.Where(t => t.Branch == saccoBranch);
            ViewBag.suppliers = suppliers.OrderByDescending(i=>i.Id).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StandingOrder([Bind("Id,Sno,TransDate,StartDate,Installment,Amount,Paid,Description,AuditId,Auditdatetime,SaccoCode,Zone")] StandingOrder standingOrder)
        {
            standingOrder.Installment = standingOrder?.Installment ?? 0;
            standingOrder.Amount = standingOrder?.Amount ?? 0;
            standingOrder.StartDate = standingOrder?.StartDate ?? DateTime.Today;
            standingOrder.TransDate = DateTime.Today;
            standingOrder.Auditdatetime = DateTime.Now;
            standingOrder.Paid = 0;
            standingOrder.AuditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(standingOrder.AuditId))
                return Redirect("~/");
            standingOrder.SaccoCode = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            utilities.SetUpPrivileges(this);
            SetStandingOrderValues();
            standingOrder.Branch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            if (string.IsNullOrEmpty(standingOrder.Sno))
            {
                _notyf.Error("Sorry, Kindly provide supplier No");
                return View(standingOrder);
            }

            if (standingOrder.Installment < 1)
            {
                _notyf.Error("Sorry, Kindly provide installment");
                return View(standingOrder);
            }

            if (standingOrder.Amount < 1)
            {
                _notyf.Error("Sorry, Kindly provide standing order amount");
                return View(standingOrder);
            }

            if(standingOrder.StartDate < DateTime.Today)
            {
                _notyf.Error("Sorry, Start date must be greater than today");
                return View(standingOrder);
            }

            _context.StandingOrder.Add(standingOrder);
            _context.SaveChanges();

            _notyf.Success("Standing order saved successfully");
            return RedirectToAction(nameof(GetStandingOrder));
        }

        public async Task<IActionResult> ProcessStandingOrder()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";

            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var activeorders = await _context.StandingOrder.Where(o => o.SaccoCode == sacco && o.Paid < o.Amount && !o.Status).ToListAsync();
            var productIntakes = await _context.ProductIntake.Where(i => i.Remarks == "Society Standing Order" && i.TransDate >= startDate && i.TransDate <= endDate && i.SaccoCode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
            {
                //activeorders = activeorders.Where(i => i.Branch == saccoBranch).ToList();
                //productIntakes = productIntakes.Where(i => i.Branch == saccoBranch).ToList();
            }
                
            var intakes = new List<ProductIntake>();
            activeorders.ForEach(o =>
            {
                o.Sno = o?.Sno ?? "";
                o.Description = o?.Description ?? "";
                if (!productIntakes.Any(i => i.Sno.ToUpper().Equals(o.Sno.ToUpper()) && i.Description.ToUpper().Equals(o.Description.ToUpper())))
                {
                    var balance = o.Amount - o.Paid;
                    o.Installment = o.Installment > balance ? balance : o.Installment;
                    intakes.Add(new ProductIntake
                    {
                        Sno = o.Sno,
                        TransDate = endDate,
                        TransTime = DateTime.Now.TimeOfDay,
                        ProductType = o.Description,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = 0,
                        DR = o.Installment,
                        Balance = 0,
                        Description = o.Description,
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Society Standing Order",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });

                    o.Paid += o.Installment;
                }
            });

            if (intakes.Any())
            {
                await _context.ProductIntake.AddRangeAsync(intakes);
                await _context.SaveChangesAsync();
            }
            
            _notyf.Success("Standing order processed successfully");
            return RedirectToAction(nameof(GetStandingOrder));
        }
    }
}
