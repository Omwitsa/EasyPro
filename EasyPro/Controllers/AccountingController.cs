using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult JournalPosting()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = _context.Glsetups.Where(a => a.saccocode == sacco).ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

            return View();
        }

        [HttpPost]
        public JsonResult JournalPosting([FromBody] List<Gltransaction> transactions)
        {
            try
            {
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                transactions.ForEach(t =>
                {
                    t.AuditId = loggedInUser;
                    t.TransDate = DateTime.Today;
                    t.AuditTime = DateTime.Now;
                    t.Source = "";
                    t.TransDescript = t?.TransDescript ?? "";
                    t.Transactionno = $"{loggedInUser}{DateTime.Now}";
                    t.SaccoCode = sacco;
                });
                _context.Gltransactions.AddRange(transactions);
                _context.SaveChanges();
                _notyf.Success("Journal posted successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        public IActionResult Budget()
        {
            utilities.SetUpPrivileges(this);
            var perMonth = new string[] { "Yes", "No" };
            ViewBag.perMonth = new SelectList(perMonth);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = _context.Glsetups.Where(a => a.saccocode == sacco).ToList();
            ViewBag.glAccounts = glAccounts;
            return View();
        }

        [HttpGet]
        public JsonResult Comparison(DateTime period)
        {
            utilities.SetUpPrivileges(this);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var budgets = _context.Budgets.Where(b => b.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && b.Mmonth == period.Month && b.Yyear == period.Year).ToList();
            var comparisons = new List<ComparisonVm>();
            budgets.ForEach(b =>
            {
                var glsetup = _context.Glsetups.FirstOrDefault(g => g.AccNo.ToUpper().Equals(b.Accno.ToUpper()));
                var accName = glsetup?.GlAccName ?? "";
                var customerBalances = _context.CustomerBalances.Where(c => c.AccNo.ToUpper().Equals(b.Accno.ToUpper())
                && c.TransDate.GetValueOrDefault().Month == period.Month
                && c.TransDate.GetValueOrDefault().Year == period.Year);
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
        public JsonResult Budget([FromBody] BudgetFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var budgets = _context.Budgets.Where(b => b.Accno.ToUpper().Equals(filter.AccNo.ToUpper())
                && b.Yyear == filter.Period.Year && b.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
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
                budgets = _context.Budgets.Where(b => b.Accno.ToUpper().Equals(filter.AccNo.ToUpper())
                && b.Yyear == filter.Period.Year && b.SaccoCode.ToUpper().Equals(sacco.ToUpper()));

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

        public IActionResult GLInquiry()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = _context.Glsetups.Where(a => a.saccocode == sacco).ToList();
            ViewBag.glAccounts = glAccounts;

            return View();
        }

        [HttpPost]
        public JsonResult GLInquiry([FromBody] JournalFilter filter)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var gltransactions = _context.Gltransactions
                .Where(t => t.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate
                && (t.DrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())
                || t.CrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())));

            var debit = gltransactions.Where(t => t.DrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())).ToList();
            var credit = gltransactions.Where(t => t.CrAccNo.ToUpper().Equals(filter.AccNo.ToUpper())).ToList();
            var glsetup = _context.Glsetups.FirstOrDefault(t => t.AccNo.Trim().ToUpper().Equals(filter.AccNo.ToUpper()) && t.saccocode == sacco);
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
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public JsonResult JournalListing([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                var gltransactions = _context.Gltransactions.Where(t => t.SaccoCode == sacco 
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToList();
                
                gltransactions.ForEach(t =>
                {
                    var debtorsAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo && a.saccocode == sacco);
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
                    var creditorsAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo && a.saccocode == sacco);
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
        public JsonResult IntakeEndOfDay()
        {
            try
            {
                var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var productIntakes = _context.ProductIntake
                    .Where(i => i.TransDate == DateTime.Today && i.SaccoCode == sacco && (i.Posted == null || !(bool)i.Posted)).ToList();

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
                var gltransactions = _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= DateTime.Today && t.TransDescript == "Intake").ToList();
                gltransactions.ForEach(t =>
                {
                    var debtorssAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo && a.saccocode == sacco);
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
                    

                    var creditorsAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo && a.saccocode == sacco);
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
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public JsonResult TrialBalance([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                var gltransactions = _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToList();
                gltransactions.ForEach(t =>
                {
                    var debtorssAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo && a.saccocode == sacco);
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

                    var creditorsAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo && a.saccocode == sacco);
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
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public JsonResult IncomeStatement([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                var gltransactions = _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToList();
                gltransactions.ForEach(t =>
                {
                    var debtorssAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.DrAccNo && a.saccocode == sacco && a.GlAccType == "Income Statement");
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

                    var creditorsAcc = _context.Glsetups.FirstOrDefault(a => a.AccNo == t.CrAccNo && a.saccocode == sacco && a.GlAccType == "Income Statement");
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

        public IActionResult BalanceSheet()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        [HttpPost]
        public JsonResult BalanceSheet([FromBody] JournalFilter filter)
        {
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var journalListings = new List<JournalVm>();
                var gltransactions = _context.Gltransactions.Where(t => t.SaccoCode == sacco
                && t.TransDate >= filter.FromDate && t.TransDate <= filter.ToDate)
                    .ToList();

                var glsetups = _context.Glsetups.Where(a => a.GlAccType == "Balance Sheet" && a.saccocode == sacco).ToList();
                glsetups.ForEach(s =>
                {
                    var transaction = gltransactions.FirstOrDefault();
                    var debitAmount = gltransactions.Where(t => t.DrAccNo == s.AccNo).Sum(t => t.Amount);
                    var creditAmount = gltransactions.Where(t => t.CrAccNo == s.AccNo).Sum(t => t.Amount);
                    if (debitAmount != 0 || creditAmount != 0)
                    {
                        journalListings.Add(new JournalVm
                        {
                            GlAcc = s.AccNo,
                            TransDate = transaction.TransDate,
                            AccName = s.GlAccName,
                            Dr = debitAmount,
                            DocumentNo = transaction.DocumentNo,
                            Cr = creditAmount,
                            TransDescript = transaction.TransDescript,
                            Group = s.GlAccMainGroup,
                        });
                    }
                });

                var assets = journalListings.Where(a => a.Group == "ASSETS").ToList();
                var liabilities = journalListings.Where(a => a.Group == "LIABILITIES").ToList();
                var capitals = journalListings.Where(a => a.Group == "CAPITAL").ToList();
                return Json(new
                {
                    assets,
                    liabilities,
                    capitals
                });
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }
    }
}
