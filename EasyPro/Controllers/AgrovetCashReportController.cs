﻿using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
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
    public class AgrovetCashReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AgrovetCashReportController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public async Task<IActionResult> Index()
        {
            DateTime Now = DateTime.Today;
            DateTime startDate = new DateTime(Now.Year, Now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var Salescheckoff = await _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.TDate >= startDate && i.TDate <= enDate && i.SNo.ToUpper().Equals("CASH")).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                Salescheckoff = Salescheckoff.Where(i => i.Branch == saccobranch).ToList();

            Salescheckoff = Salescheckoff.OrderByDescending(s => s.RId).ToList();
            return View(Salescheckoff);
        }

        private async Task SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var products = await _context.AgProducts
                .Where(s => s.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                products = products.Where(s => s.Branch == saccoBranch).ToList();
            ViewBag.products = new SelectList(products, "PCode", "PName");
        }

        [HttpPost]
        public async Task<JsonResult> Cashreport(DateTime startDate, DateTime endDate, string product, string sno)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper().Equals("CASH"))
                .OrderByDescending(i => i.TDate).ToList();

            return Json(Salescheckoff);
        }

        [HttpPost]
        public async Task<JsonResult> CashSummary(DateTime startDate, DateTime endDate, string product, string sno)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper().Equals("CASH"))
                .OrderByDescending(i => i.TDate).ToList();
            var groupedReports = Salescheckoff.GroupBy(i => i.TDate).ToList();
            var productName = "";
            if (!string.IsNullOrEmpty(product))
            {
                var products = await _context.AgProducts.Where(p => p.saccocode == sacco && p.PCode == product).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    products = products.Where(i => i.Branch == saccobranch).ToList();
                productName = products.FirstOrDefault()?.PName ?? "";
            }

            var summary = new List<dynamic>();
            groupedReports.ForEach(i =>
            {
                summary.Add(new
                {
                    date = i.Key,
                    product,
                    productName,
                    sno,
                    quantity = i.Sum(r => r.Qua),
                    amount = i.Sum(r => r.Amount)
                });
            });

            return Json(summary);
        }


        private async Task<List<AgReceipt>> GetAgReceipts(DateTime startDate, DateTime endDate, string product, string sno)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var receipts = await _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
               && i.TDate >= startDate && i.TDate <= endDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                receipts = receipts.Where(i => i.Branch == saccobranch).ToList();

            if (!string.IsNullOrEmpty(product))
                receipts = receipts.Where(r => r.PCode == product).ToList();
            if (!string.IsNullOrEmpty(sno))
                receipts = receipts.Where(r => r.SNo == sno).ToList();

            return receipts;
        }

    }
}
