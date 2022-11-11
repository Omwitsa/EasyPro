﻿using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Hosting;
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
    public class MilkBalancingController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public MilkBalancingController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            var date = DateTime.Now;
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            PostTransporterIntake();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var suppliers = _context.TransportersBalancings
                .Where(i => i.Code.ToUpper().Equals(sacco.ToUpper()) && i.Date >= startDate 
                && i.Date <= endDate).OrderByDescending(s => s.Date).ToList();
            
            return View(suppliers);
        }

        public void PostTransporterIntake()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var transporterIntakes = _context.d_TransporterIntake.Where(i => i.Posted != "Posted"
            && i.SaccoCode == sacco && i.Branch== saccobranch).ToList();
            var intakes = transporterIntakes.GroupBy(i => i.Date).ToList();
            intakes.ForEach(i =>
            {
                var transGroups = i.GroupBy(t => t.TransCode).ToList();
                transGroups.ForEach(t =>
                {
                    var intake = t.FirstOrDefault();
                    var qty = t.Sum(t => t.ActualKg);
                    var savedIntakes = GetTransporterIntakes(intake.TransCode, intake.Date);
                    var supplies = savedIntakes.Sum(s => s.Qsupplied);
                    var variance = supplies - qty;
                    _context.TransportersBalancings.Add(new TransportersBalancing
                    {
                        Date = intake.Date,
                        Transporter = intake.TransCode,
                        Quantity = "" + supplies,
                        ActualBal = "" + qty,
                        Rejects = "0",
                        Spillage = "0",
                        Varriance = "" + variance,
                        Code = sacco,
                        Branch = saccobranch,
                    });

                });

                i.ToList().ForEach(t => t.Posted = "Posted");
                _context.SaveChanges();
            });
        }

        public IActionResult Create(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            GetInitialValues();
            

            return View(new TransportersBalancing
            {
                Spillage = "0",
                Varriance = "0",
                Rejects = "0",
            });
        }
        // GET: DSuppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            if (id == null)
            {
                return NotFound();
            }
            var TransportersBalancings = await _context.TransportersBalancings.FindAsync(id);
            if (TransportersBalancings == null)
            {
                return NotFound();
            }
            var transporter = _context.DTransporters.FirstOrDefault(i=>i.TransCode.ToUpper().Equals(TransportersBalancings.Transporter.ToUpper())
            && i.ParentT== TransportersBalancings.Code && i.Tbranch== TransportersBalancings.Branch);
            
            return View(new TransportersBalancing
            {
                Code = TransportersBalancings.Transporter,
                Transporter= transporter.TransName,
                Date= TransportersBalancings.Date,
                Varriance = TransportersBalancings.Varriance,
                Spillage = TransportersBalancings.Spillage,
                Rejects= TransportersBalancings.Rejects
            });
        }
        // GET: DSuppliers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var TransportersBalancings = await _context.TransportersBalancings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (TransportersBalancings == null)
            {
                return NotFound();
            }

            return View(TransportersBalancings);
        }

        // POST: DSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dSupplier = await _context.TransportersBalancings.FindAsync(id);
            _context.TransportersBalancings.Remove(dSupplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var agproducts = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            var TransportersName = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if(user.AccessLevel == AccessLevel.Branch)
            {
                agproducts = agproducts.Where(i => i.Tbranch == saccobranch).ToList();
                TransportersName = TransportersName.Where(t => t.Tbranch == saccobranch).ToList();
            }
            ViewBag.agproductsall = agproducts;
            ViewBag.Transporterslist = new SelectList(TransportersName, "TransName", "TransName");
            if(sacco.ToUpper()== "MBURUGU DAIRY F.C.S")
                ViewBag.checkiftoenable = 1;
            else
                ViewBag.checkiftoenable = 0;
        }

        [HttpPost]
        public JsonResult GetSuppliedItems([FromBody] ProductBalancingFilterVm filter)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
                
                if (string.IsNullOrEmpty(filter.TCode))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }

                var suppliersdeliveries = _context.d_TransporterIntake.Where(i => i.Date == filter.Date && i.TransCode.ToUpper().Equals(filter.TCode.ToUpper())
                && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).Sum(i => i.ActualKg);
                var intakes = GetTransporterIntakes(filter.TCode, filter.Date);
                return Json(new {
                    intakes,
                    suppliersdeliveries
                });
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        private IEnumerable<ProductIntake> GetTransporterIntakes(string transCode, DateTime? date)
        {
            transCode = transCode ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var transporterSuppliers = _context.DTransports.Where(t => t.TransCode.ToUpper().Equals(transCode.ToUpper()) && t.saccocode == sacco)
                .Select(t => t.Sno.ToString());

            var notTransporterSuppliers = _context.ProductIntake.Where(i => i.AuditId.ToUpper().Equals(transCode.ToUpper()) 
                && i.SaccoCode == sacco && !transporterSuppliers.Contains(i.Sno) && i.TransDate == date)
                .Select(t => t.Sno).Distinct().ToList();

            var intakes = _context.ProductIntake.Where(s => s.TransDate == date && s.SaccoCode == sacco && (s.Description == "Intake" || s.Description == "Correction") 
            && (transporterSuppliers.Contains(s.Sno) || notTransporterSuppliers.Contains(s.Sno)) ).ToList();

            return intakes;
        }


        [HttpPost]//editVariance
        public JsonResult SaveVariance([FromBody] TransportersBalancing balancing )
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(balancing.Transporter))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.Quantity))
                {
                    _notyf.Error("Sorry, Supplied items could not be found");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.ActualBal))
                {
                    _notyf.Error("Sorry, Kindy provide actuals");
                    return Json("");
                }
                var checkexist = _context.TransportersBalancings
                    .FirstOrDefault(s => s.Date == balancing.Date && s.Code == sacco && s.Branch== saccoBranch && s.Transporter == balancing.Transporter);
                if (checkexist!=null)
                {
                    var id = checkexist.Id;
                    var transportersBalancing = _context.TransportersBalancings.Find(id);
                    _context.TransportersBalancings.Remove(transportersBalancing);
                }
                

                    balancing.Code = sacco;
                    balancing.Branch = saccoBranch;
                    _context.TransportersBalancings.Add(balancing);
               
                _context.SaveChanges();
                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult EditVariance([FromBody] TransportersBalancing balancing)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(balancing.Transporter))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.Quantity))
                {
                    _notyf.Error("Sorry, Supplied items could not be found");
                    return Json("");
                }
                if (string.IsNullOrEmpty(balancing.ActualBal))
                {
                    _notyf.Error("Sorry, Kindy provide actuals");
                    return Json("");
                }

                balancing.Code = sacco;
                _context.TransportersBalancings.Update(balancing);
                _context.SaveChanges();
                _notyf.Success("Edited successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
