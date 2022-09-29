using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class DSharesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DSharesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DShares
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.DShares.ToListAsync());
        }

        // GET: DShares/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dShare = await _context.DShares
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dShare == null)
            {
                return NotFound();
            }

            return View(dShare);
        }

        // GET: DShares/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var suppliers = _context.DSuppliers
                .Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.suppliers = suppliers;
            var codes = _context.DDcodes.Where(a => a.Dcode == sacco).ToList();
            ViewBag.codes = new SelectList(codes, "Description", "Description");
            var modes = new string[] { "Cash", "Checkoff", "Mpesa" };
            ViewBag.modes = new SelectList(modes);
            return View();
        }

        // POST: DShares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,Bal,IdNo,Code,Name,Sex,Loc,Type,TransDate,Pmode,Cash,Period,Amnt,AuditId,AuditDateTime,Shares,Regdate,Mno,Amount,Premium,Spu")] DShare dShare)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            dShare.Period = DateTime.Today.Month.ToString();
            dShare.TransDate = DateTime.Today;
            dShare.AuditDateTime = DateTime.Now;
            dShare.SaccoCode = sacco;
            dShare.AuditId = auditId;

            var dDcode = _context.DDcodes.FirstOrDefault(c => c.Description == dShare.Type && c.Dcode == sacco);
            if(dDcode == null)
            {
                _notyf.Error("Sorry, Deduction not set");
                return View(dShare);
            }
            if (string.IsNullOrEmpty(dDcode.Contraacc))
            {
                _notyf.Error("Sorry, Contra acc not set");
                return View(dShare);
            }
            if (string.IsNullOrEmpty(dDcode.Dedaccno))
            {
                _notyf.Error("Sorry, Deduction acc not set");
                return View(dShare);
            }

            if (ModelState.IsValid)
            {
                _context.Add(dShare);
                var gltransaction = new Gltransaction
                {
                    AuditId = auditId,
                    TransDate = DateTime.Today,
                    Amount = dShare.Amount,
                    AuditTime = DateTime.Now,
                    Source = "Shares",
                    TransDescript = "Shares",
                    Transactionno = $"{auditId}{DateTime.Now}",
                    SaccoCode = sacco,
                    DrAccNo = dDcode.Dedaccno,
                    CrAccNo = dDcode.Contraacc,
                };
                if(dShare.Amount < 0)
                {
                    gltransaction.Amount = -dShare.Amount;
                    gltransaction.DrAccNo  = dDcode.Contraacc;
                    gltransaction.CrAccNo = dDcode.Dedaccno;   
                }
                _context.Gltransactions.Add(gltransaction);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dShare);
        }

        // GET: DShares/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dShare = await _context.DShares.FindAsync(id);
            if (dShare == null)
            {
                return NotFound();
            }
            return View(dShare);
        }

        // POST: DShares/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,Bal,IdNo,Code,Name,Sex,Loc,Type,TransDate,Pmode,Cash,Period,Amnt,AuditId,AuditDateTime,Shares,Regdate,Mno,Amount,Premium,Spu")] DShare dShare)
        {
            utilities.SetUpPrivileges(this);
            if (id != dShare.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dShare);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DShareExists(dShare.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dShare);
        }

        // GET: DShares/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dShare = await _context.DShares
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dShare == null)
            {
                return NotFound();
            }

            return View(dShare);
        }

        // POST: DShares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dShare = await _context.DShares.FindAsync(id);
            _context.DShares.Remove(dShare);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DShareExists(long id)
        {
            return _context.DShares.Any(e => e.Id == id);
        }
    }
}
