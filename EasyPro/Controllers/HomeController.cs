using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public HomeController(MORINGAContext context, ILogger<HomeController> logger, INotyfService notyf)
        {
            _context = context;
            _logger = logger;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var intakes = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate && i.TransDate <= DateTime.Today);
            var products = _context.DPrices.Where(p => p.SaccoCode == sacco).Select(p => p.Products.ToUpper()).Distinct().ToList();
            var intakeStatistics = new List<PoductStatistics>();
            products.ForEach(p =>
            {
                var productIntakes = intakes.Where(i => i.ProductType.ToUpper().Equals(p));
                var rate = 0;
                if (intakes.Any())
                    rate = (productIntakes.Count() * 100) / intakes.Count();

                intakeStatistics.Add(new PoductStatistics
                {
                    Name = p,
                    Rate = rate
                });
            });

            ViewBag.intakeStatistics = intakeStatistics;
            var suppliers = _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper()));
            ViewBag.suppliers = suppliers.Count();

            var activeSupNos = _context.ProductIntake.Where(p => p.TransDate >= startDate && p.TransDate <= endDate && p.SaccoCode == sacco)
                .Select(p => p.Sno);
            var activeSuppliers = suppliers.Where(s => activeSupNos.Contains(s.Sno.ToString()));
            ViewBag.activeSuppliers = activeSuppliers.Count();

            var todatysIntake = _context.ProductIntake.Where(p => p.TransDate == DateTime.Today && p.SaccoCode == sacco);
            var todaySupNos = todatysIntake.Select(p => p.Sno);
            var failedSuppliers = suppliers.Where(s => !todaySupNos.Contains(s.Sno.ToString()));
            ViewBag.failedSuppliers = failedSuppliers.Count();

            var sales = _context.Dispatch.Where(d => d.Transdate == DateTime.Today && d.Dcode == sacco).Sum(d => d.Dispatchkgs);
            ViewBag.todaysales = sales;

            var newFarmers = suppliers.Where(s => s.Regdate >= startDate && s.Regdate <= endDate);
            ViewBag.newFarmers = newFarmers.Count();

            var lastMonthStartDate = startDate.AddMonths(-1);
            var lastMonthStartDateEndDate = lastMonthStartDate.AddMonths(1).AddDays(-1);

            var lastMonthActiveSupNos = _context.ProductIntake.Where(p => p.TransDate >= lastMonthStartDate && p.TransDate <= lastMonthStartDateEndDate 
            && p.SaccoCode == sacco).Select(p => p.Sno);
            var newSupNos = newFarmers.Select(s => s.Sno.ToString());
            var lastMonthDomant = activeSuppliers.Where(s => !lastMonthActiveSupNos.Contains(s.Sno.ToString()) && newSupNos.Contains(s.Sno.ToString()));
            ViewBag.lastMonthDomant = lastMonthDomant.Count();

            var transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper()));
            ViewBag.transporters = transporters.Count();

            ViewBag.grossItake = intakes.Sum(i => i.CR);
            ViewBag.advance = intakes.Where(i => i.ProductType.ToLower().Contains("advance")).Sum(i => i.DR);
            ViewBag.transport = intakes.Where(i => i.ProductType.ToLower().Contains("transport")).Sum(i => i.DR);
            ViewBag.agrovet = intakes.Where(i => i.ProductType.ToLower().Contains("agrovet")).Sum(i => i.DR);
            ViewBag.bonus = intakes.Where(i => i.ProductType.ToLower().Contains("bonus")).Sum(i => i.DR);
            ViewBag.shares = intakes.Where(i => i.ProductType.ToLower().Contains("shares")).Sum(i => i.DR);

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] LoginVm login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(login.Username))
                    {
                        _notyf.Error("Sorry, Kindly provide username");
                        return View();
                    }
                    if (string.IsNullOrEmpty(login.Password))
                    {
                        _notyf.Error("Sorry, Kindly provide password");
                        return View();
                    }

                    var user = await _context.UserAccounts
                        .FirstOrDefaultAsync(u => u.UserLoginIds.ToUpper().Equals(login.Username.ToUpper()));
                    if (user == null)
                    {
                        _notyf.Error("Sorry, Invalid user credentials");
                        return View();
                    }
                    user.Reset = user?.Reset ?? false;
                    if ((bool)user.Reset)
                    {
                        _notyf.Error("Sorry, Kindly wait while your password is being reset");
                        return View();
                    }
                    login.Password = Decryptor.Decript_String(login.Password);
                    if (!user.Password.Equals(login.Password))
                    {
                        _notyf.Error("Sorry, Invalid user credentials");
                        return View();
                    }

                    HttpContext.Session.SetString(StrValues.LoggedInUser, user.UserLoginIds);
                    HttpContext.Session.SetString(StrValues.UserSacco, user.Branchcode);
                    HttpContext.Session.SetString(StrValues.UserGroup, user.UserGroup);
                    HttpContext.Session.SetString(StrValues.Branch, user.Branch);
                    _notyf.Success("Logged in successfully");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _notyf.Error("Logged in fail.");
                }
            }
            _notyf.Error("Sorry, An error occurred");
            return View(login);
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind("Username,Password")] LoginVm login)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(login.Username))
                {
                    _notyf.Error("Sorry, Kindly provide username");
                    return View();
                }
                
                var user = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => u.UserLoginIds.ToUpper().Equals(login.Username.ToUpper()));
                if (user == null)
                {
                    _notyf.Error("Sorry, Invalid user credentials");
                    return View();
                }

                user.Reset = true;
                _context.SaveChanges();
                _notyf.Success("Request submitted successfully");
                return RedirectToAction(nameof(Login));
            }
            _notyf.Error("Sorry, Invalid user credentials");
            return View(login);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
