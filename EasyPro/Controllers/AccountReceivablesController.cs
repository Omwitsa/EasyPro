using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(CInvoice invoice)
        {
            utilities.SetUpPrivileges(this);
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCreditNote(CreditNote note)
        {
            utilities.SetUpPrivileges(this);
            _context.CreditNotes.Add(note);
            _context.SaveChanges();
            return RedirectToAction(nameof(GetCreditNotes));
        }
    }
}
