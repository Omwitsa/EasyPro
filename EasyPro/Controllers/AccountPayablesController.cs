using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var venders = _context.Venders.Where(c => c.SaccoCode == sacco).ToList();
            ViewBag.venders = new SelectList(venders, "Name", "Name");
            var products = _context.VProducts.Where(c => c.SaccoCode == sacco).ToList();
            ViewBag.products = products;
            ViewBag.productNames = new SelectList(products, "Name", "Name");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBill(Bill bill)
        {
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            bill.SaccoCode = sacco;
            _context.Bills.Add(bill);
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Bills",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco
            });
            _context.SaveChanges();
            return RedirectToAction(nameof(GetBills));
        }

        public async Task<IActionResult> GetRefunds()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.Refunds.ToListAsync());
        }

        public IActionResult CreateRefund()
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRefund(Refund refund)
        {
            utilities.SetUpPrivileges(this);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            refund.SaccoCode = sacco;
            _context.Refunds.Add(refund);
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Refunds",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco
            });
            _context.SaveChanges();
            return RedirectToAction(nameof(GetRefunds));
        }

        [HttpGet]
        public JsonResult GetTax(string product)
        {
            try
            {
                decimal? taxAmount = 0;
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var product1 = _context.VProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(product.ToUpper()) && p.SaccoCode == sacco);
                if(product1 != null)
                {
                    var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product1.VenderTax.ToUpper())
                    && t.SaccoCode == sacco && t.Type == "Purchases");
                    if(tax != null)
                    {
                        taxAmount = product1.Price * tax.Rate * (decimal?)0.01;
                    }
                }
                
                return Json(taxAmount);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
