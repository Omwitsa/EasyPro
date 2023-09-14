using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
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
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                transactions.ForEach(t =>
                {
                    t.AuditId = loggedInUser;
                    t.TransDate = t.TransDate;
                    t.AuditTime = DateTime.Now;
                    t.Source = "";
                    t.TransDescript = t?.TransDescript ?? "";
                    t.Transactionno = $"{loggedInUser}{DateTime.Now}";
                    t.SaccoCode = sacco;
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
                        if (g.NormalBal.ToLower().Equals("debit"))
                            dr = g.OpeningBal;
                        if (g.NormalBal.ToLower().Equals("credit"))
                            cr = g.OpeningBal;
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = g.AccNo,
                            TransDate = filter.ToDate,
                            AccName = g.GlAccName,
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
                var journalListings = await GetIncomeStatement(filter);
                var income = journalListings.Where(a => a.Group == "INCOME").ToList();
                var expenses = journalListings.Where(a => a.Group == "EXPENSES").ToList();
                return Json(new
                {
                    income,
                    expenses
                });
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        private async Task<List<JournalVm>> GetIncomeStatement(JournalFilter filter)
        {
            var journalListings = new List<JournalVm>();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            List<Glsetup> glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco && g.GlAccType == "Income Statement").ToListAsync();
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
                        Cr = cr,
                        DocumentNo = "",
                        Dr = dr,
                        TransDescript = "Opening Bal",
                        Group = g.GlAccMainGroup,
                    });
                }
            });

            var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco
            && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                .ToListAsync();
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
                        Cr = t.Amount,
                        DocumentNo = t.DocumentNo,
                        Dr = 0,
                        TransDescript = t.TransDescript,
                        Group = creditorsAcc.GlAccMainGroup,
                    });
                }
            });

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
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                var gltransactions = await _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToListAsync();

                List<Glsetup> glsetups = await _context.Glsetups.Where(g => g.saccocode == sacco && g.GlAccType == "Balance Sheet").ToListAsync();
                glsetups.ForEach(s =>
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

                journalListings = await GetIncomeStatement(filter);
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
    }
}
