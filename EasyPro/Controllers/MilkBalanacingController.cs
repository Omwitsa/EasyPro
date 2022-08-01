using AspNetCoreHero.ToastNotification.Abstractions;
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var suppliers = _context.TransportersBalancings
                .Where(i => i.Code.ToUpper().Equals(sacco.ToUpper()) && i.Date >= startDate && i.Date <= endDate).OrderByDescending(s=>s.Date).ToList();
            return View(suppliers);
        }
        public IActionResult Create()
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
                ActualBal="0"
            });
        }
        // GET: DSuppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }
            var TransportersBalancings = await _context.TransportersBalancings.FindAsync(id);
            if (TransportersBalancings == null)
            {
                return NotFound();
            }

            return View(TransportersBalancings);
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var agproducts = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.agproductsall = agproducts;

            var TransportersName = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.Transporterslist = new SelectList(TransportersName, "TransName", "TransName");

        }
        [HttpPost]
        public JsonResult GetSuppliedItems([FromBody] ProductBalancingFilterVm filter)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(filter.TCode))
                {
                    _notyf.Error("Sorry, Kindly select transporter");
                    return Json("");
                }

                var transporterSuppliers = _context.DTransports.Where(t => t.TransCode.ToUpper().Equals(filter.TCode.ToUpper()) && t.saccocode == sacco)
                    .Select(t => t.Sno.ToString());

                var intakes = _context.ProductIntake.Where(s => s.TransDate== filter.Date && s.SaccoCode == sacco && transporterSuppliers.Contains(s.Sno)).ToList();

                return Json(intakes);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }



        [HttpPost]//editVariance
        public JsonResult SaveVariance([FromBody] TransportersBalancing balancing)
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
                var checkexist = _context.TransportersBalancings
                    .Any(s => s.Date == balancing.Date && s.Code == sacco && s.Transporter == balancing.Transporter);
                if (checkexist)
                {
                    _notyf.Error("Sorry, Intake for the Transporter already Balanced");
                    return Json("");
                }

                balancing.Code = sacco;
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
