using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace EasyPro.Controllers
{
    public class DispatchBalancingController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public DispatchBalancingController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var date = DateTime.Now;
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var dispatches = _context.DispatchBalancing
                .Where(i => i.Saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Date >= startDate && i.Date <= endDate).OrderByDescending(s => s.Date).ToList();
            return View(dispatches);
        }

        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpGet]
        public JsonResult GetSuppliedItems(DateTime? date)
        {
            try
            {
                date = date ?? DateTime.Now;
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var intakes = _context.ProductIntake.Where(s => s.TransDate== date && s.SaccoCode == sacco).ToList();

                var dispatch = _context.DispatchBalancing.FirstOrDefault(d => d.Saccocode == sacco && d.Date == date);
                dispatch = dispatch == null ? new DispatchBalancing() : dispatch;
                var balancing = new DispatchBalancing
                {
                    Intake = intakes.Sum(i => i.Qsupplied),
                    Dispatch = dispatch.Dispatch,
                    CF = dispatch.CF,
                    BF = dispatch.BF,
                    Actuals = dispatch.Actuals,
                    Spillage = dispatch.Spillage,
                    Rejects = dispatch.Rejects,
                    Varriance = dispatch.Varriance,
                    Saccocode = sacco
                };
                return Json(balancing);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        [HttpPost]//editVariance
        public JsonResult SaveVariance([FromBody] DispatchBalancing balancing)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (balancing.Actuals == null)
                {
                    _notyf.Error("Sorry, Kindly provide actual Kgs");
                    return Json("");
                }
               
                var checkexist = _context.DispatchBalancing.Any(s => s.Date == balancing.Date && s.Saccocode == sacco);
                if (checkexist)
                {
                    _notyf.Error("Sorry, Variance already saved");
                    return Json("");
                }
                if(balancing.Dispatch > balancing.Actuals)
                {
                    _notyf.Error("Sorry, Dispatch cannot exceed actuals");
                    return Json("");
                }

                balancing.Saccocode = sacco;
                _context.DispatchBalancing.Add(balancing);
                _context.SaveChanges();
                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
