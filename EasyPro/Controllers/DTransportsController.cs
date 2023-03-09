﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using EasyPro.ViewModels.TranssupplyVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace EasyPro.Controllers
{
    public class DTransportsController : Controller
    {
        private readonly MORINGAContext _context;
        private IEnumerable<DSupplier> DSuppliers;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public DTransportsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }
        public TransSuppliers TransSuppliersobj { get; private set; }
        public IEnumerable<DTransporter> DTransporter { get; private set; }
        public DTransporter dtransporterobj { get; private set; }
        // GET: DTransports
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var intakes = GetAssignedTransporters();
            intakes = intakes.OrderBy(K => K.Sno).ToList();
            return View(intakes);
        }
        private List<TransSuppliersVM> GetAssignedTransporters()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var today = DateTime.Now;
            var month = new DateTime(today.Year, today.Month, 1);
            var startdate = month;
            var enddate = startdate.AddMonths(1).AddDays(-1);
            var transdeduction = _context.DTransports.Where(i => i.Active && i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                transdeduction = transdeduction.Where(t => t.Branch == saccoBranch).ToList();
            var intakes = new List<TransSuppliersVM>();
            foreach (var intake in transdeduction)
            {
                var trans = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.TransCode.Trim().ToUpper() && i.ParentT.ToUpper().Equals(sacco.ToUpper()) && i.Tbranch == saccoBranch);
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == intake.Sno && i.Branch==saccoBranch && i.Scode.ToUpper().Equals(sacco.ToUpper()));
                if(supplier != null && trans != null)
                {
                    intakes.Add(new TransSuppliersVM
                    {
                        Id = intake.Id,
                        TransCode = trans.TransCode,
                        TransName = trans.TransName,
                        Sno = supplier.Sno,
                        Names = supplier.Names,
                        Rate = intake.Rate,
                        Startdate = intake.Startdate,
                        Morning= intake.Morning
                    });
                }
            }

            return intakes;
        }
        // GET: DTransports/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransport == null)
            {
                return NotFound();
            }
            return View(dTransport);
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var producttypes = _context.DPrices.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Products).ToList();
            ViewBag.producttypes = new SelectList(producttypes, "");
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

            //var zone = _context.Zones.Where(a => a.Code == sacco).ToList();
            if (zones.Count != 0)
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;

            List<SelectListItem> morningEve = new()
            {
                new SelectListItem { Text = "" },
                new SelectListItem { Text = "Morning" },
                new SelectListItem { Text = "Evening" },
            };
            ViewBag.morningEve = morningEve;
        }

        // GET: DTransports/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var transporters = _context.DTransporters.Where(t => t.ParentT == sacco);
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco);

            if(user.AccessLevel == AccessLevel.Branch)
            {
                transporters = transporters.Where(t => t.Tbranch == saccoBranch);
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);
            }

            TransSuppliersobj = new TransSuppliers
            {
                DTransport = new DTransport(),
                DTransporter = transporters,
                DSuppliers = suppliers,
            };

            GetInitialValues();
            //return Json(new { data = Farmersobj });
            return View(TransSuppliersobj);
        }
        [HttpGet]
        public JsonResult SelectedDateIntake( string sno)
        {
            sno = sno ?? "";
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var suppliers = _context.DSuppliers.Where(L => L.Sno.ToUpper().Equals(sno.ToUpper()) && L.Scode == sacco);
            

            var todaysIntake = suppliers.Select(b => b.Names).ToList();
            return Json(todaysIntake);
        }
        // POST: DTransports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,Sno,Rate,Startdate,Active,DateInactivate,Auditid,Auditdatetime,Isfrate,producttype,Zone,Morning")] DTransport dTransport)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var transporters = _context.DTransporters.Where(t => t.TransCode == dTransport.TransCode && t.Active && t.ParentT == sacco);
            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                transporters = transporters.Where(t => t.Tbranch == saccoBranch);
                suppliers = suppliers.Where(s => s.Branch == saccoBranch);
            }
                
            if (!transporters.Any())
            {
                _notyf.Error("TransCode not Exist or InActive");
                TransSuppliersobj = new TransSuppliers
                {
                    DTransport = new DTransport(),
                    DTransporter = transporters,
                    DSuppliers = suppliers,
                };
                GetInitialValues();
                return View(TransSuppliersobj);
            }
            var dTransporterExists = _context.DTransports
                .Any(i => i.Sno == dTransport.Sno && i.Active == true && i.producttype == dTransport.producttype
                && i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccoBranch && i.Morning== dTransport.Morning);
            if (dTransporterExists)
            {
                _notyf.Error("Supplier has an active Assignment");
                TransSuppliersobj = new TransSuppliers
                {
                    DTransport = new DTransport(),
                    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper()) && i.Tbranch == saccoBranch),
                    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccoBranch),
                };
                GetInitialValues();
                return View(TransSuppliersobj);
            }
            if (ModelState.IsValid)
            {
                dTransport.saccocode = sacco;
                dTransport.Branch = saccoBranch;
                _context.Add(dTransport);

                UpdateTransporter(dTransport);
                _notyf.Success("Assignment saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dTransport);
        }
        private void UpdateTransporter(DTransport dTransport)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            dTransport.Rate = dTransport?.Rate ?? 0;
            var sessionIntakes = _context.ProductIntake
                .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccoBranch && i.ProductType == dTransport.producttype
                && i.TransDate >= dTransport.Startdate && i.Qsupplied != 0 && (i.MornEvening == null || i.MornEvening == dTransport.Morning)).ToList();

            var transport = _context.DTransports.FirstOrDefault(t => t.saccocode == sacco && t.Branch == saccoBranch 
            && t.Sno.ToUpper().Equals(dTransport.Sno.ToUpper()) && (t.Morning == null || t.Morning == dTransport.Morning));
            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                       && p.Products.ToUpper().Equals(dTransport.producttype.ToUpper()));

            var transCode = transport?.TransCode ?? "";
            var intakeIds = new List<long>();
            var intakes = new List<ProductIntake>();
            sessionIntakes.ForEach(details =>
            {
                if (details.Sno.ToUpper().Equals(dTransport.Sno.ToUpper()))
                {
                    transCode = string.IsNullOrEmpty(transCode) ? details.AuditId : transCode;
                    details.Auditdatetime = DateTime.Now;
                    if (details.TransactionType == TransactionType.Intake)
                    {
                        intakes.Add(new ProductIntake
                        {
                            Sno = details.Sno,
                            TransDate = details.TransDate,
                            TransTime = details.TransTime,
                            ProductType = details.ProductType,
                            Qsupplied = details.Qsupplied,
                            Ppu = price.Price,
                            CR = details.Qsupplied * price.Price,
                            DR = details.DR,
                            Balance = details.Balance,
                            Description = details.Description,
                            TransactionType = details.TransactionType,
                            Remarks = details.Remarks,
                            AuditId = details.AuditId,
                            Auditdatetime = DateTime.Now,
                            Branch = details.Branch,
                            SaccoCode = details.SaccoCode,
                            DrAccNo = details.DrAccNo,
                            CrAccNo = details.CrAccNo,
                            Zone = details.Zone,
                            MornEvening = details.MornEvening
                        });
                        intakeIds.Add(details.Id);
                    }
                    if (details.TransactionType == TransactionType.Deduction)
                    {
                        intakes.Add(new ProductIntake
                        {
                            Sno = details.Sno,
                            TransDate = details.TransDate,
                            TransTime = details.TransTime,
                            ProductType = details.ProductType,
                            Qsupplied = details.Qsupplied,
                            Ppu = dTransport.Rate,
                            CR = details.CR,
                            DR = details.Qsupplied * dTransport.Rate,
                            Balance = details.Balance,
                            Description = details.Description,
                            TransactionType = details.TransactionType,
                            Remarks = details.Remarks,
                            AuditId = details.AuditId,
                            Auditdatetime = DateTime.Now,
                            Branch = details.Branch,
                            SaccoCode = details.SaccoCode,
                            DrAccNo = details.DrAccNo,
                            CrAccNo = details.CrAccNo,
                            Zone = details.Zone,
                            MornEvening = details.MornEvening
                        });
                        intakeIds.Add(details.Id);
                    }

                    var transporterEntry = sessionIntakes.FirstOrDefault(i => i.AuditId == details.AuditId && i.TransDate == details.TransDate 
                    && i.TransTime == details.TransTime && i.Qsupplied == details.Qsupplied && details.TransactionType == TransactionType.Deduction);
                    if (transporterEntry != null)
                    {
                        intakes.Add(new ProductIntake
                        {
                            Sno = dTransport.TransCode,
                            TransDate = transporterEntry.TransDate,
                            TransTime = transporterEntry.TransTime,
                            ProductType = transporterEntry.ProductType,
                            Qsupplied = transporterEntry.Qsupplied,
                            Ppu = dTransport.Rate,
                            CR = transporterEntry.Qsupplied * dTransport.Rate,
                            DR = transporterEntry.DR,
                            Balance = transporterEntry.Balance,
                            Description = transporterEntry.Description,
                            TransactionType = transporterEntry.TransactionType,
                            Remarks = transporterEntry.Remarks,
                            AuditId = transporterEntry.AuditId,
                            Auditdatetime = DateTime.Now,
                            Branch = transporterEntry.Branch,
                            SaccoCode = transporterEntry.SaccoCode,
                            DrAccNo = transporterEntry.DrAccNo,
                            CrAccNo = transporterEntry.CrAccNo,
                            Zone = transporterEntry.Zone,
                            MornEvening = transporterEntry.MornEvening
                        });
                        intakeIds.Add(transporterEntry.Id);
                    }
                }
            });

            if (intakes.Any())
            {
                var savedIntakes = _context.ProductIntake.Where(i => intakeIds.Contains(i.Id));
                _context.ProductIntake.RemoveRange(savedIntakes);
                _context.ProductIntake.AddRange(intakes);
                _context.SaveChanges();
            }
        }
        private decimal? GetBalance(DTransport dTransport)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var latestIntake = _context.ProductIntake.Where(i => i.Sno == dTransport.Sno.ToString() && i.Branch == saccoBranch && i.SaccoCode == sacco)
                    .OrderByDescending(i => i.Id).FirstOrDefault();
            if (latestIntake == null)
                latestIntake = new ProductIntake();
            latestIntake.Balance = latestIntake?.Balance ?? 0;
            latestIntake.DR = latestIntake?.DR ?? 0;
            latestIntake.CR = latestIntake?.CR ?? 0;
            var balance = latestIntake.Balance + latestIntake.CR - latestIntake.DR;
            return balance;
        }

        private void UpdateIntakeKgs(DTransport dTransport)
        {
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var transExisting = _context.DTransports
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Active && i.producttype == dTransport.producttype
                && i.Startdate <= dTransport.DateInactivate && i.Sno == dTransport.Sno && i.Branch == saccoBranch);
            if (transExisting.Any())
            {
                var selectassigntrans = _context.ProductIntake
                    .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.ProductType == dTransport.producttype
                    && i.TransDate >= dTransport.DateInactivate && i.Sno == dTransport.Sno.ToString() && i.Qsupplied != 0 
                    && i.Branch == saccoBranch);
                if (selectassigntrans.Any())
                {
                    _context.ProductIntake.RemoveRange(selectassigntrans);
                }
                foreach (var details in transExisting)
                {
                    details.Active = false;
                    details.DateInactivate = dTransport.DateInactivate;
                }
                _context.SaveChanges();
            }

        }
        // GET: DTransports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            //return Json(new { data = Farmersobj });
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports.FindAsync(id);
            if (dTransport == null)
            {
                return NotFound();
            }
            TransSuppliersobj = new TransSuppliers
            {
                DTransport = await _context.DTransports.FindAsync(id),
                DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
                DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
            };
           
            TransSuppliersobj.DTransporter = TransSuppliersobj.DTransporter.Where(i => i.TransCode == dTransport.TransCode && i.ParentT.ToUpper().Equals(sacco.ToUpper()));
            TransSuppliersobj.DSuppliers = TransSuppliersobj.DSuppliers.Where(i => i.Sno == dTransport.Sno && i.Scode.ToUpper().Equals(sacco.ToUpper()));
            return View(TransSuppliersobj);
        }

        // POST: DTransports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,Sno,Rate,Startdate,Active,DateInactivate,Auditid,Auditdatetime,Isfrate,producttype,Zone,Morning")] DTransport dTransport)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id != dTransport.Id)
            {
                return NotFound();
            }
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            sacco = sacco ?? "";
            if(dTransport.Startdate > dTransport.DateInactivate)
            {
                _notyf.Error("Data InActivated should not be less than Date Assigned, Try Again");
                TransSuppliersobj = new TransSuppliers
                {
                    DTransport = new DTransport(),
                    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper()) && i.Tbranch== saccoBranch),
                    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()) && i.Branch==sacco),
                };
                return View(TransSuppliersobj);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dTransport.saccocode = sacco;
                    //dTransport.producttype = dTransport.producttype;
                    _context.Update(dTransport);
                    _notyf.Success("Edit saved successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransportExists(dTransport.Id))
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
            return View(dTransport);
        }

        // GET: DTransports/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransport == null)
            {
                return NotFound();
            }

            return View(dTransport);
        }

        // POST: DTransports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dTransport = await _context.DTransports.FindAsync(id);
            _context.DTransports.Remove(dTransport);
            await _context.SaveChangesAsync();
            _notyf.Success("Data Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public JsonResult getsuppliers([FromBody] DSupplier supplier, string? filter, string? condition)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var Transporterssuppliers = _context.DTransports.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                Transporterssuppliers = Transporterssuppliers.Where(i => i.Branch == saccobranch).ToList();
            if (condition == "SNo")
                Transporterssuppliers = Transporterssuppliers.Where(i => i.Sno.ToUpper().Contains(filter.ToUpper())).ToList();
            if (condition == "TNo")
                Transporterssuppliers = Transporterssuppliers.Where(i => i.TransCode.ToUpper().Contains(filter.ToUpper())).ToList();

            Transporterssuppliers = Transporterssuppliers.OrderByDescending(i => i.Startdate).Take(505).ToList();
            var assignlist = new List<TransSuppliersVM>();
            foreach (var intake in Transporterssuppliers)
            {
               // intake = intake;
                var suppliername = _context.DSuppliers.FirstOrDefault(i => i.Sno.ToUpper().Equals(intake.Sno.ToUpper()) && i.Scode == sacco && i.Branch == intake.Branch);
                var transportername = _context.DTransporters.FirstOrDefault(i => i.TransCode.ToUpper().Equals(intake.TransCode.Trim().ToUpper()) && i.ParentT == sacco && i.Tbranch == intake.Branch);
                var supName = suppliername?.Names ?? "";
                var transName = transportername?.TransName ?? "";
                intake.Morning = string.IsNullOrEmpty(intake.Morning) ?  "All" : intake.Morning;
                    
                assignlist.Add(new TransSuppliersVM
                {
                    Id = intake.Id,
                    TransCode = intake.TransCode,
                    TransName = transName,
                    Sno = intake.Sno,
                    Names = supName,
                    Rate = intake.Rate,
                    Startdate = intake.Startdate,
                    Morning = intake.Morning
                });
            }
            _context.SaveChanges();
            return Json(assignlist);
        }
        private bool DTransportExists(long id)
        {
            return _context.DTransports.Any(e => e.Id == id);
        }
    }
}
