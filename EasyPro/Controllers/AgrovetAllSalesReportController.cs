using AspNetCoreHero.ToastNotification.Abstractions;
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
    public class AgrovetAllSalesReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AgrovetAllSalesReportController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            DateTime Now = DateTime.Today;
            DateTime startDate = new DateTime(Now.Year, Now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var SalesAnalysis = await _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.TDate >= startDate && i.TDate <= enDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                SalesAnalysis = SalesAnalysis.Where(i => i.Branch == saccobranch).ToList();

            SalesAnalysis = SalesAnalysis.OrderByDescending(s => s.RId).ToList();
            return View(SalesAnalysis);
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
        public async Task<JsonResult> AllSalesreport(DateTime startDate, DateTime endDate, string product)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product);
            return Json(Salescheckoff.OrderByDescending(i => i.TDate).ToList());
        }

        [HttpPost]
        public async Task<JsonResult> AllSalesSummary(DateTime startDate, DateTime endDate, string product)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product);
            var groupedReports = Salescheckoff.OrderByDescending(i => i.TDate).ToList().GroupBy(i => i.TDate).ToList();
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
                    quantity = i.Sum(r => r.Qua),
                    amount = i.Sum(r => r.Amount)
                });
            });

            return Json(summary);
        }
        [HttpPost]
        public async Task<JsonResult> allSalesSummaryDetailed(DateTime startDate, DateTime endDate, string product)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product);
            var groupedReports = Salescheckoff.OrderByDescending(i => i.TDate).ToList().GroupBy(i => i.TDate).ToList();

            var summary = new List<dynamic>();
            groupedReports.ForEach(i =>
            {
                summary.Add(new
                {
                    date = i.Key,
                    Checkoff = i.Where(n=>n.SNo != "cash").Sum(m=>m.Amount),
                    cash = i.Where(n => n.SNo == "cash").Sum(m => m.Amount),
                    Total = i.Sum(m=>m.Amount)
                });
            });

            return Json(summary);
        }
        private async Task<List<AgReceipt>> GetAgReceipts(DateTime startDate, DateTime endDate, string product)
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
            
            return receipts;
        }

    }
}
