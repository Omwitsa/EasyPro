﻿using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var glAccounts = _context.Glsetups.ToList();
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
            var glAccounts = _context.Glsetups.ToList();
            ViewBag.glAccounts = glAccounts;
            return View();
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
            var glAccounts = _context.Glsetups.ToList();
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
            var glsetup = _context.Glsetups.FirstOrDefault(t => t.AccNo.Trim().ToUpper().Equals(filter.AccNo.ToUpper()));
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
    }
}
