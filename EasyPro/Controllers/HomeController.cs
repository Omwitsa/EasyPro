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

        public HomeController(MORINGAContext context, ILogger<HomeController> logger, INotyfService notyf)
        {
            _context = context;
            _logger = logger;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            //var intakes = _context.ProductIntake.Where(i => i.TransDate == DateTime.Today);
            //var seeds = intakes.Where(i => i.ProductType.ToLower().Equals("seeds"));
            //ViewBag.seeds = (seeds.Count() * 100) / intakes.Count();
            //var leaves = intakes.Where(i => i.ProductType.ToLower().Equals("leaves"));
            //ViewBag.leaves = (leaves.Count() * 100) / intakes.Count();
            //var powder = intakes.Where(i => i.ProductType.ToLower().Equals("powder"));
            //ViewBag.powder = (powder.Count() * 100) / intakes.Count();
            //var moringaSeeds = intakes.Where(i => i.ProductType.ToLower().Equals("moringa seeds"));
            //ViewBag.moringaSeeds = (moringaSeeds.Count() * 100) / intakes.Count();
            //var dryLeaves = intakes.Where(i => i.ProductType.ToLower().Equals("dry leaves"));
            //ViewBag.dryLeaves = (dryLeaves.Count() * 100) / intakes.Count();
            //var freshLeaves = intakes.Where(i => i.ProductType.ToLower().Equals("fresh leaves"));
            //ViewBag.freshLeaves = (freshLeaves.Count() * 100) / intakes.Count();

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

                login.Password = Decryptor.Decript_String(login.Password);
                var user = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => u.UserLoginIds.ToUpper().Equals(login.Username.ToUpper()) 
                    && u.Password.Equals(login.Password));
                if (user == null)
                {
                    _notyf.Error("Sorry, Invalid user credentials");
                    return View();
                }

                HttpContext.Session.SetString(StrValues.LoggedInUser, user.UserLoginIds);
                HttpContext.Session.SetString(StrValues.UserSacco, user.Branchcode);
                _notyf.Success("Logged in successfully");
                return RedirectToAction(nameof(Index));
            }
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
