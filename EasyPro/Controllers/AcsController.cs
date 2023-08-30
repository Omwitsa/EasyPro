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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var filters = new string[] { "SNO", "MemberNo" };
            ViewBag.filters = new SelectList(filters);
            return View();
        }

        [HttpGet]
        public JsonResult GetLoanLiable(string membership, string no)
        {
            no = no ?? "";
            membership = membership ?? "";

            if (string.IsNullOrEmpty(membership) || string.IsNullOrEmpty(no))
            {
                _notyf.Error("Sorry, Kindly Provide membership");
                return Json("");
            }
            
            var sno = no;
            var memberNo = no;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            if (membership == "SNO")
            {
                var idNo = _context.DSuppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(no.ToUpper()) && s.Scode == sacco)?.IdNo ?? "";
                memberNo = _bosaDbContext.MEMBERS.FirstOrDefault(m => m.IDNo== idNo)?.MemberNo ?? "";
            }

            if (membership == "MemberNo")
            {
                var idNo = _bosaDbContext.MEMBERS.FirstOrDefault(s => s.MemberNo.ToUpper().Equals(no.ToUpper()))?.IDNo ?? "";
                sno = _context.DSuppliers.FirstOrDefault(m => m.IdNo == idNo)?.Sno ?? "";
            }

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
                Shares = 0,
                FlmdValue = 0,
                LiableAmount = averageAmount * 0.33M,
            });

            var sharesAmount = _bosaDbContext.CONTRIB.Where(m => m.MemberNo.ToUpper().Equals(memberNo.ToUpper())).Sum(m => m.Amount);
            acs.Add(new AcsVm
            {
                Name = "SACCO",
                LastMonthAmount = 0,
                Last2MonthAmount = 0,
                Last3MonthAmount = 0,
                AverageAmount = 0,
                Shares = sharesAmount,
                FlmdValue = 0,
                LiableAmount = sharesAmount * 0.33M
            });

            decimal? flmdAmount = 0;
            var fLMDs = _context.FLMD.Where(f => f.Sno == sno && f.SaccoCode == sacco);
            flmdAmount += fLMDs.Sum(f => f.ExoticCattleValue) + fLMDs.Sum(f => f.IndigenousCattleValue) + fLMDs.Sum(f => f.IndigenousChickenValue) 
                + fLMDs.Sum(f => f.SheepValue) + fLMDs.Sum(f => f.GoatsValue) + fLMDs.Sum(f => f.CamelsValue) + fLMDs.Sum(f => f.DonkeysValue) 
                + fLMDs.Sum(f => f.PigsValue) + fLMDs.Sum(f => f.BeeHivesValue);

            var fLMDCrops = _context.FLMDCrops.Where(f => f.Sno == sno && f.SaccoCode == sacco);
            flmdAmount += fLMDCrops.Sum(c => c.CashCropsValue) + fLMDCrops.Sum(c => c.ConsumerCropsValue) + fLMDCrops.Sum(c => c.VegetablesValue) 
                + fLMDCrops.Sum(c => c.AnimalFeedsValue);

            var lands = _context.FLMDLand.Where(l => l.Sno == sno && l.SaccoCode == sacco);
            flmdAmount += lands.Sum(l => l.PlotValue);

            acs.Add(new AcsVm
            {
                Name = "FLMD",
                LastMonthAmount = 0,
                Last2MonthAmount = 0,
                Last3MonthAmount = 0,
                AverageAmount = 0,
                Shares = 0,
                FlmdValue = flmdAmount,
                LiableAmount = flmdAmount * 0.33M
            });

            return Json(acs);
        }

    }
}
