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
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.ViewModels;

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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var dispatches = await _context.Dispatch
                .Where(i => i.Dcode.ToUpper().Equals(sacco.ToUpper())
                && i.Transdate >= startDate && i.Transdate <= endDate)
                .OrderByDescending(s => s.Transdate).ToListAsync();

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                dispatches = dispatches.Where(s => s.Branch.ToUpper().Equals(saccoBranch.ToUpper())).ToList();

            return View(dispatches);
        }

        // GET: Dispatches Branch
        public async Task<IActionResult> BranchIndex()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return View(await _context.Dispatch
                .Where(i => i.Dcode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccoBranch
                && i.Transdate>= startDate && i.Transdate<= DateTime.Today)
                .OrderByDescending(s => s.Transdate).ToListAsync());
        }

        // GET: Dispatches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        // GET: Dispatches/Create
        public IActionResult BranchCreate()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BranchCreate([Bind("Id,Dcode,DName,Transdate,Dispatchkgs,TIntake,Remarks,auditid")] Dispatch dispatch)
        {
            try
            {
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                if (string.IsNullOrEmpty(loggedInUser))
                    return Redirect("~/");
                utilities.SetUpPrivileges(this);
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var branch = HttpContext.Session.GetString(StrValues.Branch);
                var locations = _context.Dispatch.Any(i => i.DName == dispatch.DName && i.Dcode == sacco 
                && i.Transdate == dispatch.Transdate && i.Branch == branch);
                //if (locations)
                //{
                //    _notyf.Error("Sorry, The Dispatch to Debtor Name already exist");
                //    GetInitialValues();
                //    return View();
                //}
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
                    dispatch.Branch = branch;
                    _context.Add(dispatch);
                    _context.SaveChanges();
                    var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                    var debtor = _context.DDebtors.FirstOrDefault(d => d.Dname.Trim().ToUpper().Equals(dispatch.DName.ToUpper()) && d.Dcode == sacco);

                    _context.Gltransactions.Add(new Gltransaction
                    {
                        AuditId = auditId,
                        TransDate = DateTime.Today,
                        Amount = (decimal)(debtor.Price * dispatch.Dispatchkgs),
                        AuditTime = DateTime.Now,
                        DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                        Source = dispatch.DName + branch,
                        TransDescript = "Sales",
                        Transactionno = $"{auditId}{DateTime.Now}",
                        SaccoCode = sacco,
                        DrAccNo = debtor.AccDr,
                        CrAccNo = debtor.AccCr,
                    });
                    _context.SaveChanges();
                    _notyf.Success("Dispatch saved successfully");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return View(dispatch);
            }
            return View(dispatch);
        }


        // POST: Dispatches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dcode,DName,Transdate,Dispatchkgs,TIntake,Remarks,auditid")] Dispatch dispatch)
        {
            try
            {
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                if (string.IsNullOrEmpty(loggedInUser))
                    return Redirect("~/");
                utilities.SetUpPrivileges(this);
                GetInitialValues();
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                var locations = await _context.Dispatch.Where(i => i.DName == dispatch.DName && i.Dcode == sacco && i.Transdate == dispatch.Transdate).ToListAsync();
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    locations = locations.Where(s => s.Branch.ToUpper().Equals(saccoBranch.ToUpper())).ToList();

                if (dispatch.TIntake < dispatch.Dispatchkgs)
                {
                    _notyf.Error("Sorry, Dispatch amount cannot be greater than stock");
                    return View();
                }

                if (ModelState.IsValid)
                {
                    dispatch.auditid = loggedInUser;
                    dispatch.Dcode = sacco;
                    dispatch.Branch = saccoBranch;
                    _context.Add(dispatch);
                    _context.SaveChanges();
                    var debtor = _context.DDebtors.FirstOrDefault(d => d.Dname.Trim().ToUpper().Equals(dispatch.DName.ToUpper()) && d.Dcode == sacco);
                    _context.Gltransactions.Add(new Gltransaction
                    {
                        AuditId = loggedInUser,
                        TransDate = dispatch.Transdate,
                        Amount = (decimal)(debtor.Price * dispatch.Dispatchkgs),
                        AuditTime = DateTime.Now,
                        DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                        Source = dispatch.DName,
                        TransDescript = "Sales",
                        Transactionno = $"{loggedInUser}{DateTime.Now}",
                        SaccoCode = sacco,
                        DrAccNo = debtor.AccDr,
                        CrAccNo = debtor.AccCr,
                        Branch = saccoBranch
                    });
                    _context.SaveChanges();
                    _notyf.Success("Dispatch saved successfully");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return View(dispatch);
            }
            return View(dispatch);
        }

        // GET: Dispatches/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
        public async Task<JsonResult> SelectedDateIntake(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var todaysIntake = await GetTodaysIntake(date);
            return Json(todaysIntake);
        }

        [HttpGet]
        public JsonResult SelectedDateIntakebranch(DateTime date)
        {
            utilities.SetUpPrivileges(this);
            var branch = HttpContext.Session.GetString(StrValues.Branch);
            var todaysIntake = GetTodaysIntake(date, branch);
            return Json(todaysIntake);
        }
        private decimal GetTodaysIntake(DateTime date, string branch)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var intakes = _context.ProductIntake.Where(i => i.TransDate == date && i.SaccoCode == sacco && i.Branch==branch && i.Description == "Intake");
            var todaysIntake = intakes.Sum(i => i.Qsupplied);
            var dispatchKgs = _context.Dispatch.Where(d => d.Dcode == sacco && d.Branch == branch && d.Transdate == date).Sum(d => d.Dispatchkgs);
            todaysIntake -= dispatchKgs;
            return todaysIntake;
        }
        private async Task<decimal> GetTodaysIntake(DateTime date)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var intakes = await _context.ProductIntake.Where(i => i.TransDate == date && (i.TransactionType == TransactionType.Intake || i.TransactionType == TransactionType.Correction) && i.SaccoCode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccoBranch).ToList();
            
            var dispatches = await _context.Dispatch.Where(d => d.Transdate == date && d.Dcode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                dispatches = dispatches.Where(i => i.Branch == saccoBranch).ToList();

            var yesturday = date.AddDays(-1);
            var yesturdayBalancing = await _context.DispatchBalancing.FirstOrDefaultAsync(d => d.Saccocode == sacco && d.Date == yesturday);
            decimal broughtForward = yesturdayBalancing?.CF ?? 0;
            var todaysIntake = intakes.Sum(i => i.Qsupplied);
            var dispatchKgs = dispatches.Sum(d => d.Dispatchkgs);
            todaysIntake -= dispatchKgs;
            return todaysIntake + broughtForward;
        }

        // POST: Dispatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
