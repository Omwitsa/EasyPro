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
            utilities.SetUpPrivileges(this);
            return View(await _context.CInvoices.ToListAsync());
        }

        public IActionResult CreateInvoice()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var customers = _context.Customers.Where(c => c.SaccoCode == sacco).ToList();
            ViewBag.customers = new SelectList(customers, "Name", "Name");
            var products = _context.CProducts.Where(c => c.SaccoCode == sacco).ToList();
            ViewBag.products = products;
            ViewBag.productNames = new SelectList(products, "Name", "Name");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(CInvoice invoice)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            invoice.SaccoCode = sacco;
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
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
            _context.CInvoices.Add(invoice);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetInvoices));
        }

        public async Task<IActionResult> GetCreditNotes()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.CreditNotes.ToListAsync());
        }

        public IActionResult CreateCreditNote()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var customers = _context.Customers.Where(c => c.SaccoCode == sacco).ToList();
            ViewBag.customers = new SelectList(customers, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCreditNote(CreditNote note)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            note.SaccoCode = sacco;
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
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
            _context.CreditNotes.Add(note);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetCreditNotes));
        }
    }
}
