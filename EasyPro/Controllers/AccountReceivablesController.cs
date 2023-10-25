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
    public class AccountReceivablesController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public AccountReceivablesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        public async Task<IActionResult> GetInvoices()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var invoices = await _context.CInvoices.Where(i => i.SaccoCode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                invoices = invoices.Where(i => i.Branch == saccoBranch).ToList();
            return View(invoices);
        }

        public IActionResult CreateInvoice()
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
            var customers = _context.Customers.Where(c => c.SaccoCode == sacco).OrderBy(m => m.Name).ToList();
            ViewBag.customers = new SelectList(customers, "Name", "Name");
            var products = _context.CProducts.Where(c => c.SaccoCode == sacco).OrderBy(m => m.Name).ToList();
            ViewBag.products = products;
            ViewBag.productNames = new SelectList(products, "Name", "Name");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(CInvoice invoice)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var product = _context.CProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(invoice.Ref.ToUpper()) && p.SaccoCode == sacco);
            var customer = _context.Customers.FirstOrDefault(v => v.Name.ToUpper().Equals(invoice.Customer.ToUpper()) && v.SaccoCode == sacco);
            var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product.CustomerTax.ToUpper())
                     && t.SaccoCode == sacco && t.Type == "Sales");
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)invoice.NetAmount,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Invoice",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = customer.ARGlAccount,
                CrAccNo = product.ContraAccount,
                Branch = saccoBranch
            });

            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)invoice.Tax,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Tax",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                Branch = saccoBranch,
                DrAccNo = tax.GlAccount,
                CrAccNo = product.ContraAccount
            });

            invoice.SaccoCode = sacco;
            invoice.Branch = saccoBranch;
            _context.CInvoices.Add(invoice);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetInvoices));
        }

        public async Task<IActionResult> GetCreditNotes()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var creditNotes = await _context.CreditNotes.Where(n => n.SaccoCode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                creditNotes = creditNotes.Where(i => i.Branch == saccoBranch).ToList();
            return View(creditNotes);
        }

        public IActionResult CreateCreditNote()
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
        public async Task<IActionResult> CreateCreditNote(CreditNote note)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var product = _context.CProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(note.Ref.ToUpper()) && p.SaccoCode == sacco);
            var customer = _context.Customers.FirstOrDefault(v => v.Name.ToUpper().Equals(note.Customer.ToUpper()) && v.SaccoCode == sacco);
            var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product.CustomerTax.ToUpper())
                     && t.SaccoCode == sacco && t.Type == "Sales");
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)note.NetAmount,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Credit Note",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                DrAccNo = product.ContraAccount,
                Branch = saccoBranch,
                CrAccNo = customer.ARGlAccount
            });

            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = DateTime.Today,
                Amount = (decimal)note.NetAmount,
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Credit Note",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                Branch = saccoBranch,
                DrAccNo = product.ContraAccount,
                CrAccNo = tax.GlAccount
            });

            note.SaccoCode = sacco;
            note.Branch = saccoBranch;
            _context.CreditNotes.Add(note);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetCreditNotes));
        }

        [HttpGet]
        public JsonResult GetTaxRate(string product)
        {
            try
            {
                decimal? taxRate = 0;
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var product1 = _context.CProducts.FirstOrDefault(p => p.Name.ToUpper().Equals(product.ToUpper()) && p.SaccoCode == sacco);
                if (product1 != null)
                {
                    var tax = _context.Taxes.FirstOrDefault(t => t.Name.ToUpper().Equals(product1.CustomerTax.ToUpper())
                    && t.SaccoCode == sacco && t.Type == "Sales");
                    if (tax != null)
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
