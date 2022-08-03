using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EasyPro.Models;
using EasyPro.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.ViewModels;
using EasyPro.ViewModels.EnquiryVM;
using System.Collections.Generic;

namespace EasyPro.Controllers
{
    public class SupplierEnquiryController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;

        public SupplierEnquiryController(MORINGAContext context)
        {
            _context = context;
            utilities = new Utilities(context);
        }
        public TransportersEnquiryVM TransportersEnquiryVMobj { get; set; }
        public DTmpTransEnquery DTmpTransEnqueryobj { get; set; }
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            GetInitialValues();
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            //.ToUpper().Equals(sacco.ToUpper())
            ViewBag.suppliers = _context.DSuppliers.Where(i=>i.Scode.ToUpper().Equals(sacco.ToUpper())).Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
                IdNo = s.IdNo,
                PhoneNo = s.PhoneNo,
                AccNo = s.AccNo,
                Bbranch = s.Bbranch
            }).ToList();

            return View();
        }
        public IActionResult Transporters()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            //GetInitialValues();
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.transporters = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).Select(s => new DTransporter
            {
                TransCode = s.TransCode,
                TransName = s.TransName,
                CertNo = s.CertNo,
                Phoneno = s.Phoneno,
                Bcode= s.Bcode,
                Accno = s.Accno,
                Bbranch = s.Bbranch
            }).ToList();
            //TransportersEnquiryVMobj.FilterVm.DateFrom = startDate;
            //TransportersEnquiryVMobj.FilterVm.DateTo = enDate;
            return View();
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var products = _context.DBranchProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
        }
        [HttpPost]
        public JsonResult SuppliedProducts([FromBody] DSupplier supplier,DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);

            var intakes = _context.ProductIntake.Where(i => i.Sno == supplier.Sno.ToString() && i.SaccoCode.ToUpper()
            .Equals(sacco.ToUpper()) && i.TransDate>=date1 && i.TransDate<=date2).OrderByDescending(i => i.TransDate).ToList();
            return Json(intakes);
        }
        [HttpPost]
        public JsonResult SuppliedProducts2(string sno, string producttype, DateTime date1, DateTime date2)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate)
                .Where(i => i.Sno ==sno && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.ProductType==producttype && i.TransDate >= date1 && i.TransDate <= date2).ToList();
            return Json(intakes);
        }
        [HttpPost]
        public JsonResult SuppliedProducts3(string sno, DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);

            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate)
                .Where(i => i.Sno == sno && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.TransDate >= date1 && i.TransDate <= date2).ToList();
            return Json(intakes);
        }
        [HttpPost]
        public JsonResult SuppliedProductsTransporter(string sno, DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);

            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate)
                .Where(i => i.Sno == sno && i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.TransDate >= date1 && i.TransDate <= date2).ToList();
            return Json(intakes);
        }

        [HttpPost]
        public JsonResult SuppliedProductsTransporter1(string sno, DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            var resultsget = _context.ProductIntake.OrderByDescending(i => i.TransDate)
                        .Where(i => i.SaccoCode == sacco
                    && i.TransDate >= date1 && i.TransDate <= date2).ToList();
            var transExist =  _context.DTransporters.Any(u => u.TransCode == sno && u.Active == true && u.ParentT == sacco);
            if (transExist)
            {
                var transassign = _context.DTransports
                    .Where(u => u.TransCode == sno && u.Active == true && u.saccocode.ToUpper().Equals(sacco.ToUpper()));
                foreach(var snoo in transassign)
                {
                    var sumkgspersupplier = _context.ProductIntake
                        .Where(u => u.SaccoCode == sacco && u.TransDate >= date1 && u.TransDate <= date2 && u.Sno == snoo.Sno.ToString())
                        .Sum(i=>i.CR);
                    var all = _context.DTmpTransEnqueries;
                    DTmpTransEnqueryobj.sacco = sacco;
                    DTmpTransEnqueryobj.Sno = snoo.Sno.ToString();
                    DTmpTransEnqueryobj.TransDate = date2;
                    DTmpTransEnqueryobj.Cr = sumkgspersupplier;
                }
                var intakes = _context.DTmpTransEnqueries.OrderByDescending(i => i.TransDate)
                       .Where(i => i.Sno == sno && i.sacco.ToUpper().Equals(sacco.ToUpper())
                   && i.TransDate >= date1 && i.TransDate <= date2).ToList();
                //resultsget = intakes;
            }
            return Json(resultsget);
        }
    }
}
