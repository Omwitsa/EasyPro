using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class AccountingController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AccountingController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public IActionResult JournalPosting()
        {
            utilities.SetUpPrivileges(this);
            var glAccounts = _context.Glsetups.ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

            return View();
        }

        /*
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JournalPosting([Bind("Id,LocalId,TransDate,Amount,DrAccNo,CrAccNo,DocumentNo,Source,TransDescript,AuditTime,AuditId,Cash,DocPosted,ChequeNo,Dregard,TimeTrans,Transactionno,Module,Pmode,Refid,Recon,ReconId,Run")] Gltransaction transaction)
        {

        }
         */

        [HttpPost]
        public JsonResult JournalPosting([FromBody] List<Gltransaction> transactions)
        {
            try
            {
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                transactions.ForEach(t =>
                {
                    t.AuditId = loggedInUser;
                    t.TransDate = DateTime.Today;
                    t.AuditTime = DateTime.Now;
                    t.Source = "";
                    t.TransDescript = t?.TransDescript ?? "";
                    t.Transactionno = $"{loggedInUser}{DateTime.Now}";
                });
                _context.Gltransactions.AddRange(transactions);
                _context.SaveChanges();
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
