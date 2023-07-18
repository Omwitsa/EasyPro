using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EasyPro.ViewModels.AccountingVm;
using System.Collections.Generic;
using EasyPro.ViewModels;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using Syncfusion.EJ2.Linq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models.BosaModels;

namespace EasyPro.Controllers
{
    public class AcsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly BosaDbContext _bosaDbContext;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AcsController(MORINGAContext context, INotyfService notyf, BosaDbContext bosaDbContext)
        {
            _context = context;
            _bosaDbContext = bosaDbContext;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpGet]
        public JsonResult GetLoanLiable(string sno)
        {
            sno = sno ?? "";
            utilities.SetUpPrivileges(this);

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);
            var last2MonthStart = monthStart.AddMonths(-2);
            var last2MonthEnd = lastMonthStart.AddDays(-1);
            var last3MonthStart = monthStart.AddMonths(-3);
            var last3MonthEnd = last2MonthStart.AddDays(-1);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            IQueryable<ProductIntake> intakes = _context.ProductIntake.Where(i => i.Sno == sno && i.SaccoCode == sacco);
            var product = intakes.FirstOrDefault()?.ProductType ?? "";
            var lastMonthQuantity = intakes.Where(i => i.TransDate >= lastMonthStart && i.TransDate <= lastMonthEnd).Sum(i => i.Qsupplied);
            var last2MonthQuantity = intakes.Where(i => i.TransDate >= last2MonthStart && i.TransDate <= last2MonthEnd).Sum(i => i.Qsupplied);
            var last3MonthQuantity = intakes.Where(i => i.TransDate >= last3MonthStart && i.TransDate <= last3MonthEnd).Sum(i => i.Qsupplied);

            var price = _context.DPrices.FirstOrDefault(p => p.Products == product)?.Price ?? 0;
            var lastMonthAmount = lastMonthQuantity * price;
            var last2MonthAmount = last2MonthQuantity * price;
            var last3MonthAmount = last3MonthQuantity * price;
            var averageAmount = (lastMonthAmount + last2MonthAmount + last3MonthAmount) / 3;
            var acs = new List<AcsVm>();
            acs.Add(new AcsVm
            {
                Name = "PO",
                LastMonthAmount = lastMonthAmount,
                Last2MonthAmount = last2MonthAmount,
                Last3MonthAmount = last3MonthAmount,
                AverageAmount = averageAmount,
                LiableAmount = averageAmount * 0.33M
            });

           var val = _bosaDbContext.MEMBERS.FirstOrDefault(m => m.IDNo == "198");

            return Json(acs);
        }

    }
}
