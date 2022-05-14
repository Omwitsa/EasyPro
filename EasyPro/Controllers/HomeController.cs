using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
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
                    return View(login);
                }
                if (string.IsNullOrEmpty(login.Password))
                {
                    _notyf.Error("Sorry, Kindly provide password");
                    return View(login);
                }

                login.Password = Decryptor.Decript_String(login.Password);
                var isValidUser = await _context.UserAccounts
                    .AnyAsync(u => u.UserLoginIds.ToUpper().Equals(login.Username.ToUpper()) 
                    && u.Password.Equals(login.Password));
                if (!isValidUser)
                {
                    _notyf.Error("Sorry, Invalid user credentials");
                    return View(login);
                }

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
