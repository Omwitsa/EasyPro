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

        public class Farmersdata
        {
            public string xValue;
            public double yValue;
            public double Score;
            public string text;
        }
        public class ColumnChartData
        {
            public string x;
            public double yValue;
        }

        private void SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

        }
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            SetIntakeInitialValues();

            IQueryable<DSupplier> suppliers = _context.DSuppliers;
            var totalsuppliers = suppliers.Where(m => m.Scode == sacco);
            double x = totalsuppliers.Where(k =>k.Type.ToLower().Equals("male")).Count();
            double y = totalsuppliers.Count();
            double percentage = Math.Round(((x / y) * 100), 0);

            double x1 = totalsuppliers.Where(k => k.Type.ToLower().Equals("female")).Count();
            double percentages = Math.Round(((x1 / y) * 100), 0);
            ViewBag.males = percentage;
            ViewBag.females = percentages;

            List<Farmersdata> chartData = new List<Farmersdata>
            {

                new Farmersdata { xValue = "Male", yValue = ViewBag.males, text = "Male:" + ViewBag.males + "%" },
                new Farmersdata { xValue = "Female", yValue = ViewBag.females, text = "Female:" + ViewBag.females + "%" }
            };

            ViewBag.dataSource = chartData;


            var startDate1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate1 = startDate1.AddMonths(1).AddDays(-1);

            IQueryable<ProductIntake> activesup = _context.ProductIntake;

            var listactivesuppliers = activesup.Where(m => m.SaccoCode == sacco && m.Description == "Intake" && m.TransDate>= startDate1
            && m.TransDate<= endDate1 ).ToList();


            double x2 = listactivesuppliers.Select(b=>b.Sno).Distinct().Count();

            ViewBag.suppliers = y;
            ViewBag.activeSuppliers = x2;
            ViewBag.failedSuppliers = y-x2;

            double percentage2 = Math.Round(((x2 / y) * 100), 0);

            double x3 = y-x2;
            double percentages3 = Math.Round(((x3 / y) * 100), 0);
            ViewBag.males = percentage2;
            ViewBag.females = percentages3;

            List<Farmersdata> chartData1 = new List<Farmersdata>
            {

                new Farmersdata { xValue = "Active", yValue = ViewBag.males, text = "Active:" + ViewBag.males + "%" },
                new Farmersdata { xValue = "InActive", yValue = ViewBag.females, text = "InActive:" + ViewBag.females + "%" }
            };

            ViewBag.dataSource1 = chartData1;

            IQueryable<DShare> shares = _context.DShares;

            List<object> data = new List<object>();
            List<ColumnChartData> chartDatas4 = new List<ColumnChartData>();
            var sharespersacco = shares.Where(m => m.SaccoCode == sacco).ToList();
            decimal totalshares = sharespersacco.ToList().Sum(a => a.Amount);
            var sharesdedu = sharespersacco.Select(n=>n.Type.ToUpper()).Distinct().ToList();

            sharesdedu.ForEach(c =>
            {
                decimal sharesamount = sharespersacco.Where(x => x.Type.ToUpper().Equals(c.ToUpper())).ToList().Sum(a=>a.Amount);
                decimal percentage4 = Math.Round(((sharesamount / totalshares) * 100), 0);

                chartDatas4.Add(new ColumnChartData
                {
                    x= c.ToUpper(), yValue= (double)percentage4,
                });
            });

            ViewBag.dataSources3 = chartDatas4;

            //var intakes = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate && i.TransDate <= DateTime.Today);
            //var products = _context.DPrices.Where(p => p.SaccoCode == sacco).Select(p => p.Products.ToUpper()).Distinct().ToList();
            var intakeStatistics = new List<PoductStatistics>();
            //products.ForEach(p =>
            //{
            //    var productIntakes = intakes.Where(i => i.ProductType.ToUpper().Equals(p));
            //    var rate = 0;
            //    if (intakes.Any())
            //        rate = (productIntakes.Count() * 100) / intakes.Count();

            //    intakeStatistics.Add(new PoductStatistics
            //    {
            //        Name = p,
            //        Rate = rate
            //    });
            //});

            ViewBag.intakeStatistics = intakeStatistics;
            //var suppliers = _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            //ViewBag.suppliers = suppliers.Count();
           
            //var activeSupNos = _context.ProductIntake.Where(p => p.TransDate >= startDate && p.TransDate <= endDate && p.SaccoCode == sacco)
            //    .Select(p => p.Sno).ToList();
            //var activeSuppliers = suppliers.Where(s => activeSupNos.Contains(s.Sno.ToString())).ToList();
            //ViewBag.activeSuppliers = activeSuppliers.Count;


            //var todatysIntake = _context.ProductIntake.Where(p => p.TransDate == DateTime.Today && p.SaccoCode == sacco);
            //var todaySupNos = todatysIntake.Select(p => p.Sno);
            //var failedSuppliers = suppliers.Where(s => !todaySupNos.Contains(s.Sno.ToString()));
            //ViewBag.failedSuppliers = failedSuppliers.Count();


            //var sales = _context.Dispatch.Where(d => d.Transdate == DateTime.Today && d.Dcode == sacco).Sum(d => d.Dispatchkgs);
            //ViewBag.todaysales = sales;
            ViewBag.todaysales = 0;

            var newFarmers = suppliers.Where(s =>s.Scode== sacco && s.Regdate >= startDate1 && s.Regdate <= endDate1);
            ViewBag.newFarmers = newFarmers.Count();
            //ViewBag.newFarmers = 0;

            var lastMonthStartDate = startDate.AddMonths(-1);
            var lastMonthStartDateEndDate = lastMonthStartDate.AddMonths(1).AddDays(-1);

            //var lastMonthActiveSupNos = _context.ProductIntake.Where(p => p.TransDate >= lastMonthStartDate && p.TransDate <= lastMonthStartDateEndDate 
            //&& p.SaccoCode == sacco).Select(p => p.Sno).ToList();
            //var newSupNos = newFarmers.Select(s => s.Sno.ToString()).ToList();
            //var lastMonthDomant = activeSuppliers.Where(s => !lastMonthActiveSupNos.Contains(s.Sno.ToString()) && newSupNos.Contains(s.Sno.ToString()));
            //ViewBag.lastMonthDomant = lastMonthDomant.Count();
            ViewBag.lastMonthDomant = 0;

            var transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.transporters = transporters.Count();
            //ViewBag.transporters = 0;

            //ViewBag.grossItake = intakes.Sum(i => i.CR);
            //ViewBag.advance = intakes.Where(i => i.ProductType.ToLower().Contains("advance")).Sum(i => i.DR);
            //ViewBag.transport = intakes.Where(i => i.ProductType.ToLower().Contains("transport")).Sum(i => i.DR);
            //ViewBag.agrovet = intakes.Where(i => i.ProductType.ToLower().Contains("agrovet")).Sum(i => i.DR);
            //ViewBag.bonus = intakes.Where(i => i.ProductType.ToLower().Contains("bonus")).Sum(i => i.DR);
            //ViewBag.shares = intakes.Where(i => i.ProductType.ToLower().Contains("shares")).Sum(i => i.DR);

            ViewBag.grossItake = 0;
            ViewBag.advance = 0;
            ViewBag.transport = 0;
            ViewBag.agrovet = 0;
            ViewBag.bonus = 0;
            ViewBag.shares = 0;

            
            return View();
        }

        public IActionResult NewUI()
        {
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
                    //_notyf.Success("Logged in successfully");
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
        
        //[HttpGet]
        //public ActionResult GetSalesDatas()
        //{
        //    utilities.SetUpPrivileges(this);
        //    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";

        //    List<object> data = new List<object>();
        //    var labels = _context.d_DCodesQuantity.Select(n => n.Month).ToList();
        //    data.Add(labels);

        //    var label = _context.d_DCodesQuantity.Select(k => k.TotalSales).ToList();
        //    data.Add(label);

        //    var list = JsonConvert.SerializeObject(data,
        //    Formatting.None,
        //    new JsonSerializerSettings()
        //    {
        //        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        //    });

        //    return Content(list, "application/json");
        //}

        [HttpGet]
        public JsonResult GetSalesData()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";

            List<object> data = new List<object>();
            List<string> labels = _context.d_DCodesQuantity.Select(n => n.Month).ToList();
            data.Add(labels);

            List<decimal?> label = _context.d_DCodesQuantity.Select(k => k.TotalSales).ToList();
            data.Add(label);

            return Json(data);
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
