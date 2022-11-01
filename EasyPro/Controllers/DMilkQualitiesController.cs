﻿using System;
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
using EasyPro.ViewModels;

namespace EasyPro.Controllers
{
    public class DMilkQualitiesController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public DMilkQualitiesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }

        // GET: DMilkQualities
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var intakes = GetAssignedTransporters();
            intakes = intakes.OrderByDescending(m => m.TransDate).ToList();
            return View(intakes);
        }
        public IActionResult RejectsIndex()
        {
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(0);
            var endDate = month.AddMonths(1).AddDays(-1);

            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var rejects = _context.milkcontrol2.Where(i => i.code.ToUpper().Equals(sacco.ToUpper()) 
            && i.transdate >= startDate && i.transdate <= endDate).ToList();
            return View(rejects);
        }
        public IActionResult RejectsCreate()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectsCreate([Bind("Id,Intake, SQuantity, Reject, transdate, auditid, cfa, Spillage, FromStation, Tostation, Bf, code, Branch")] milkcontrol2 milkcontrol2)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
            if (milkcontrol2.Intake == 0)
            {
                _notyf.Error("Sorry, Intake cannot be zero");
                GetInitialValues();
                return View();
            }
            var milkchecking = _context.milkcontrol2.FirstOrDefault(u => u.code == sacco && u.Branch == milkcontrol2.Branch
            && u.transdate == milkcontrol2.transdate);
            if (milkchecking!=null)
            {
                _notyf.Error("Sorry, Data already exist");
                GetInitialValues();
                return View();
            }

            if (ModelState.IsValid)
            {
                milkcontrol2.code = sacco;
                milkcontrol2.auditid = user;
                _context.Add(milkcontrol2);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(RejectsIndex));
            }
            return View(milkcontrol2);
        }
        public IActionResult MilktransferIndex()
        {
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(0);
            var endDate = month.AddMonths(1).AddDays(-1);

            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var rejects = _context.Milktransfer.Where(i => i.Code.ToUpper().Equals(sacco.ToUpper()) && i.Transdate>= startDate && i.Transdate<= endDate).ToList();
            rejects = rejects.OrderByDescending(y=>y.Transdate).ToList();
            return View(rejects);
        }
        [HttpGet]
        public JsonResult SelectedDateIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var todaysIntake = GetTodaysIntake(date);
            return Json(todaysIntake);
        }

        private decimal GetTodaysIntake(DateTime date)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var intakes = _context.ProductIntake.Where(i => i.TransDate == date && i.Branch== saccobranch && i.SaccoCode == sacco && (i.Description == "Correction"|| i.Description == "Intake"));
            var todaysIntake = intakes.Sum(i => i.Qsupplied);
            return todaysIntake;
        }
        private List<MilkqualityVM> GetAssignedTransporters()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var today = DateTime.Now;
            var month = new DateTime(today.Year, today.Month, 1);
            var startdate = month;
            var enddate = month.AddMonths(1).AddDays(-1);
            var transdeduction = _context.DMilkQuality.Where(i => i.code.ToUpper().Equals(sacco.ToUpper()) 
            && i.RejDate>= startdate && i.RejDate<= enddate).ToList();
            var intakes = new List<MilkqualityVM>();
            foreach (var intake in transdeduction)
            {
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == intake.Sno && i.Zone== intake.Branch && i.Scode.ToUpper().Equals(sacco.ToUpper()));
                if (supplier != null)
                {
                    intakes.Add(new MilkqualityVM
                    {
                        Id= intake.Id,
                        Sno = intake.Sno.ToString(),
                        Name = supplier.Names,
                        TransDate = intake.RejDate,
                        Approximate=intake.ApproxKgs,
                        Delivered = intake.DeKgs,
                        Transporter = intake.TransMode,
                        Organoloptic = intake.Organoleptic,
                        Alcohol = intake.Alcohol,
                        Antibiotic = intake.Antibiotic,
                        Reason = intake.RejReasons,
                        Zone = intake.Branch,
                    });
                }
            }

            return intakes;
        }
        // GET: DMilkQualities/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var dMilkQuality = await _context.DMilkQuality
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dMilkQuality == null)
            {
                return NotFound();
            }

            return View(dMilkQuality);
        }

        // GET: DMilkQualities/Create
        public IActionResult MilktransferCreate()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MilktransferCreate([Bind("Id, Transdate, fromStation, Tostation, FromBranch, ToBranch, auditid, Intake, Code")] Milktransfer milktransfer)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
            if (milktransfer.Intake == 0)
            {
                _notyf.Error("Sorry, Intake cannot be zero");
                GetInitialValues();
                return View();
            }
            if (!_context.Milktransfer.Any(u=>u.Code==sacco && u.Transdate== milktransfer.Transdate))
            {
                _notyf.Error("Sorry, Transfer already exist");
                GetInitialValues();
                return View();
            }

            if (ModelState.IsValid)
            {
                milktransfer.Code = sacco;
                milktransfer.auditid = user;
                _context.Add(milktransfer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(milktransfer);
        }
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            return View();
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";
            ViewBag.sacco = sacco;
            var counties = _context.County.Select(b => b.Name).ToList();
            ViewBag.counties = new SelectList(counties);
            var SubCountyName = _context.SubCounty.ToList();
            ViewBag.SubCountyName = SubCountyName;
            ViewBag.subCounties = new SelectList(SubCountyName, "Name", "Name");
            var WardSubCounty = _context.Ward.ToList();
            ViewBag.WardSubCounty = WardSubCounty;
            ViewBag.wards = new SelectList(WardSubCounty, "Name", "Name");
            var locations = _context.DLocations.Where(l => l.Lcode == sacco && l.Branch == saccoBranch).Select(b => b.Lname).ToList();
            ViewBag.locations = new SelectList(locations);

            var banksname = _context.DBanks.Where(a => a.BankCode == sacco).Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            var brances = _context.DBranch.Where(a => a.Bcode == sacco).Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

            var dSuppliers = _context.DSuppliers.Where(a => a.Scode == sacco).ToList();
            ViewBag.dSuppliers = dSuppliers;

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);
            if (zones.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;
            //var TransMode = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            //ViewBag.TransMode = new SelectList(TransMode);
            List<SelectListItem> Pcheck = new()
            {
                new SelectListItem { Value = "1", Text = "YES" },
                new SelectListItem {Value="0", Text = "NO" },
            };
            ViewBag.Pcheck = Pcheck ;

            List<SelectListItem> Antibiotic = new()
            {
                new SelectListItem { Text = "Positive" },
                new SelectListItem {Text = "Negative" },
            };
            ViewBag.Antibiotic = Antibiotic;

            List<SelectListItem> Conttype = new()
            {
                new SelectListItem { Value = "ALUMINIUM", Text = "ALUMINIUM" },
                new SelectListItem { Value = "GLASS", Text = "GLASS" },
                new SelectListItem { Value = "PLASTIC", Text = "PLASTIC" },
                new SelectListItem { Value = "Stainless steel", Text = "Stainless steel" },
            };
            ViewBag.Conttype = Conttype;

            List<SelectListItem> Ttransporter = new()
            {
                new SelectListItem { Value = "Individual Farmer", Text = "Individual Farmer" },
                new SelectListItem { Value = "Trader", Text = "Trader" },
                new SelectListItem { Value = "Bulk Transporter", Text = "Bulk Transporter" },
            };
            ViewBag.Ttransporter = Ttransporter;

            List<SelectListItem> TransMode = new()
            {
                new SelectListItem { Text = "BICYCLE" },
                new SelectListItem { Text = "DONKEY" },
                new SelectListItem { Text = "HANDCART" },
                new SelectListItem { Text = "LORRY" },
                new SelectListItem { Text = "MOTOR CYCLE" },
                new SelectListItem { Text = "ON FOOT" },
                new SelectListItem { Text = "PICK UP TRUCK" },
                new SelectListItem { Text = "TRACTOR" },
            };
            ViewBag.TransMode = TransMode;

            List<SelectListItem> lac = new()
            {
                new SelectListItem { Text = "1.000" },
                new SelectListItem { Text = "1.001" },
                new SelectListItem { Text = "1.002" },
                new SelectListItem { Text = "1.003" },
                new SelectListItem { Text = "1.004" },
                new SelectListItem { Text = "1.005" },
                new SelectListItem { Text = "1.006" },
                new SelectListItem { Text = "1.007" },
                new SelectListItem { Text = "1.008" },
                new SelectListItem { Text = "1.009" },
                new SelectListItem { Text = "1.010" },
                new SelectListItem { Text = "1.011" },
                new SelectListItem { Text = "1.012" },
                new SelectListItem { Text = "1.013" },
                new SelectListItem { Text = "1.014" },
                new SelectListItem { Text = "1.015" },
                new SelectListItem { Text = "1.016" },
                new SelectListItem { Text = "1.017" },
                new SelectListItem { Text = "1.018" },
                new SelectListItem { Text = "1.019" },
                new SelectListItem { Text = "1.020" },
                new SelectListItem { Text = "1.021" },
                new SelectListItem { Text = "1.022" },
                new SelectListItem { Text = "1.023" },
                new SelectListItem { Text = "1.024" },
                new SelectListItem { Text = "1.025" },
                new SelectListItem { Text = "1.026" },
                new SelectListItem { Text = "1.027" },
                new SelectListItem { Text = "1.028" },
                new SelectListItem { Text = "1.029" },
                new SelectListItem { Text = "1.030" },
                new SelectListItem { Text = "1.031" },
                new SelectListItem { Text = "1.032" },
                new SelectListItem { Text = "1.033" }

            };
            ViewBag.Lact = lac;
            List<SelectListItem> Rez = new()
            {
                new SelectListItem { Text = "0" },
                new SelectListItem { Text = "1" },
                new SelectListItem { Text = "2" },
                new SelectListItem { Text = "3" },
                new SelectListItem { Text = "4" },
                new SelectListItem { Text = "5" },
                new SelectListItem { Text = "6" },
            };
            ViewBag.Rez = Rez;
            List<SelectListItem> Alcohol = new()
            {
                new SelectListItem { Value = "Positive", Text = "Positive" },
                new SelectListItem { Value = "Negative", Text = "Negative" },
            };
            ViewBag.Alcohol = Alcohol;
            List<SelectListItem> Organoleptic = new()
            {
                new SelectListItem { Value = "Good", Text = "Good" },
                new SelectListItem { Value = "Bad", Text = "Bad" },
            };
            ViewBag.Organoleptic = Organoleptic;
        }
        // POST: DMilkQualities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,RejDate,ApproxKgs,DeKgs,ContCpcity,Ttransporter,Conttype,TransMode,Organoleptic,Rez,Lact,PlateCount,Alcohol,TimeIn,TimeOut,Pcheck,Dramsk,RejReasons,Auditid,Auditdatetime,Antibiotic,Branch")] DMilkQuality dMilkQuality)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
            if (ModelState.IsValid)
            {
                dMilkQuality.code = sacco;
                dMilkQuality.Auditdatetime = DateTime.Now;
                dMilkQuality.Auditid = user;
                _context.Add(dMilkQuality);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dMilkQuality);
        }

        // GET: DMilkQualities/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var dMilkQuality = await _context.DMilkQuality.FindAsync(id);
            if (dMilkQuality == null)
            {
                return NotFound();
            }
            return View(dMilkQuality);
        }

        // POST: DMilkQualities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,RejDate,ApproxKgs,DeKgs,ContCpcity,Ttransporter,Conttype,TransMode,Organoleptic,Rez,Lact,PlateCount,Alcohol,TimeIn,TimeOut,Pcheck,Dramsk,RejReasons,Auditid,Auditdatetime")] DMilkQuality dMilkQuality)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id != dMilkQuality.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dMilkQuality);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DMilkQualityExists(dMilkQuality.Id))
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
            return View(dMilkQuality);
        }

        // GET: DMilkQualities/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var dMilkQuality = await _context.DMilkQuality
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dMilkQuality == null)
            {
                return NotFound();
            }

            return View(dMilkQuality);
        }
        public async Task<IActionResult> RejectsDelete(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var dMilkQuality = await _context.milkcontrol2
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dMilkQuality == null)
            {
                return NotFound();
            }

            return View(dMilkQuality);
        }

        public async Task<IActionResult> MilktransferDelete(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }

            var milktransfer = await _context.Milktransfer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (milktransfer == null)
            {
                return NotFound();
            }

            return View(milktransfer);
        }
        [HttpPost, ActionName("MilktransferDelete")]//
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MilktransferDeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var milktransfer = await _context.Milktransfer.FindAsync(id);
            _context.Milktransfer.Remove(milktransfer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MilktransferIndex));
        }
        [HttpPost, ActionName("RejectsDelete")]//MilktransferDelete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectsDeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var milkcontrol2 = await _context.milkcontrol2.FindAsync(id);
            _context.milkcontrol2.Remove(milkcontrol2);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(RejectsIndex));
        }

        // POST: DMilkQualities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var dMilkQuality = await _context.DMilkQuality.FindAsync(id);
            _context.DMilkQuality.Remove(dMilkQuality);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DMilkQualityExists(long id)
        {
            return _context.DMilkQuality.Any(e => e.Id == id);
        }
    }
}