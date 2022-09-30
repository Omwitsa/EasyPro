using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace EasyPro.Controllers
{
    public class GlsetupsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public GlsetupsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Glsetups
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.Glsetups.Where(g => g.saccocode == sacco).ToListAsync());
        }

        // GET: Glsetups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var glsetup = await _context.Glsetups
                .FirstOrDefaultAsync(m => m.AccNo == id);
            if (glsetup == null)
            {
                return NotFound();
            }

            return View(glsetup);
        }

        // GET: Glsetups/Create
        public IActionResult Create()
        {
            SetInitialValues();
            utilities.SetUpPrivileges(this);
            return View();
        }

        private void SetInitialValues()
        {
            var accTypes = new string[] { "Income Statement", "Balance Sheet", "Retained Earnings" };
            ViewBag.accTypes = new SelectList(accTypes);
            var accGroups = new string[] { "ASSETS", "CAPITAL", "EXPENSES", "INCOME", "LIABILITIES", "RETAINED EARNINGS", "SUSPENSE ACCOUNT" };
            ViewBag.accGroups = new SelectList(accGroups);
            var subGroups = new string[] { "Accumulated Depreciation", "Assets", "Board Costs", "Closing Inventory", 
                "Cost and Expenses", "Cost of Sales", "Current Assets", "Current liabilities", "Fixed assets", 
                "Interest Income", "Liabilities", "Long Term Liability", "Openning Inventory", "Operating Expenses", 
                "Operating Income", "Other", "Other Assets", "Other Income and Expenses", "Provision for Income Taxes", 
                "Purchases", "Retained Earnings", "Revenue", "Share Holders Equity", "Staff Costs", "Suspense Acc" };
            ViewBag.subGroups = new SelectList(subGroups);
            var normalBal = new string[] { "Debit", "Credit" };
            ViewBag.normalBal = new SelectList(normalBal);
            var currencies = new string[] { "KES", "USD", "GBP", "TSH", "USH", "ZAR" };
            ViewBag.currencies = new SelectList(currencies);
            var categories = new string[] { "FINANCIAL EXP", "PERSONNEL EXP", "ADMININISTRATIVE EXP", "MANAGEMENT EXP", "GOVERNANCE EXP", "DIRECT EXP" };
            ViewBag.categories = new SelectList(categories);
        }

        // POST: Glsetups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Glid,GlCode,GlAccName,AccNo,GlAccType,GlAccGroup,GlAccMainGroup,NormalBal,GlAccStatus,OpeningBal,CurrentBal,Bal,CurrCode,AuditOrg,AuditId,AuditDate,Curr,Actuals,Budgetted,TransDate,IsSubLedger,AccCategory,NewGlopeningBal,NewGlopeningBalDate,Branch,Hcode,Mcode,Hname,Header,Mheader,Iorder,Border,Type,Subtype,IsRearning,Issuspense,Run,saccocode")] Glsetup glsetup)
        {
            utilities.SetUpPrivileges(this);
            glsetup.NewGlopeningBalDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                    if (string.IsNullOrEmpty(glsetup.AccNo))
                    {
                        _notyf.Error("Sorry, Kindly provide account No.");
                        return RedirectToAction(nameof(Create));
                    }
                    if (string.IsNullOrEmpty(glsetup.GlAccName))
                    {
                        _notyf.Error("Sorry, Kindly provide account Name");
                        return RedirectToAction(nameof(Create));
                    }
                    if (string.IsNullOrEmpty(glsetup.NormalBal))
                    {
                        _notyf.Error("Sorry, Kindly provide normal balance");
                        return RedirectToAction(nameof(Create));
                    }
                    if (_context.Glsetups.Any(u => u.saccocode == sacco && u.AccNo.ToUpper().Equals(glsetup.AccNo.ToUpper())))
                    {
                        _notyf.Error("Sorry, Account No. already exist");
                        return RedirectToAction(nameof(Create));
                    }
                    if (_context.Glsetups.Any(u =>u.saccocode==sacco && u.GlAccName.ToUpper().Equals(glsetup.GlAccName.ToUpper())))
                    {
                        _notyf.Error("Sorry, Account Name already exist");
                        return RedirectToAction(nameof(Create));
                    }

                    glsetup.CurrCode = glsetup?.CurrCode ?? 0;
                    glsetup.IsSubLedger = glsetup?.IsSubLedger ?? false;
                    glsetup.saccocode = sacco;
                    _context.Add(glsetup);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                   
                }
            }
            _notyf.Error("Sorry, An error occurred");
            return RedirectToAction(nameof(Create));
        }

        // GET: Glsetups/Edit/5
        public async Task<IActionResult> Edit(long Glid)
        {
            SetInitialValues();
            utilities.SetUpPrivileges(this);
            if (Glid == 0)
            {
                return NotFound();
            }
            var glsetup = await _context.Glsetups.FindAsync(Glid);
            if (glsetup == null)
            {
                return NotFound();
            }
            return View(glsetup);
        }

        // POST: Glsetups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long Glid, [Bind("Glid,GlCode,GlAccName,AccNo,GlAccType,GlAccGroup,GlAccMainGroup,NormalBal,GlAccStatus,OpeningBal,CurrentBal,Bal,CurrCode,AuditOrg,AuditId,AuditDate,Curr,Actuals,Budgetted,TransDate,IsSubLedger,AccCategory,NewGlopeningBal,NewGlopeningBalDate,Branch,Hcode,Mcode,Hname,Header,Mheader,Iorder,Border,Type,Subtype,IsRearning,Issuspense,Run,saccocode")] Glsetup glsetup)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var LoggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (Glid != glsetup.Glid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(glsetup.AccNo))
                    {
                        _notyf.Error("Sorry, Kindly provide account No.");
                        return View(glsetup);
                    }
                    if (string.IsNullOrEmpty(glsetup.GlAccName))
                    {
                        _notyf.Error("Sorry, Kindly provide account Name");
                        return View(glsetup);
                    }
                    if (string.IsNullOrEmpty(glsetup.NormalBal))
                    {
                        _notyf.Error("Sorry, Kindly provide normal balance");
                        return View(glsetup);
                    }
                    if (_context.Glsetups.Any(u => u.saccocode == sacco && u.AccNo.ToUpper().Equals(glsetup.AccNo.ToUpper()) && u.Glid != glsetup.Glid))
                    {
                        _notyf.Error("Sorry, Account No. already exist");
                        return View(glsetup);
                    }
                    if (_context.Glsetups.Any(u =>u.saccocode==sacco && u.GlAccName.ToUpper().Equals(glsetup.GlAccName.ToUpper()) && u.Glid != glsetup.Glid))
                    {
                        _notyf.Error("Sorry, Account Name already exist");
                        return View(glsetup);
                    }
                        glsetup.TransDate = DateTime.Now;
                        glsetup.NewGlopeningBalDate = DateTime.Now;
                        glsetup.AuditDate = DateTime.Now;
                        glsetup.Branch = "Null";
                        glsetup.saccocode = sacco;
                        glsetup.AuditId = LoggedInUser;
                    _context.Update(glsetup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlsetupExists(glsetup.AccNo))
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
            return View(glsetup);
        }

        // GET: Glsetups/Delete/5
        public async Task<IActionResult> Delete(string Glid)
        {
            utilities.SetUpPrivileges(this);
            if (Glid == null)
            {
                return NotFound();
            }

            var glsetup = await _context.Glsetups
                .FirstOrDefaultAsync(m => m.AccNo == Glid);
            if (glsetup == null)
            {
                return NotFound();
            }

            return View(glsetup);
        }

        // POST: Glsetups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Glid)
        {
            utilities.SetUpPrivileges(this);
            var glsetup = await _context.Glsetups.FindAsync(Glid);
            _context.Glsetups.Remove(glsetup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GlsetupExists(string Glid)
        {
            return _context.Glsetups.Any(e => e.AccNo == Glid);
        }
    }
}
