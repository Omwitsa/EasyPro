using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels.TransportersVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Stripe;
using Stripe.FinancialConnections;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EasyPro.ViewModels.AccountingVm;

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
        public async Task<IActionResult> JournalPosting()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var glAccounts = await _context.Glsetups.Where(a => a.saccocode == sacco).ToListAsync();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> JournalPosting([FromBody] List<Gltransaction> transactions)
        {
            try
            {
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                transactions.ForEach(t =>
                {
                    t.AuditId = loggedInUser;
                    t.TransDate = t.TransDate;
                    t.AuditTime = DateTime.Now;
                    t.Source = "";
                    t.TransDescript = t?.TransDescript ?? "";
                    t.Transactionno = $"{loggedInUser}{DateTime.Now}";
                    t.SaccoCode = sacco;
                    t.Branch = saccoBranch;
                });
                await _context.Gltransactions.AddRangeAsync(transactions);
                await _context.SaveChangesAsync();
                _notyf.Success("Journal posted successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        public async Task<IActionResult> Budget()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var perMonth = new string[] { "Yes", "No" };
            ViewBag.perMonth = new SelectList(perMonth);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = await _context.Glsetups.Where(a => a.saccocode == sacco).ToListAsync();
            ViewBag.glAccounts = glAccounts;
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Comparison(DateTime period)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var budgets = await _context.Budgets.Where(b => b.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && b.Mmonth == period.Month && b.Yyear == period.Year).ToListAsync();
            var glsetups = await _context.Glsetups.Where(s => s.saccocode == sacco).ToListAsync();
            var balances = await _context.CustomerBalances.Where(b => b.SCode== sacco
            && b.TransDate.GetValueOrDefault().Month == period.Month 
            && b.TransDate.GetValueOrDefault().Year == period.Year).ToListAsync();
            var comparisons = new List<ComparisonVm>();
            budgets.ForEach(b =>
            {
                var glsetup = glsetups.FirstOrDefault(g => g.AccNo.ToUpper().Equals(b.Accno.ToUpper()));
                var accName = glsetup?.GlAccName ?? "";
                var customerBalances = balances.Where(c => c.AccNo.ToUpper().Equals(b.Accno.ToUpper()));
                var actualAmount = customerBalances.Sum(b => b.Amount);
                var variance = b.Budgetted - actualAmount;
                comparisons.Add(new ComparisonVm
                {
                    AccNo = b.Accno,
                    AccName = accName,
                    BudgettedAmount = b.Budgetted,
                    ActualAmount = actualAmount,
                    Variance = variance,
                    Percentage = variance / b.Budgetted * 100
                });
            });
            return Json(comparisons);
        }

        [HttpPost]
        public async Task<JsonResult> Budget([FromBody] BudgetFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var budgets = await _context.Budgets.Where(b => b.Accno.ToUpper().Equals(filter.AccNo.ToUpper())
                && b.Yyear == filter.Period.Year && b.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
                if (budgets.Any())
                {
                    _context.Budgets.RemoveRange(budgets);
                    _context.SaveChanges();
                }

                if (!filter.Fixed)
                    filter.Amount = filter.Amount / 12;
                var currentMonth = 1;
                while (currentMonth <= 12)
                {
                    _context.Budgets.Add(new Budget
                    {
                        Accno = filter.AccNo,
                        Mmonth = currentMonth,
                        Yyear = filter.Period.Year,
                        Budgetted = filter.Amount,
                        SaccoCode = sacco,
                        Actual = 0,
                        Variance = 0,
                    });

                    ++currentMonth;
                }

                _context.SaveChanges();
                _notyf.Success("Budget saved successfully");
                budgets = await _context.Budgets.Where(b => b.Accno.ToUpper().Equals(filter.AccNo.ToUpper())
                && b.Yyear == filter.Period.Year && b.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();

                DateTime lastDay = new DateTime(filter.Period.Year, filter.Period.Month, 1).AddMonths(1).AddDays(-1);
                var budgetVms = new List<BudgetVm>();
                foreach(var budget in budgets)
                {
                    budgetVms.Add(new BudgetVm
                    {
                        Period = budget.Mmonth,
                        EndDate = lastDay,
                        BudgettedAmount = budget.Budgetted
                    });
                }
                return Json(budgetVms);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        public async Task<IActionResult> GLInquiry()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = await _context.Glsetups.Where(a => a.saccocode == sacco).ToListAsync();
            ViewBag.glAccounts = glAccounts;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GLInquiry([FromBody] JournalFilter filter)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var gltransactions = await _context.Gltransactions
                .Where(t => t.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate
                && (t.DrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())
                || t.CrAccNo.ToUpper().Equals(filter.AccNo.ToUpper()))).ToListAsync();

            var debit = gltransactions.Where(t => t.DrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())).ToList();
            var credit = gltransactions.Where(t => t.CrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())).ToList();
            var glsetup = await _context.Glsetups.FirstOrDefaultAsync(t => t.AccNo.Trim().ToUpper().Equals(filter.AccNo.ToUpper()) && t.saccocode == sacco);
            var journals = new List<JournalVm>();
            var bookBalance = 0M;
            if (glsetup != null)
            {
                var isDebit = glsetup.NormalBal.ToLower().Equals("debit");
                var debitAmt = debit.Sum(t => t.Amount);
                var creditAmt = credit.Sum(t => t.Amount);
                if (isDebit)
                    bookBalance = glsetup.OpeningBal + debitAmt - creditAmt;
                else
                    bookBalance = glsetup.OpeningBal + creditAmt - debitAmt;

                var balance = glsetup.OpeningBal;
                debit.ForEach(d =>
                {
                    journals.Add(new JournalVm
                    {
                        DocumentNo = d.DocumentNo,
                        TransDescript = d.TransDescript,
                        TransDate = d.TransDate,
                        Dr = d.Amount,
                        Cr = 0,
                    });
                });

                credit.ForEach(d =>
                {
                    journals.Add(new JournalVm
                    {
                        DocumentNo = d.DocumentNo,
                        TransDescript = d.TransDescript,
                        TransDate = d.TransDate,
                        Dr = 0,
                        Cr = d.Amount
                    });
                });

                journals = journals.OrderBy(j => j.TransDate).ToList();
                journals.ForEach(j =>
                {
                    if (isDebit)
                        balance = balance + j.Dr - j.Cr;
                    else
                        balance = balance + j.Cr - j.Dr;

                    j.Bal = balance;
                });
            }

            return Json(new {
                bookBalance,
                journals
            });
        }

        public IActionResult JournalListing()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> JournalListing([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                List<Glsetup> glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco).ToListAsync();
                glsetups.ForEach(g =>
                {
                    g.NormalBal = g?.NormalBal ?? "";
                    g.OpeningBal = g?.OpeningBal ?? 0;
                    if (g.OpeningBal > 0)
                    {
                        decimal dr = 0;
                        decimal cr = 0;
                        if (g.NormalBal.ToUpper().Equals("DR"))
                            dr = g.OpeningBal;
                        if (g.NormalBal.ToUpper().Equals("CR"))
                            cr = g.OpeningBal;
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = g.AccNo,
                            TransDate = filter.ToDate,
                            AccName = g.GlAccName,
                            AccCategory = g.AccCategory,
                            Dr = dr,
                            DocumentNo = "",
                            Cr = cr,
                            TransDescript = "Opening Bal"
                        });
                    }
                });

                var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco 
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToListAsync();
                
                gltransactions.ForEach(t =>
                {
                    var debtorsAcc = glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo);
                    if(debtorsAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = t.DrAccNo,
                            TransDate = t.TransDate,
                            AccName = debtorsAcc.GlAccName,
                            AccCategory = debtorsAcc.AccCategory,
                            Dr = t.Amount,
                            DocumentNo = t.DocumentNo,
                            Cr = 0,
                            TransDescript = t.TransDescript
                        });
                    }
                    var creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo);
                    if(creditorsAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = t.CrAccNo,
                            TransDate = t.TransDate,
                            AccName = creditorsAcc.GlAccName,
                            AccCategory = creditorsAcc.AccCategory,
                            Cr = t.Amount,
                            DocumentNo = t.DocumentNo,
                            Dr = 0,
                            TransDescript = t.TransDescript
                        });
                    }
                });

                var totalCr = journalListings.Sum(j => j.Cr);
                var totalDr = journalListings.Sum(j => j.Dr);
                journalListings.Add(new JournalVm
                {
                    GlAcc = "",
                    TransDate = null,
                    AccCategory = "",
                    Cr = totalCr,
                    DocumentNo = "",
                    Dr = totalDr,
                    TransDescript = ""
                });

                return Json(journalListings);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        [HttpGet]
        public async Task<JsonResult> IntakeEndOfDay()
        {
            try
            {
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var productIntakes = await _context.ProductIntake
                    .Where(i => i.TransDate == DateTime.Today && i.SaccoCode == sacco && (i.Posted == null || !(bool)i.Posted)).ToListAsync();

                var intakes = productIntakes.GroupBy(i => i.ProductType).ToList();
                intakes.ForEach(i =>
                {
                    var firstIntake = i.FirstOrDefault();
                    if(!string.IsNullOrEmpty(firstIntake.CrAccNo) && !string.IsNullOrEmpty(firstIntake.DrAccNo))
                    {
                        _context.Gltransactions.Add(new Gltransaction
                        {
                            AuditId = auditId,
                            TransDate = DateTime.Today,
                            Amount = (decimal)i.Sum(t => t.CR),
                            AuditTime = DateTime.Now,
                            Source = $"EOD {firstIntake.ProductType} Purchases",
                            TransDescript = "Intake",
                            Transactionno = $"{auditId}{DateTime.Now}",
                            SaccoCode = sacco,
                            DrAccNo = firstIntake.DrAccNo,
                            CrAccNo = firstIntake.CrAccNo,
                        });

                        foreach (var intake in i)
                        {
                            intake.Posted = true;
                        }

                        _context.SaveChanges();
                    }
                });

                var journalListings = new List<JournalVm>();
                var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= DateTime.Today && t.TransDescript == "Intake").ToListAsync();
                var glsetups = await _context.Glsetups.Where(s => s.saccocode == sacco).ToListAsync();
                gltransactions.ForEach(t =>
                {
                    var debtorssAcc = glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo);
                    if (debtorssAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = t.DrAccNo,
                            TransDate = t.TransDate,
                            AccName = debtorssAcc.GlAccName,
                            AccCategory = debtorssAcc.AccCategory,
                            Dr = t.Amount,
                            DocumentNo = t.DocumentNo,
                            Cr = 0,
                            TransDescript = t.TransDescript
                        });
                    }
                    

                    var creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo);
                    if(creditorsAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = t.CrAccNo,
                            TransDate = t.TransDate,
                            Cr = t.Amount,
                            AccName = creditorsAcc.GlAccName,
                            AccCategory = creditorsAcc.AccCategory,
                            DocumentNo = t.DocumentNo,
                            Dr = 0,
                            TransDescript = t.TransDescript
                        });
                    }
                });

                var totalCr = journalListings.Sum(j => j.Cr);
                var totalDr = journalListings.Sum(j => j.Dr);
                journalListings.Add(new JournalVm
                {
                    GlAcc = "",
                    AccCategory = "",
                    TransDate = null,
                    Cr = totalCr,
                    DocumentNo = "",
                    Dr = totalDr,
                    TransDescript = ""
                });

                return Json(journalListings);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        public IActionResult TrialBalance()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> TrialBalance([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                List<Glsetup> glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco).ToListAsync();
                glsetups.ForEach(g =>
                {
                    g.NormalBal = g?.NormalBal ?? "";
                    g.OpeningBal = g?.OpeningBal ?? 0;
                    if(g.OpeningBal > 0)
                    {
                        decimal dr = 0;
                        decimal cr = 0;
                        if (g.NormalBal.ToLower().Equals("debit"))
                            dr = g.OpeningBal;
                        if (g.NormalBal.ToLower().Equals("credit"))
                            cr = g.OpeningBal;
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = g.AccNo,
                            TransDate = filter.ToDate,
                            AccName = g.GlAccName,
                            AccCategory = g.AccCategory,
                            Dr = dr,
                            DocumentNo = "",
                            Cr = cr,
                            TransDescript = "Opening Bal"
                        });
                    }
                });

                var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToListAsync();
                gltransactions.ForEach(t =>
                {
                    var debtorssAcc = glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo);
                    if(debtorssAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = t.DrAccNo,
                            TransDate = t.TransDate,
                            AccName = debtorssAcc.GlAccName,
                            AccCategory = debtorssAcc.AccCategory,
                            Dr = t.Amount,
                            DocumentNo = t.DocumentNo,
                            Cr = 0,
                            TransDescript = t.TransDescript
                        });
                    }

                    var creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo);
                    if(creditorsAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = t.CrAccNo,
                            TransDate = t.TransDate,
                            AccName = creditorsAcc.GlAccName,
                            AccCategory = creditorsAcc.AccCategory,
                            Cr = t.Amount,
                            DocumentNo = t.DocumentNo,
                            Dr = 0,
                            TransDescript = t.TransDescript
                        });
                    }
                });

                var totalCr = journalListings.Sum(j => j.Cr);
                var totalDr = journalListings.Sum(j => j.Dr);
                journalListings.Add(new JournalVm
                {
                    GlAcc = "",
                    AccCategory = "",
                    TransDate = null,
                    Cr = totalCr,
                    DocumentNo = "",
                    Dr = totalDr,
                    TransDescript = ""
                });

                return Json(journalListings);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        public IActionResult IncomeStatement()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> IncomeStatement([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco && g.GlAccType == "Income Statement").ToListAsync();
                if (StrValues.Slopes == sacco)
                    glsetups = glsetups.Where(g => !g.GlAccName.ToUpper().Equals("AGROVET STORE") && !g.GlAccName.ToUpper().Equals("AGROVET SALES")
                    && !g.GlAccName.ToUpper().Equals("STORE")).ToList();
                var journalListings = await GetIncomeStatement(filter, glsetups);
                var income = journalListings.Where(a => a.Group == "INCOME").ToList();
                var expenses = journalListings.Where(a => a.Group == "EXPENSES").ToList();
                var totalKgs = await _context.ProductIntake.Where(i => (i.Description == "Intake" || i.Description == "Correction") 
                && i.TransDate >= filter.FromDate && i.TransDate <= filter.ToDate && i.SaccoCode == sacco)
                    .SumAsync(i => i.Qsupplied);
                return Json(new
                {
                    income,
                    expenses,
                    totalKgs
                });
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpPost]
        public async Task<JsonResult> IncomeStatementSummary([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco && g.GlAccType == "Income Statement"
                && !g.GlAccName.ToUpper().Equals("AGROVET STORE") && !g.GlAccName.ToUpper().Equals("AGROVET SALES")
                && !g.GlAccName.ToUpper().Equals("STORE")).ToListAsync();
                var debtors = await _context.DDebtors.Where(p => p.Dcode == sacco).ToListAsync();
                var dispatches = await _context.Dispatch.Where(p => p.Dcode == sacco && p.Transdate >= filter.FromDate 
                && p.Transdate <= filter.ToDate).ToListAsync();
                var journalListings = await GetIncomeStatement(filter, glsetups);
                var incomes = journalListings.Where(a => a.Group == "INCOME").ToList().GroupBy(a => a.TransDescript).ToList();
                var income = new List<Votehead>();
                incomes.ForEach(i =>
                {
                    var kgs = dispatches.Where(d => d.DName == i.Key).Sum(d => d.Dispatchkgs);
                    var debtor = debtors.FirstOrDefault(d => d.Dname == i.Key);
                    if(debtor != null)
                        income.Add(new Votehead
                        {
                            Name = i.Key,
                            Quantity = kgs,
                            Price = debtor.Price,
                            Amount = i.Sum(o => o.Cr)
                        });
                });

                var expense = journalListings.Where(a => a.Group == "EXPENSES").ToList().GroupBy(a => a.AccCategory).ToList();
                var expenses = new List<StatementSummaryVm>();
                expense.ForEach(e =>
                {
                    var voteheads = new List<Votehead>();
                    var accounts = e.ToList().GroupBy(a => a.AccName);
                    accounts.ForEach(a =>
                    {
                        var accNo = a.FirstOrDefault()?.GlAcc ?? "";
                        voteheads.Add(new Votehead
                        {
                            Name = a.Key,
                            AccNo = accNo,
                            Amount = a.Sum(o => o.Dr),
                        });
                    });

                    expenses.Add(new StatementSummaryVm
                    {
                        Categoy = e.Key,
                        voteheads = voteheads,
                        Total = voteheads.Sum(c => c.Amount)
                    });
                });
                var totalKgs = await _context.ProductIntake.Where(i => ((i.Description == "Intake" || i.Description == "Correction"))
                && i.TransDate >= filter.FromDate && i.TransDate <= filter.ToDate && i.SaccoCode == sacco)
                    .SumAsync(i => i.Qsupplied);
                return Json(new
                {
                    income,
                    expenses,
                    totalKgs
                });
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        private async Task<List<JournalVm>> GetIncomeStatement(JournalFilter filter, List<Glsetup> glsetups)
        {
            var journalListings = new List<JournalVm>();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco
            && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                .ToListAsync();
            glsetups.ForEach(g =>
            {
                g.NormalBal = g?.NormalBal ?? "";
                g.OpeningBal = g?.OpeningBal ?? 0;
                if (g.OpeningBal > 0)
                {
                    decimal dr = 0;
                    decimal cr = 0;
                    if (g.NormalBal.ToLower().Equals("debit"))
                        dr = g.OpeningBal;
                    if (g.NormalBal.ToLower().Equals("credit"))
                        cr = g.OpeningBal;

                    journalListings.Add(new JournalVm
                    {
                        GlAcc = g.AccNo,
                        TransDate = filter.ToDate,
                        AccName = g.GlAccName,
                        AccCategory = g.AccCategory,
                        Cr = cr,
                        DocumentNo = "",
                        Dr = dr,
                        TransDescript = "Opening Bal",
                        Group = g.GlAccMainGroup,
                    });
                }
            });

            gltransactions.ForEach(t =>
            {
                t.TransDescript = t.TransDescript == "Sales" ? t.Source : t.TransDescript;
                var debtorssAcc = glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo);
                if (debtorssAcc != null)
                {
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = t.DrAccNo,
                        TransDate = t.TransDate,
                        AccName = debtorssAcc.GlAccName,
                        AccCategory = debtorssAcc.AccCategory,
                        Dr = t.Amount,
                        DocumentNo = t.DocumentNo,
                        Cr = 0,
                        TransDescript = t.TransDescript,
                        Group = debtorssAcc.GlAccMainGroup,
                    });
                }

                var creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo);
                if (creditorsAcc != null)
                {
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = t.CrAccNo,
                        TransDate = t.TransDate,
                        AccName = creditorsAcc.GlAccName,
                        AccCategory = creditorsAcc.AccCategory,
                        Cr = t.Amount,
                        DocumentNo = t.DocumentNo,
                        Dr = 0,
                        TransDescript = t.TransDescript,
                        Group = creditorsAcc.GlAccMainGroup,
                    });
                }
            });

            var balancings = await _context.DispatchBalancing.Where(d => d.Saccocode == sacco && d.Date >= filter.FromDate && d.Date <= filter.ToDate).ToListAsync();

            var standingOrders = await _context.SocietyStandingOrder.Where(o => o.SaccoCode == sacco).ToListAsync();
            standingOrders.ForEach(t =>
            {
                 
                if (t.HasRate)
                {
                    var balancing = balancings.Where(b => b.Date == filter.FromDate).FirstOrDefault();
                    var broughtForward = balancing?.BF ?? 0;
                    var totalKgs = balancings.Sum(b => b.Intake) + broughtForward;
                    t.Amount = (decimal)(t.Amount * totalKgs);
                }
                var debtorssAcc = glsetups.FirstOrDefault(a => a.AccNo == t.GlAcc);
                if (debtorssAcc != null)
                {
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = t.GlAcc,
                        TransDate = filter.ToDate,
                        AccName = debtorssAcc.GlAccName,
                        AccCategory = debtorssAcc.AccCategory,
                        Dr = t.Amount,
                        DocumentNo = "",
                        Cr = 0,
                        TransDescript = t.Name,
                        Group = debtorssAcc.GlAccMainGroup,
                    });
                }

                var creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == t.ContraAcc);
                if (creditorsAcc != null)
                {
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = t.ContraAcc,
                        TransDate = filter.ToDate,
                        AccName = creditorsAcc.GlAccName,
                        AccCategory = creditorsAcc.AccCategory,
                        Cr = t.Amount,
                        DocumentNo = "",
                        Dr = 0,
                        TransDescript = t.Name,
                        Group = creditorsAcc.GlAccMainGroup,
                    });
                }
            });

            if(StrValues.Slopes == sacco)
            {
                var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == sacco);
                var intakes = await _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= filter.FromDate && i.TransDate <= filter.ToDate).ToListAsync();
                var farmersIntake = intakes.Where(i => (i.Description == "Intake" || i.Description == "Correction"));
                var activeFarmersNos = farmersIntake.Select(s => s.Sno.ToUpper()).Distinct();
                var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco && activeFarmersNos.Contains(s.Sno.ToUpper())).ToListAsync();
                var transporters = await _context.DTransporters.Where(t => t.ParentT == sacco).ToListAsync();
                var days = (filter.ToDate - filter.FromDate).TotalDays + 1;
                decimal transportationAmount = 0;
                decimal subsidy = 0;
                decimal tanker = 0;
                foreach (var transporter in transporters)
                {
                    var totalSupplied = intakes.Where(i => i.Sno.ToUpper().Equals(transporter.TransCode.ToUpper())).Sum(s => s.Qsupplied);
                    var averageSupplied = totalSupplied / (decimal)days;
                    transporter.TraderRate = transporter?.TraderRate ?? 0;
                    decimal amount = 0;
                    if (transporter.CertNo.ToUpper().Equals("KDK 015D"))
                        tanker = totalSupplied * (decimal)transporter.Rate;
                    else
                    {
                        amount = totalSupplied * (decimal)transporter.Rate;
                        // Assigning trader rate means the transporter is a trader
                        if (transporter.TraderRate > 0)
                        {
                            amount = totalSupplied * (decimal)transporter.TraderRate;
                            if (price != null && averageSupplied >= price.SubsidyQty)
                                subsidy += totalSupplied * (decimal)transporter.Rate;
                        }
                    }
                    transportationAmount += amount;
                }

                var debtorssAcc = glsetups.FirstOrDefault(a => a.AccNo == price.TransportDrAccNo);
                if (debtorssAcc != null)
                {
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = debtorssAcc.AccNo,
                        TransDate = filter.ToDate,
                        AccName = debtorssAcc.GlAccName,
                        AccCategory = debtorssAcc.AccCategory,
                        Dr = transportationAmount,
                        DocumentNo = "",
                        Cr = 0,
                        TransDescript = "Milk Transport",
                        Group = debtorssAcc.GlAccMainGroup,
                    });
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = debtorssAcc.AccNo,
                        TransDate = filter.ToDate,
                        AccName = debtorssAcc.GlAccName,
                        AccCategory = debtorssAcc.AccCategory,
                        Dr = tanker,
                        DocumentNo = "",
                        Cr = 0,
                        TransDescript = "Tanker",
                        Group = debtorssAcc.GlAccMainGroup,
                    });
                    if (subsidy > 0)
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = debtorssAcc.AccNo,
                            TransDate = filter.ToDate,
                            AccName = debtorssAcc.GlAccName,
                            AccCategory = debtorssAcc.AccCategory,
                            Dr = subsidy,
                            DocumentNo = "",
                            Cr = 0,
                            TransDescript = "Special Milk Transport",
                            Group = debtorssAcc.GlAccMainGroup,
                        });
                }

                var creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == price.TransportCrAccNo);
                if (creditorsAcc != null)
                {
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = creditorsAcc.AccNo,
                        TransDate = filter.ToDate,
                        AccName = creditorsAcc.GlAccName,
                        AccCategory = creditorsAcc.AccCategory,
                        Cr = transportationAmount,
                        DocumentNo = "",
                        Dr = 0,
                        TransDescript = "Milk Transport",
                        Group = creditorsAcc.GlAccMainGroup,
                    });
                    journalListings.Add(new JournalVm
                    {
                        GlAcc = creditorsAcc.AccNo,
                        TransDate = filter.ToDate,
                        AccName = creditorsAcc.GlAccName,
                        AccCategory = creditorsAcc.AccCategory,
                        Cr = tanker,
                        DocumentNo = "",
                        Dr = 0,
                        TransDescript = "Tanker",
                        Group = creditorsAcc.GlAccMainGroup,
                    });
                    if (subsidy > 0)
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = creditorsAcc.AccNo,
                            TransDate = filter.ToDate,
                            AccName = creditorsAcc.GlAccName,
                            AccCategory = creditorsAcc.AccCategory,
                            Cr = subsidy,
                            DocumentNo = "",
                            Dr = 0,
                            TransDescript = "Special Milk Transport",
                            Group = creditorsAcc.GlAccMainGroup,
                        });
                }

                decimal farmersSpecialPrice = 0;
                suppliers.ForEach(s =>
                {
                    var totalSupplied = farmersIntake.Where(i => i.Sno == s.Sno).Sum(i => i.Qsupplied);
                    var averageSupplied = totalSupplied / (decimal)days;
                    if (averageSupplied >= price.SubsidyQty)
                    {
                        farmersSpecialPrice += (decimal)(totalSupplied * price.SubsidyPrice);
                    }
                });

                if(farmersSpecialPrice > 0)
                {
                    debtorssAcc = glsetups.FirstOrDefault(a => a.AccNo == price.DrAccNo);
                    if (debtorssAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = debtorssAcc.AccNo,
                            TransDate = filter.ToDate,
                            AccName = debtorssAcc.GlAccName,
                            AccCategory = debtorssAcc.AccCategory,
                            Dr = farmersSpecialPrice,
                            DocumentNo = "",
                            Cr = 0,
                            TransDescript = "Farmers Special Price",
                            Group = debtorssAcc.GlAccMainGroup,
                        });
                    }

                    creditorsAcc = glsetups.FirstOrDefault(a => a.AccNo == price.CrAccNo);
                    if (creditorsAcc != null)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = creditorsAcc.AccNo,
                            TransDate = filter.ToDate,
                            AccName = creditorsAcc.GlAccName,
                            AccCategory = creditorsAcc.AccCategory,
                            Cr = farmersSpecialPrice,
                            DocumentNo = "",
                            Dr = 0,
                            TransDescript = "Farmers Special Price",
                            Group = creditorsAcc.GlAccMainGroup,
                        });
                    }
                }
            }

            return journalListings;
        }

        public IActionResult BalanceSheet()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> BalanceSheet([FromBody] JournalFilter filter)
        {
            try
            {
                var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToListAsync();

                var glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco).ToListAsync();
                var balanceSheetAccs = glsetups.Where(g => g.GlAccType == "Balance Sheet").ToList();
                balanceSheetAccs.ForEach(s =>
                {
                    s.NormalBal = s?.NormalBal ?? "";
                    s.OpeningBal = s?.OpeningBal ?? 0;
                    decimal debitAmount = 0;
                    decimal creditAmount = 0;
                    if (s.OpeningBal > 0)
                    {
                        if (s.NormalBal.ToLower().Equals("debit"))
                            debitAmount = s.OpeningBal;
                        if (s.NormalBal.ToLower().Equals("credit"))
                            creditAmount = s.OpeningBal;
                    }

                    var transaction = gltransactions.FirstOrDefault();
                    debitAmount += gltransactions.Where(t => t.DrAccNo == s.AccNo).Sum(t => t.Amount);
                    creditAmount += gltransactions.Where(t => t.CrAccNo == s.AccNo).Sum(t => t.Amount);
                    if (debitAmount != 0 || creditAmount != 0)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = s.AccNo,
                            TransDate = transaction?.TransDate ?? filter.ToDate,
                            AccName = s.GlAccName,
                            AccCategory = s.AccCategory,
                            Dr = debitAmount,
                            DocumentNo = transaction?.DocumentNo ?? "",
                            Cr = creditAmount,
                            TransDescript = transaction?.TransDescript ?? "Opening Bal",
                            Group = s.GlAccMainGroup,
                        });
                    }
                });

                var assets = journalListings.Where(a => a.Group == "ASSETS").ToList();
                var liabilities = journalListings.Where(a => a.Group == "LIABILITIES").ToList();
                var capitals = journalListings.Where(a => a.Group == "CAPITAL").ToList();

                var incomeStatementAccs = glsetups.Where(g => g.GlAccType == "Income Statement"
                && !g.GlAccName.ToUpper().Equals("AGROVET STORE") && !g.GlAccName.ToUpper().Equals("AGROVET SALES")).ToList();
                journalListings = await GetIncomeStatement(filter, incomeStatementAccs);
                var income = journalListings.Where(a => a.Group == "INCOME").ToList();
                var expenses = journalListings.Where(a => a.Group == "EXPENSES").ToList();
                var totalIncome = income.Sum(i => i.Cr) - income.Sum(i => i.Dr);
                var totalExpenses = expenses.Sum(e => e.Dr) - expenses.Sum(e => e.Cr);
                var profit = totalIncome - totalExpenses;
                return Json(new
                {
                    assets,
                    liabilities,
                    capitals,
                    profit
                });
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        public IActionResult SpecialPrice()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SpecialPrice([FromBody] JournalFilter filter)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var prices = await _context.SpecialPrice.Where(p => p.saccocode == sacco && p.IsFarmer == filter.IsFarmer).ToListAsync();
            if (prices.Any())
                _context.SpecialPrice.RemoveRange(prices);
            var intakes = await _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= filter.FromDate && i.TransDate <= filter.ToDate).ToListAsync();
            var farmersIntake = intakes.Where(i => (i.Description == "Intake" || i.Description == "Correction"));
            var activeFarmersNos = farmersIntake.Select(s => s.Sno.ToUpper()).Distinct();
            var days = (filter.ToDate - filter.FromDate).TotalDays + 1;
            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == sacco);
            var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco && activeFarmersNos.Contains(s.Sno.ToUpper())).ToListAsync();
            var transporters = await _context.DTransporters.Where(s => s.ParentT == sacco).ToListAsync();
            var specialPrices = new List<SpecialPrice>();
            if (filter.IsFarmer)
            {
                suppliers.ForEach(s =>
                {
                    var totalSupplied = farmersIntake.Where(i => i.Sno == s.Sno).Sum(i => i.Qsupplied);
                    var averageSupplied = totalSupplied / (decimal)days;
                    if (averageSupplied >= price.SubsidyQty)
                    {
                        specialPrices.Add(new SpecialPrice
                        {
                            Date = DateTime.Today,
                            Code = s.Sno,
                            Month = DateTime.Today.Month,
                            IsFarmer = true,
                            Quantity = totalSupplied,
                            Rate = price.SubsidyPrice,
                            Amount = totalSupplied * price.SubsidyPrice,
                            Branch = saccoBranch,
                            saccocode = sacco
                        });
                    }
                });
            }
            else
            {
                transporters.ForEach(t =>
                {
                    var totalSupplied = intakes.Where(i => i.Sno == t.TransCode).Sum(i => i.Qsupplied);
                    var averageSupplied = totalSupplied / (decimal)days;
                    if (price != null && t.TraderRate > 0 && averageSupplied >= price.SubsidyQty)
                    {
                        specialPrices.Add(new SpecialPrice
                        {
                            Date = DateTime.Today,
                            Code = t.TransCode,
                            Month = DateTime.Today.Month,
                            IsFarmer = false,
                            Quantity = totalSupplied,
                            Rate = (decimal?)t.Rate,
                            Amount = totalSupplied * (decimal?)t.Rate,
                            Branch = saccoBranch,
                            saccocode = sacco
                        });
                    }
                });

            }

            await _context.SpecialPrice.AddRangeAsync(specialPrices);
            await _context.SaveChangesAsync();

            var specials = new List<dynamic>();
            if (filter.IsFarmer)
            {
                specialPrices.ForEach(p =>
                {
                    var supplier = suppliers.FirstOrDefault(s => s.Sno == p.Code);
                    specials.Add(new
                    {
                        Name = supplier.Names,
                        Code = supplier.Sno,
                        p.Quantity,
                        p.Rate,
                        p.Amount
                    });
                });
            }
            else
            {
                specialPrices.ForEach(p =>
                {
                    var transporter = transporters.FirstOrDefault(t => t.TransCode == p.Code);
                    specials.Add(new
                    {
                        Name = transporter.TransName,
                        Code = transporter.CertNo,
                        p.Quantity,
                        p.Rate,
                        p.Amount
                    });
                });
            }

            return Json(new
            {
                specials,
            });
        }
    }
}
