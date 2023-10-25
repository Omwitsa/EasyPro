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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var bills = await _context.Bills.Where(b => b.SaccoCode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                bills = bills.Where(i => i.Branch == saccoBranch).ToList();
            return View(bills);
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
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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
                DrAccNo = product.ContraAccount,
                CrAccNo = vender.APGlAccount,
                Branch = saccoBranch
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
                DrAccNo = product.ContraAccount,
                CrAccNo = tax.GlAccount,
                Branch = saccoBranch
            });
            bill.SaccoCode = sacco;
            bill.Branch = saccoBranch;
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var refunds = await _context.Refunds.Where(r => r.SaccoCode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                refunds = refunds.Where(i => i.Branch == saccoBranch).ToList();
            return View(refunds);
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
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
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
                CrAccNo = product.ContraAccount,
                Branch = saccoBranch
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
                CrAccNo = product.ContraAccount,
                Branch= saccoBranch
            });

            refund.SaccoCode = sacco;
            refund.Branch = saccoBranch;
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
