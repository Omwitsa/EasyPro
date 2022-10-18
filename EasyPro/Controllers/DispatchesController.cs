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
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class DispatchesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public DispatchesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Dispatches
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.Dispatch
                .Where(i => i.Dcode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(s=>s.Transdate).ToListAsync());
        }

        // GET: Dispatches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dispatch = await _context.Dispatch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispatch == null)
            {
                return NotFound();
            }

            return View(dispatch);
        }

        // GET: Dispatches/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var dScode = sacco; 

            var debtorsnames = _context.DDebtors.Where(a => a.Dcode == dScode).Select(b => b.Dname).ToList();
            ViewBag.debtorsnames = new SelectList(debtorsnames);
        }

        // POST: Dispatches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dcode,DName,Transdate,Dispatchkgs,TIntake,auditid")] Dispatch dispatch)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var locations = _context.Dispatch.Any(i => i.DName == dispatch.DName && i.Dcode == sacco && i.Transdate== dispatch.Transdate);
            if (locations)
            {
                _notyf.Error("Sorry, The Dispatch to Debtor Name already exist");
                GetInitialValues();
                return View();
            }
            //if(dispatch.TIntake < dispatch.Dispatchkgs)
            //{
            //    _notyf.Error("Sorry, Dispatch amount cannot be greater than stock");
            //    GetInitialValues();
            //    return View();
            //}

            if (ModelState.IsValid)
            {
                var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
                dispatch.auditid = user;
                dispatch.Dcode = sacco;
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

                var debtor = _context.DDebtors.FirstOrDefault(d => d.Dname.Trim().ToUpper().Equals(dispatch.DName.ToUpper()) && d.Dcode==sacco);

                _context.Gltransactions.Add(new Gltransaction
                {
                    AuditId = auditId,
                    TransDate = DateTime.Today,
                    Amount = (decimal)(debtor.Price * dispatch.Dispatchkgs),
                    AuditTime = DateTime.Now,
                    DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                    Source = dispatch.DName,
                    TransDescript = "Sales",
                    Transactionno = $"{auditId}{DateTime.Now}",
                    SaccoCode = sacco,
                    DrAccNo = debtor.AccDr,
                    CrAccNo = debtor.AccCr,
                });

                _context.Add(dispatch);
                await _context.SaveChangesAsync();
                _notyf.Success("Dispatch saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dispatch);
        }

        // GET: Dispatches/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var dispatch = await _context.Dispatch.FindAsync(id);
            if (dispatch == null)
            {
                return NotFound();
            }
            return View(dispatch);
        }

        // POST: Dispatches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Dcode,DName,Transdate,Dispatchkgs,TIntake,auditid")] Dispatch dispatch)
        {
            utilities.SetUpPrivileges(this);
            var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != dispatch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dispatch.auditid = user;
                    dispatch.Dcode = sacco;
                    _context.Update(dispatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DispatchExists(dispatch.Id))
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
            return View(dispatch);
        }

        // GET: Dispatches/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dispatch = await _context.Dispatch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispatch == null)
            {
                return NotFound();
            }

            return View(dispatch);
        }

        [HttpGet]
        public JsonResult SelectedDateIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var todaysIntake = GetTodaysIntake(date);
            return Json(todaysIntake);
        }

        private decimal GetTodaysIntake(DateTime date)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var intakes = _context.ProductIntake.Where(i => i.TransDate == date && i.SaccoCode == sacco && i.Description == "Intake");
            var todaysIntake = intakes.Sum(i => i.Qsupplied);
            var dispatchKgs = _context.Dispatch.Where(d => d.Dcode == sacco && d.Transdate == date).Sum(d => d.Dispatchkgs);
            todaysIntake -= dispatchKgs;
            return todaysIntake;
        }

        // POST: Dispatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dispatch = await _context.Dispatch.FindAsync(id);
            _context.Dispatch.Remove(dispatch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DispatchExists(long id)
        {
            utilities.SetUpPrivileges(this);
            return _context.Dispatch.Any(e => e.Id == id);
        }
    }
}
