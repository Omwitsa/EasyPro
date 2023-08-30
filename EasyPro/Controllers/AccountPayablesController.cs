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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View(await _context.Bills.ToListAsync());
        }

        public IActionResult CreateBill()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var venders = _context.Venders.Where(c => c.SaccoCode == sacco).OrderBy(m => m.Name).ToList();
            ViewBag.venders = new SelectList(venders, "Name", "Name");
            var products = _context.VProducts.Where(c => c.SaccoCode == sacco).OrderBy(m => m.Name).ToList();
            ViewBag.products = products;
            ViewBag.productNames = new SelectList(products, "Name", "Name");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBill(Bill bill)
        {
            bill.Ref = bill?.Ref ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            bill.SaccoCode = sacco;
            var product = _context.VProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(bill.Ref.ToUpper()) && p.SaccoCode == sacco);
            var vender = _context.Venders.FirstOrDefault(v => v.Name.ToUpper().Equals(bill.Vender.ToUpper()) && v.SaccoCode == sacco);
            var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product.VenderTax.ToUpper())
                    && t.SaccoCode == sacco && t.Type == "Purchases");
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)bill.NetAmount,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Bills",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = product.ARGlAccount,
                CrAccNo = vender.APGlAccount
            });

            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)bill.Tax,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Tax",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = product.ARGlAccount,
                CrAccNo = tax.GlAccount
            });
            _context.Bills.Add(bill);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetBills));
        }

        public async Task<IActionResult> GetRefunds()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View(await _context.Refunds.ToListAsync());
        }

        public IActionResult CreateRefund()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRefund(Refund refund)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            refund.SaccoCode = sacco;

            var product = _context.VProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(refund.Ref.ToUpper()) && p.SaccoCode == sacco);
            var vender = _context.Venders.FirstOrDefault(v => v.Name.ToUpper().Equals(refund.Vendor.ToUpper()) && v.SaccoCode == sacco);
            var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product.VenderTax.ToUpper())
                    && t.SaccoCode == sacco && t.Type == "Purchases");
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)refund.NetAmount,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Refunds",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = vender.APGlAccount,
                CrAccNo = product.ARGlAccount
            });

            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)refund.Tax,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Tax",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = tax.GlAccount,
                CrAccNo = product.ARGlAccount
            });

            _context.Refunds.Add(refund);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetRefunds));
        }

        [HttpGet]
        public JsonResult GetTaxRate(string product)
        {
            try
            {
                decimal? taxRate = 0;
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var product1 = _context.VProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(product.ToUpper()) && p.SaccoCode == sacco);
                if(product1 != null)
                {
                    var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product1.VenderTax.ToUpper())
                    && t.SaccoCode == sacco && t.Type == "Purchases");
                    if(tax != null)
                        taxRate = tax.Rate;
                }
                
                return Json(taxRate);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
