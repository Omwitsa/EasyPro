using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class AccountPayablesController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public AccountPayablesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        public async Task<IActionResult> GetBills()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.Bills.ToListAsync());
        }

        public IActionResult CreateBill()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBill(Bill bill)
        {
            utilities.SetUpPrivileges(this);
            return View(bill);
        }

        public async Task<IActionResult> GetRefunds()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.Refunds.ToListAsync());
        }

        public IActionResult CreateRefund()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRefund(Refund refund)
        {
            utilities.SetUpPrivileges(this);
            return View(refund);
        }
    }
}
