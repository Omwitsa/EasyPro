using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Repository;
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
    public class DrawnstockReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly IReportProvider _reportProvider;
        readonly IReporting _IReporting;
        private Utilities utilities;

        public DrawnstockReportController(IReporting iReporting, IReportProvider reportProvider,
            MORINGAContext context)
        {
            _IReporting = iReporting;
            _reportProvider = reportProvider;
            utilities = new Utilities(context);
            _context = context;
        }
        public async Task<IActionResult> IndexAsync()
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

            ViewBag.User = loggedInUser;
            ViewBag.Sacco = sacco;
            
            return View(await _context.Drawnstocks
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.auditdatetime >= startDate && i.auditdatetime <= enDate)
                .OrderByDescending(s => s.auditdatetime).ToListAsync());
        }
        [HttpPost]
        public JsonResult SuppliedProducts(DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var dispatch = _context.Drawnstocks.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Date >= date1 && i.Date <= date2).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                dispatch = dispatch.Where(i => i.BranchF == saccobranch || i.Branch == saccobranch).ToList();
            //if (!string.IsNullOrEmpty(producttype))
            //    intakes = intakes.Where(i => i.ProductType.ToUpper().Equals(producttype.ToUpper())).ToList();

            dispatch = dispatch.OrderByDescending(i => i.Date).ToList();
            return Json(dispatch);
        }

    }
}
