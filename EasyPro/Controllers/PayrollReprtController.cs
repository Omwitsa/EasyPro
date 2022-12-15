using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class PayrollReprtController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public PayrollReprtController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public IActionResult TransportersIndex()
        {
            utilities.SetUpPrivileges(this);
            DateTime Now = DateTime.Today;
            DateTime startDate = new DateTime(Now.Year, Now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var SalesAnalysis = _context.DTransportersPayRolls.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.EndPeriod >= startDate && i.EndPeriod <= enDate)
            .OrderByDescending(s => s.Code.ToUpper()).ToList();
            return View(SalesAnalysis);
        }


        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            DateTime Now = DateTime.Today;
            DateTime startDate = new DateTime(Now.Year, Now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var SalesAnalysis = _context.DPayrolls.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && i.EndofPeriod >= startDate && i.EndofPeriod <= enDate)
            .OrderByDescending(s => s.Sno).ToList();
            return View(SalesAnalysis);
        }

        public IActionResult DefaultIndex()
        {
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
        public JsonResult Supplierpayroll(DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var supplierspayroll = _context.DPayrolls.Join(_context.DSuppliers,
                t => t.Sno.Trim().ToUpper(),
                i => i.Sno.Trim().ToUpper(),
                (t, i) => new
                {
                    t.EndofPeriod,
                    t.Sno,
                    i.Names,
                    t.Branch,
                    t.KgsSupplied,
                    t.Gpay,
                    t.Agrovet,
                    t.Tmshares,
                    t.Fsa,
                    t.Hshares,
                    t.Advance,
                    t.AI,
                    t.Others,
                    t.Tractor,
                    t.CLINICAL,
                    t.CurryForward,
                    t.Tdeductions,
                    t.Npay,
                    t.Bank,
                    t.AccountNumber,
                    t.Bbranch,
                    t.SaccoCode,
                }).Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.EndofPeriod == date2).ToList();

            return Json(supplierspayroll);
        }

        [HttpPost]
        public JsonResult SuppliedProducts(DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var transporterpayroll = _context.DTransportersPayRolls.Join(_context.DTransporters,
                t => t.Code.Trim().ToUpper(),
                i => i.TransCode.Trim().ToUpper(),
                (t, i) => new
                {
                    t.EndPeriod,
                    t.Code,
                    i.TransName,
                    t.Branch,
                    t.QntySup,
                    t.GrossPay,
                    t.Agrovet,
                    t.Tmshares,
                    t.Fsa,
                    t.Hshares,
                    t.Advance,
                    t.AI,
                    t.Others,
                    t.Tractor,
                    t.CLINICAL,
                    t.VARIANCE,
                    t.CurryForward,
                    t.Totaldeductions,
                    t.NetPay,
                    t.BankName,
                    t.AccNo,
                    t.BBranch,
                    t.SaccoCode,
                }).Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.EndPeriod == date2).ToList();

            return Json(transporterpayroll);
        }
    }
}
