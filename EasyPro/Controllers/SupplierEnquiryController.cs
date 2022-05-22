﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EasyPro.Models;
using EasyPro.Utils;

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

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            
            ViewBag.suppliers = _context.DSuppliers.Select(s => new DSupplier
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

        [HttpPost]
        public JsonResult SuppliedProducts([FromBody] DSupplier supplier)
        {
            utilities.SetUpPrivileges(this);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate).Where(i => i.Sno == supplier.Sno.ToString()).ToList();
            return Json(intakes);
        }
    }
}
