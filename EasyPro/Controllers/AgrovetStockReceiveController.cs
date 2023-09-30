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
    public class AgrovetStockReceiveController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AgrovetStockReceiveController(MORINGAContext context, INotyfService notyf)
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
            var agProducts = await _context.AgProducts4s.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.AuditDate >= startDate && i.AuditDate <= enDate)
            .OrderByDescending(s => s.AuditDate).ToListAsync();

            ViewBag.User = loggedInUser;
            ViewBag.Sacco = sacco;

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                agProducts = agProducts.Where(s => s.Branch == saccobranch).ToList();
            return View(agProducts);
        }

        public IActionResult DefaultIndex()
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
            var agProducts = _context.AgProducts4s.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.AuditDate >= startDate && i.AuditDate <= enDate)
            .OrderByDescending(s => s.AuditDate).ToList();
            return View(agProducts);
        }


        [HttpPost]
        public JsonResult SuppliedProducts(DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var agProductsReceive = _context.AgProducts4s.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.DateEntered >= date1 && i.DateEntered <= date2).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                agProductsReceive = agProductsReceive.Where(i => i.Branch == saccobranch).ToList();
            //if (!string.IsNullOrEmpty(producttype))
            //    intakes = intakes.Where(i => i.ProductType.ToUpper().Equals(producttype.ToUpper())).ToList();

            agProductsReceive = agProductsReceive.OrderByDescending(i => i.DateEntered).ToList();
            return Json(agProductsReceive);
        }
    }
}
