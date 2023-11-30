using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Bibliography;
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
                .Where(s => s.saccocode.ToUpper().Equals(sacco.ToUpper()) && s.Branch == saccoBranch).ToListAsync();
            //var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            //if (user.AccessLevel == AccessLevel.Branch)
            //    products = products.Where(s => s.Branch == saccoBranch).ToList();
            ViewBag.products = new SelectList(products, "PCode", "PName");
        }

        [HttpPost]
        public async Task<JsonResult> Cashreport(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno, salestype);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper().Equals("CASH") && i.RNo == sno)
                .OrderByDescending(i => i.TDate).ToList();
            return Json(Salescheckoff);
        }

        [HttpPost]
        public async Task<JsonResult> CashSummary(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno, salestype);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper().Equals("CASH") && i.RNo == sno)
                .OrderByDescending(i => i.TDate).ToList();
            var groupedReports = Salescheckoff.GroupBy(i => i.TDate).ToList();

            var products = _context.AgProducts.Where(p => p.saccocode == sacco).ToList();
            if (!string.IsNullOrEmpty(product))
            {
               var productz = products.Where(p => p.PCode == product && p.Branch == saccobranch).ToList();
                products = productz;
            }

            var summary = new List<dynamic>();
            groupedReports.ForEach(i =>
            {
                var productName = i.FirstOrDefault().Remarks;
                if (!string.IsNullOrEmpty(product))
                {
                    productName = products.FirstOrDefault().PName;
                }

                 var correctquantity = i.Where(b => b.Amount > 0).Sum(r => r.Qua) - i.Where(b => b.Amount < 0).Sum(r => r.Qua);
                summary.Add(new
                {
                    date = i.Key,
                    product = i.FirstOrDefault().PCode,
                    productName,
                    sno,
                    quantity = correctquantity,
                    amount = i.Sum(r => r.Amount)
                });
            });
            

            return Json(summary);
        }


        private async Task<List<AgReceipt>> GetAgReceipts(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
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
                receipts = receipts.Where(r => r.RNo == sno).ToList();
            if (salestype == 1)
                receipts = receipts.Where(m => m.Amount > 0).ToList();
            if (salestype == 2)
                receipts = receipts.Where(m => m.Amount < 0).ToList();

            return receipts;
        }

    }
}
