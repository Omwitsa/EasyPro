using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public JsonResult PostTransactions([FromBody] List<Gltransaction> transactions)
        {
            _context.Gltransactions.AddRange(transactions);
            _context.SaveChanges();
            return Json("");
        }
    }
}
