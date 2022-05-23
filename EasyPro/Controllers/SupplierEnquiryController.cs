﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.ViewModels.EnquiryVM;

namespace EasyPro.Controllers
{
    public class SupplierEnquiryController : Controller
    {
        private readonly MORINGAContext _context;

        public SupplierEnquiryController(MORINGAContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
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
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            //var intakes = _context.ProductIntake.Where(i => i.Sno == supplier.Sno).ToList();
            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate).Where(i => i.Sno == supplier.Sno && i.TransDate>=startDate && i.TransDate<=enDate).ToList();
            return Json(intakes);
        }
    }
}
