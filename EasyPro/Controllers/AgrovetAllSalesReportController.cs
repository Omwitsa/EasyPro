using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<JsonResult> SuppliedProducts(DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var SalesAnalysis = await _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate >= date1 && i.TDate <= date2).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                SalesAnalysis = SalesAnalysis.Where(i => i.Branch == saccobranch).ToList();
            
            SalesAnalysis = SalesAnalysis.OrderByDescending(i => i.TDate).ToList();
            return Json(SalesAnalysis);
        }
    }
}
