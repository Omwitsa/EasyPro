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
            var intakes = _context.ProductIntake.Where(i => i.TransDate == DateTime.Today);
            var seedsRate = 0;
            var leavesRate = 0;
            var powderRate = 0;
            var moringaSeedsRate = 0;
            var dryLeavesRate = 0;
            var freshLeavesRate = 0;
            if(intakes.Count() > 0)
            {
                var seeds = intakes.Where(i => i.ProductType.ToLower().Equals("seeds"));
                seedsRate = (seeds.Count() * 100) / intakes.Count();
                var leaves = intakes.Where(i => i.ProductType.ToLower().Equals("leaves"));
                leavesRate = (leaves.Count() * 100) / intakes.Count();
                var powder = intakes.Where(i => i.ProductType.ToLower().Equals("powder"));
                powderRate = (powder.Count() * 100) / intakes.Count();
                var moringaSeeds = intakes.Where(i => i.ProductType.ToLower().Equals("moringa seeds"));
                moringaSeedsRate = (moringaSeeds.Count() * 100) / intakes.Count();
                var dryLeaves = intakes.Where(i => i.ProductType.ToLower().Equals("dry leaves"));
                dryLeavesRate = (dryLeaves.Count() * 100) / intakes.Count();
                var freshLeaves = intakes.Where(i => i.ProductType.ToLower().Equals("fresh leaves"));
                freshLeavesRate = (freshLeaves.Count() * 100) / intakes.Count();
            }

            ViewBag.seeds = seedsRate;
            ViewBag.leaves = leavesRate;
            ViewBag.powder = powderRate;
            ViewBag.moringaSeeds = moringaSeedsRate;
            ViewBag.dryLeaves = dryLeavesRate;
            ViewBag.freshLeaves = freshLeavesRate;

            //ViewBag.privileges = JsonConvert.SerializeObject(selectedSettings);

            ViewBag.prices = _context.DPrices.Where(p => p.Edate >= DateTime.Today).ToList();
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
                    _notyf.Success("Logged in successfully");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    
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
