﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Linq;
using DocumentFormat.OpenXml.InkML;
using Stripe;
using EasyPro.Models.BosaModels;
using Syncfusion.EJ2.Diagrams;
using EasyPro.IProvider;
using DocumentFormat.OpenXml.Drawing.Charts;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.Wordprocessing;

namespace EasyPro.Controllers
{
    public class DPayrollsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        private readonly BosaDbContext _bosaDbContext;

        public DPayrollsController(MORINGAContext context, INotyfService notyf, BosaDbContext bosaDbContext)
        {
            _context = context;
            _notyf = notyf;
            _bosaDbContext = bosaDbContext;
            utilities = new Utilities(context);
        }

        // GET: DPayrolls
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startDate = month.AddMonths(-1);
            var endDate = month.AddDays(-1);
            var suppliers = await _context.DSuppliers.Where(d => d.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            IQueryable<DPayroll> dPayrolls = _context.DPayrolls.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.EndofPeriod == endDate);
            var payrolls = await dPayrolls.OrderBy(p => p.Bank).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
            {
                suppliers = suppliers.Where(i => i.Branch == saccoBranch).ToList();
                payrolls = payrolls.Where(i => i.Branch == saccoBranch).ToList();
            }

            var payroll = new List<PayrollVm>();
            payrolls.ForEach(p =>
            {
                var supplier = suppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(p.Sno.ToUpper()));
                payroll.Add(new PayrollVm
                {
                    Sno = supplier.Sno,
                    Names = supplier.Names,
                    PhoneNo = supplier.PhoneNo,
                    IdNo = supplier.IdNo,
                    Bank = supplier.Bcode,
                    AccNo = supplier.AccNo,
                    Branch = supplier.Branch,
                    Quantity = p.KgsSupplied,
                    GrossPay = p.Gpay,
                    Transport = p.Transport,
                    Advance = p.Advance,
                    MIDPAY = p.MIDPAY,
                    Fsa = p.Fsa, // loan
                    Others = p.Others,
                    Registration = p.Registration,
                    Netpay = p.Npay,
                    Agrovet = p.Agrovet,
                    Bonus = p.Bonus,
                    Hshares = p.Hshares,
                    CurryForward = p.CurryForward,
                    Clinical = p.CLINICAL,
                    AI = p.AI,
                    Tractor = p.Tractor,
                    Extension = p.extension,
                    SMS = p.SMS,
                });
            });

            return View(payroll);
        }

        // GET: DPayrolls/Details/5
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

            var dPayroll = await _context.DPayrolls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPayroll == null)
            {
                return NotFound();
            }

            return View(dPayroll);
        }

        // GET: DPayrolls/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DPayrolls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EndDate")] PayrollPeriod period)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var loanlastdate = DateTime.Today;
            var startDate = new DateTime(period.EndDate.Year, period.EndDate.Month, 1);
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var nextMonthStartDate = startDate.AddMonths(1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            IQueryable<DPayroll> payrolls = _context.DPayrolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            IQueryable<DTransportersPayRoll> transportersPayRolls = _context.DTransportersPayRolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            IQueryable<StandingOrder> activeorders = _context.StandingOrder.Where(o => o.SaccoCode == sacco && o.Paid < o.Amount && !o.Status);
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate
            && i.TransDate <= period.EndDate);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                payrolls = payrolls.Where(p => p.Branch == saccoBranch);
                transportersPayRolls = transportersPayRolls.Where(p => p.Branch == saccoBranch);
                productIntakeslist = productIntakeslist.Where(p => p.Branch == saccoBranch);
                activeorders = activeorders.Where(o => o.Branch == saccoBranch);
            }

            if (payrolls.Any())
                _context.DPayrolls.RemoveRange(payrolls);
            if (transportersPayRolls.Any())
                _context.DTransportersPayRolls.RemoveRange(transportersPayRolls);

            if (sacco != StrValues.Mburugu && sacco != StrValues.Slopes)
            {
                IQueryable<ProductIntake> transpoterIntakes = productIntakeslist.Where(i => i.Description.ToLower().Equals("transport"));
                if (transpoterIntakes.Any())
                    _context.ProductIntake.RemoveRange(transpoterIntakes);

                IQueryable<ProductIntake> checkifanydeduction = productIntakeslist.Where(n => n.Description.ToLower().Contains("midpay"));
                if (checkifanydeduction.Any())
                    _context.ProductIntake.RemoveRange(checkifanydeduction);

                IQueryable<ProductIntake> deletesubsidy = productIntakeslist.Where(i => i.ProductType == "SUBSIDY");
                if (deletesubsidy.Any())
                    _context.ProductIntake.RemoveRange(deletesubsidy);
            }

            if (StrValues.Slopes == sacco)
            {
                var carryForwardList = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.Description == "Carry Forward" && (i.TransDate == monthsLastDate || i.TransDate == nextMonthStartDate));
                if (carryForwardList.Any())
                    _context.ProductIntake.RemoveRange(carryForwardList);
            }

            IQueryable<ProductIntake> deletestandingorder = productIntakeslist.Where(i => i.Remarks == "Standing Order");
            if (deletestandingorder.Any())
                _context.ProductIntake.RemoveRange(deletestandingorder);

            IQueryable<DPreSet> preSets = _context.d_PreSets.Where(b => b.saccocode == sacco);
            if (user.AccessLevel == AccessLevel.Branch)
                preSets = preSets.Where(p => p.BranchCode == saccoBranch);
            var selectdistinctdedname = await preSets.Select(b => b.Deduction.ToUpper()).Distinct().ToListAsync();
            IQueryable<ProductIntake> deletesdefaultded = productIntakeslist.Where(i => selectdistinctdedname.Contains(i.ProductType.ToUpper()));
            if (StrValues.Elburgon == sacco)
                deletesdefaultded = deletesdefaultded.Where(d => d.AuditId != "admin");

            if (deletesdefaultded.Any())
                _context.ProductIntake.RemoveRange(deletesdefaultded);

            IQueryable<Gltransaction> checkgls = _context.Gltransactions
               .Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= startDate && f.TransDate <= period.EndDate
               && (f.TransDescript.ToUpper().Equals("Bonus".ToUpper()) || f.TransDescript.ToUpper().Equals("Shares".ToUpper())));
            if (user.AccessLevel == AccessLevel.Branch)
                checkgls = checkgls.Where(p => p.Branch == saccoBranch);
            if (checkgls.Any())
                _context.Gltransactions.RemoveRange(checkgls);

            IQueryable<DShare> checksharespaid = _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                            && f.TransDate >= startDate && f.TransDate <= period.EndDate && f.Type.ToUpper().Equals("SHARES")
                            && f.Pmode == "Checkoff");
            if (user.AccessLevel == AccessLevel.Branch)
                checksharespaid = checksharespaid.Where(p => p.Branch == saccoBranch);
            if (checksharespaid.Any())
                _context.DShares.RemoveRange(checksharespaid);

            var intakeList = await productIntakeslist.ToListAsync();
            if (sacco != StrValues.Mburugu && sacco != StrValues.Slopes)
                await ConsolidateTranspoterIntakes(startDate, period.EndDate, intakeList);

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if (StrValues.Elburgon != sacco)
            {
                var preSet = preSets.FirstOrDefault(n => n.saccocode == sacco);
                //update default deductions if any like bonus
                if (preSet != null)
                    await CalcDefaultdeductions(startDate, period.EndDate, intakeList);
            }
            else
            {
                await CalcDefaultdeductions(startDate, period.EndDate, intakeList);
            }

            var societyStandingOrders = intakeList.Where(i => i.Remarks == "Society Standing Order").ToList();
            var orderDeductions = new List<ProductIntake>();
            activeorders.ForEach(o =>
            {
                o.Sno = o?.Sno ?? "";
                o.Description = o?.Description ?? "";
                if (!societyStandingOrders.Any(i => i.Sno.ToUpper().Equals(o.Sno.ToUpper()) && i.Description.ToUpper().Equals(o.Description.ToUpper())))
                {
                    var balance = o.Amount - o.Paid;
                    o.Installment = o.Installment > balance ? balance : o.Installment;
                    orderDeductions.Add(new ProductIntake
                    {
                        Sno = o.Sno,
                        TransDate = monthsLastDate,
                        TransTime = DateTime.Now.TimeOfDay,
                        ProductType = o.Description,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = 0,
                        DR = o.Installment,
                        Balance = 0,
                        Description = o.Description,
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Society Standing Order",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });

                    o.Paid += o.Installment;
                }
            });

            if (orderDeductions.Any())
                await _context.ProductIntake.AddRangeAsync(orderDeductions);
            await _context.SaveChangesAsync();
            productIntakeslist = _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate
            && i.TransDate <= period.EndDate);
            var dcodes = await _context.DDcodes.Where(c => c.Description.ToLower().Contains("advance") || c.Description.ToLower().Equals("transport")
               || c.Description.ToLower().Equals("agrovet") || c.Description.ToLower().Equals("store") || c.Description.ToLower().Equals("eclof")
               || c.Description.ToLower().Equals("bonus") || c.Description.ToLower().Equals("shares") || c.Description.ToLower().Equals("society shares")
               || c.Description.ToLower().Equals("loan") || c.Description.ToLower().Equals("carry forward") || c.Description.ToLower().Equals("clinical")
               || c.Description.ToLower().Equals("a.i") || c.Description.ToLower().Equals("ai") || c.Description.ToLower().Equals("tractor")
               || c.Description.ToLower().Equals("sms") || c.Description.ToLower().Equals("extension work")
               || c.Description.ToLower().Equals("registration") || c.Description.ToUpper().Contains("MAENDELEO")
               || c.Description.ToUpper().Equals("INST ADV") || c.Description.ToUpper().Equals("KIIGA")
               || c.Description.ToUpper().Equals("KIROHA DAIRY") || c.Description.ToUpper().Equals("NOV OVERPAYMENT")
               || c.Description.ToUpper().Equals("MILK RECOVERY") || c.Description.ToLower().Equals("midpay")
               ).Select(c => c.Description.ToLower()).ToListAsync();

            var suppliers = await _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            //if (user.AccessLevel == AccessLevel.Branch)
            //    suppliers = suppliers.Where(t => t.Branch == saccoBranch).ToList();

            var shareCode = "S03";
            var savingsCode = "S02";
            decimal? maxShares = 5500;
            var loans = await _bosaDbContext.LOANBAL.Where(l => l.Balance > 0 && l.LastDate <= loanlastdate && l.Companycode == StrValues.SlopesCode).ToListAsync();
            var saccoStandingOrders = _bosaDbContext.CONTRIB_standingOrder.Where(o => o.CompanyCode == StrValues.SlopesCode);
            var contribs = await _bosaDbContext.CONTRIB.Where(c => c.CompanyCode == StrValues.SlopesCode).ToListAsync();
            var listSaccoLoans = await _context.SaccoLoans.Where(l => l.Saccocode == sacco && l.TransDate == monthsLastDate).ToListAsync();
            var listSaccoShares = await _context.SaccoShares.Where(s => s.Saccocode == sacco && s.TransDate == monthsLastDate).ToListAsync();

            var saccoLoansProcessed = listSaccoLoans.Any();
            var saccoSharesProcessed = listSaccoShares.Any();
            var midMonthDeductions = await productIntakeslist.Where(n => n.TransDate == period.EndDate && n.Description.ToLower().Contains("midpay")).ToListAsync();
            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == sacco);
            var supplierNos = suppliers.Select(s => s.Sno);
            var supplierIntakes = await productIntakeslist.Where(p => supplierNos.Contains(p.Sno)).ToListAsync();
            //var supplierIntakes = await productIntakeslist.Where(p => p.Sno=="3737").ToListAsync();
            var intakes = supplierIntakes.GroupBy(p => p.Sno.ToUpper()).ToList();
            var dPayrolls = new List<DPayroll>();
            var curriedForwardProducts = new List<ProductIntake>();
            intakes.ForEach(async p =>
            {
                var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                var transport = p.Where(k => k.Description.Equals("Transport"));
                var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet") || k.ProductType.ToLower().Contains("store"));
                var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                var shares = p.Where(k => k.ProductType.ToLower().Contains("shares") || k.ProductType.ToLower().Contains("society shares"));
                var loan = p.Where(k => k.ProductType.ToLower().Contains("loan"));
                var carryforward = p.Where(k => k.ProductType.ToLower().Contains("carry forward"));
                var clinical = p.Where(k => k.ProductType.ToLower().Contains("clinical"));
                var ai = p.Where(k => (k.ProductType.ToLower().Contains("ai") || k.ProductType.ToLower().Contains("a.i")));
                var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                var extension = p.Where(k => k.ProductType.ToLower().Contains("extension work"));
                var SMS = p.Where(k => k.ProductType.ToLower().Contains("sms"));
                var registration = p.Where(k => k.ProductType.ToLower().Contains("registration"));
                var MIDPAY = p.Where(k => k.ProductType.ToLower().Contains("midpay"));
                var instantAdvance = p.Where(k => k.ProductType.ToUpper().Contains("INST ADV"));
                var kiiga = p.Where(k => k.ProductType.ToUpper().Contains("KIIGA"));
                var kiroha = p.Where(k => k.ProductType.ToUpper().Contains("KIROHA DAIRY"));
                var overpayment = p.Where(k => k.ProductType.ToUpper().Contains("NOV OVERPAYMENT"));
                var milkRecovery = p.Where(k => k.ProductType.ToUpper().Contains("MILK RECOVERY"));
                var ECLOF = p.Where(k => k.ProductType.ToLower().Contains("eclof"));
                var saccoDed = p.Where(k => k.ProductType.ToUpper().Contains("MAENDELEO"));
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                var Others = p.Where(k => !dcodes.Contains(k.ProductType.ToLower()) && !k.ProductType.ToUpper().Equals("AGROVET")
                && !k.ProductType.ToUpper().Equals("MILK") && k.TransactionType == TransactionType.Deduction);
                var supplier = suppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(p.Key));
                if (supplier != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    var credited = p.Sum(s => s.CR);
                    var framersTotal = p.Where(s => s.Description == "Intake" || s.Description == "Correction").Sum(s => s.Qsupplied);
                    decimal? subsidy = 0;
                    if (StrValues.Slopes == sacco)
                    {
                        credited = framersTotal * price.Price;
                        var daysInMonth = DateTime.DaysInMonth(period.EndDate.Year, period.EndDate.Month);
                        var averageSupplied = framersTotal / daysInMonth;

                        if (price != null && averageSupplied >= price.SubsidyQty)
                            subsidy += framersTotal * price.SubsidyPrice;
                    }

                    var grossPay = credited + subsidy;
                    //add correct gross and subsidy for other societies 
                    if (StrValues.Slopes != sacco)
                    {
                        var getCrDr = p.Where(s => s.Description == "Intake" || s.Description == "Correction");
                        var getDebitsCrDr = p.Where(s => s.Description != "Intake" || s.Description != "Correction");
                        grossPay = getCrDr.Where(n => n.CR > 0).Sum(x => x.CR) - getCrDr.Where(n => n.DR > 0).Sum(x => x.DR);
                        debits = getDebitsCrDr.Where(n => n.DR > 0).Sum(x => x.DR) - getDebitsCrDr.Where(n => n.CR > 0).Sum(x => x.CR);
                        subsidy = 0;
                    }

                    if (grossPay > 0 && (supplier.TransCode == "Weekly" || (supplier.TransCode == "Monthly" && period.EndDate == monthsLastDate)))
                    {
                        //var netPay = grossPay - (debits + Tot);
                        var payroll = new DPayroll
                        {
                            Sno = supplier.Sno,
                            Subsidy = subsidy,
                            Gpay = grossPay,
                            KgsSupplied = (double?)milk.Sum(s => s.Qsupplied),
                            Yyear = period.EndDate.Year,
                            Mmonth = period.EndDate.Month,
                            Bank = supplier.Bcode,
                            AccountNumber = supplier.AccNo,
                            Bbranch = supplier.Bbranch,
                            PhoneNo = supplier.PhoneNo,
                            IdNo = supplier.IdNo,
                            EndofPeriod = period.EndDate,
                            SaccoCode = sacco,
                            Auditid = loggedInUser,
                            Branch = supplier.Branch,
                            Advance = 0,
                            CurryForward = 0,
                            Others = 0,
                            CLINICAL = 0,
                            AI = 0,
                            Tractor = 0,
                            Transport = 0,
                            Registration = 0,
                            extension = 0,
                            SMS = 0,
                            Agrovet = 0,
                            Bonus = 0,
                            Fsa = 0,
                            Hshares = 0,
                            MIDPAY = 0,
                            ECLOF = 0,
                            saccoDed = 0,
                            Tdeductions = 0,
                            Npay = 0,
                            SACCO_SHARES = 0,
                            SACCO_SAVINGS = 0,
                            INST_ADVANCE = 0,
                            KIIGA = 0,
                            KIROHA = 0,
                            NOV_OVPMNT = 0,
                            MILK_RECOVERY = 0,
                        };

                        decimal? netPay = grossPay;
                        decimal? carriedForwardValue = 0;
                        var carryfw = (carryforward.Sum(s => s.DR) - carryforward.Sum(s => s.CR));
                        payroll.CurryForward = carryfw;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && carryfw > 0 && netPay < carryfw)
                            {
                                payroll.CurryForward = netPay;
                                carriedForwardValue = carryfw - netPay;
                            }
                            if (netPay < 0 && carryfw > 0)
                            {
                                payroll.CurryForward = 0;
                                carriedForwardValue = carryfw;
                            }
                        }

                        netPay -= carryfw;
                        if (netPay < 0 && carryfw > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Carry Forward",
                                DR = carriedForwardValue,
                            });

                        var regis = (registration.Sum(s => s.DR) - registration.Sum(s => s.CR));
                        payroll.Registration = regis;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && regis > 0 && netPay < regis)
                            {
                                payroll.Registration = netPay;
                                carriedForwardValue = regis - netPay;
                            }
                            if (netPay < 0 && regis > 0)
                            {
                                payroll.Registration = 0;
                                carriedForwardValue = regis;
                            }
                        }

                        netPay -= regis;

                        if (netPay < 0 && regis > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Registration",
                                DR = carriedForwardValue,
                            });


                        var shar = (shares.Sum(s => s.DR) - shares.Sum(s => s.CR));
                        payroll.Hshares = shar;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && shar > 0 && netPay < shar)
                            {
                                payroll.Hshares = netPay;
                                carriedForwardValue = shar - netPay;
                            }
                            if (netPay < 0 && regis > 0)
                            {
                                payroll.Hshares = 0;
                                carriedForwardValue = shar;
                            }
                        }

                        netPay -= shar;
                        if (netPay < 0 && shar > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Society Shares",
                                DR = carriedForwardValue,
                            });

                        var kiigaContrib = kiiga.Sum(s => s.DR) - kiiga.Sum(s => s.CR);
                        payroll.KIIGA = kiigaContrib;
                        carriedForwardValue = 0;
                        if (netPay > 0 && kiigaContrib > 0 && netPay < kiigaContrib)
                        {
                            payroll.KIIGA = netPay;
                            carriedForwardValue = kiigaContrib - netPay;
                        }
                        if (netPay < 0 && kiigaContrib > 0)
                        {
                            payroll.KIIGA = 0;
                            carriedForwardValue = kiigaContrib;
                        }

                        netPay -= kiigaContrib;
                        if (netPay < 0 && regis > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "KIIGA",
                                DR = carriedForwardValue,
                            });

                        var kirohaContrib = kiroha.Sum(s => s.DR) - kiroha.Sum(s => s.CR);
                        payroll.KIROHA = kirohaContrib;
                        carriedForwardValue = 0;
                        if (netPay > 0 && kirohaContrib > 0 && netPay < kirohaContrib)
                        {
                            payroll.KIROHA = netPay;
                            carriedForwardValue = kirohaContrib - netPay;
                        }
                        if (netPay < 0 && kirohaContrib > 0)
                        {
                            payroll.KIROHA = 0;
                            carriedForwardValue = kirohaContrib;
                        }

                        netPay -= kirohaContrib;
                        if (netPay < 0 && regis > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "KIROHA DAIRY",
                                DR = carriedForwardValue,
                            });

                        var AIs = (ai.Sum(s => s.DR) - ai.Sum(s => s.CR));
                        payroll.AI = AIs;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && AIs > 0 && netPay < AIs)
                            {
                                payroll.AI = netPay;
                                carriedForwardValue = AIs - netPay;
                            }
                            if (netPay < 0 && AIs > 0)
                            {
                                payroll.AI = 0;
                                carriedForwardValue = AIs;
                            }
                        }

                        netPay -= AIs;
                        if (netPay < 0 && AIs > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Ai",
                                DR = carriedForwardValue,
                            });

                        var agro = (agrovet.Sum(s => s.DR) - agrovet.Sum(s => s.CR));
                        payroll.Agrovet = agro;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && agro > 0 && netPay < agro)
                            {
                                payroll.Agrovet = netPay;
                                carriedForwardValue = agro - netPay;
                            }
                            if (netPay < 0 && agro > 0)
                            {
                                payroll.Agrovet = 0;
                                carriedForwardValue = agro;
                            }
                        }

                        netPay -= agro;
                        if (netPay < 0 && agro > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Agrovet",
                                DR = carriedForwardValue,
                            });

                        var novContrib = overpayment.Sum(s => s.DR) - overpayment.Sum(s => s.DR);
                        payroll.NOV_OVPMNT = novContrib;
                        carriedForwardValue = 0;
                        if (netPay > 0 && novContrib > 0 && netPay < novContrib)
                        {
                            payroll.NOV_OVPMNT = netPay;
                            carriedForwardValue = novContrib - netPay;
                        }
                        if (netPay < 0 && agro > 0)
                        {
                            payroll.NOV_OVPMNT = 0;
                            carriedForwardValue = novContrib;
                        }

                        if (netPay < 0 && novContrib > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "NOV OVERPAYMENT",
                                DR = carriedForwardValue,
                            });


                        var adva = (advance.Sum(s => s.DR) - advance.Sum(s => s.CR));
                        payroll.Advance = adva;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && adva > 0 && netPay < adva)
                            {
                                payroll.Advance = netPay;
                                carriedForwardValue = adva - netPay;
                            }
                            if (netPay < 0 && adva > 0)
                            {
                                payroll.Advance = 0;
                                carriedForwardValue = adva;
                            }
                        }

                        netPay -= adva;
                        if (netPay < 0 && adva > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Advance",
                                DR = carriedForwardValue,
                            });

                        var insAdv = instantAdvance.Sum(s => s.DR) - instantAdvance.Sum(s => s.CR);
                        payroll.INST_ADVANCE = insAdv;
                        carriedForwardValue = 0;
                        if (netPay > 0 && insAdv > 0 && netPay < insAdv)
                        {
                            payroll.INST_ADVANCE = netPay;
                            carriedForwardValue = insAdv - netPay;
                        }
                        if (netPay < 0 && insAdv > 0)
                        {
                            payroll.INST_ADVANCE = 0;
                            carriedForwardValue = insAdv;
                        }

                        if (netPay < 0 && insAdv > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "INST ADV",
                                DR = carriedForwardValue,
                            });

                        var trans = transport.Sum(s => s.DR) - transport.Sum(s => s.CR);
                        payroll.Transport = trans;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && trans > 0 && netPay < trans)
                            {
                                payroll.Transport = netPay;
                                carriedForwardValue = trans - netPay;
                            }
                            if (netPay < 0 && trans > 0)
                            {
                                payroll.Transport = 0;
                                carriedForwardValue = trans;
                            }
                        }

                        netPay -= trans;
                        if (netPay < 0 && trans > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Transport",
                                DR = carriedForwardValue,
                            });

                        var clini = (clinical.Sum(s => s.DR) - clinical.Sum(s => s.CR));
                        payroll.CLINICAL = clini;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && clini > 0 && netPay < clini)
                            {
                                payroll.CLINICAL = netPay;
                                carriedForwardValue = clini - netPay;
                            }
                            if (netPay < 0 && clini > 0)
                            {
                                payroll.CLINICAL = 0;
                                carriedForwardValue = clini;
                            }
                        }

                        netPay -= clini;
                        if (netPay < 0 && clini > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Clinical",
                                DR = carriedForwardValue,
                            });

                        var Tract = (tractor.Sum(s => s.DR) - tractor.Sum(s => s.CR));
                        payroll.Tractor = Tract;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && Tract > 0 && netPay < Tract)
                            {
                                payroll.Tractor = netPay;
                                carriedForwardValue = Tract - netPay;
                            }
                            if (netPay < 0 && Tract > 0)
                            {
                                payroll.Tractor = 0;
                                carriedForwardValue = Tract;
                            }
                        }

                        netPay -= Tract;
                        if (netPay < 0 && Tract > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Tractor",
                                DR = carriedForwardValue,
                            });

                        var extensionDed = extension.Sum(s => s.DR) - extension.Sum(s => s.CR);
                        payroll.extension = extensionDed;
                        carriedForwardValue = 0;
                        if (netPay > 0 && extensionDed > 0 && netPay < extensionDed)
                        {
                            payroll.extension = netPay;
                            carriedForwardValue = extensionDed - netPay;
                        }
                        if (netPay < 0 && Tract > 0)
                        {
                            payroll.extension = 0;
                            carriedForwardValue = extensionDed;
                        }

                        netPay -= extensionDed;
                        if (netPay < 0 && extensionDed > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Extension Work",
                                DR = carriedForwardValue,
                            });

                        var smsDed = SMS.Sum(s => s.DR) - SMS.Sum(s => s.CR);
                        payroll.SMS = smsDed;
                        carriedForwardValue = 0;
                        if (netPay > 0 && smsDed > 0 && netPay < smsDed)
                        {
                            payroll.SMS = netPay;
                            carriedForwardValue = smsDed - netPay;
                        }
                        if (netPay < 0 && smsDed > 0)
                        {
                            payroll.SMS = 0;
                            carriedForwardValue = smsDed;
                        }

                        netPay -= smsDed;
                        if (netPay < 0 && smsDed > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "SMS",
                                DR = carriedForwardValue,
                            });

                        var bonusDed = bonus.Sum(s => s.DR) - bonus.Sum(s => s.CR);
                        payroll.Bonus = bonusDed;
                        carriedForwardValue = 0;
                        if (netPay > 0 && bonusDed > 0 && netPay < bonusDed)
                        {
                            payroll.Bonus = netPay;
                            carriedForwardValue = bonusDed - netPay;
                        }
                        if (netPay < 0 && bonusDed > 0)
                        {
                            payroll.Bonus = 0;
                            carriedForwardValue = bonusDed;
                        }

                        netPay -= bonusDed;
                        if (netPay < 0 && bonusDed > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Bonus",
                                DR = carriedForwardValue,
                            });

                        var midPayDed = MIDPAY.Sum(s => s.DR) - MIDPAY.Sum(s => s.CR);
                        payroll.MIDPAY = midPayDed;
                        carriedForwardValue = 0;
                        if (netPay > 0 && midPayDed > 0 && netPay < midPayDed)
                        {
                            payroll.Bonus = netPay;
                            carriedForwardValue = midPayDed - netPay;
                        }
                        if (netPay < 0 && midPayDed > 0)
                        {
                            payroll.Bonus = 0;
                            carriedForwardValue = midPayDed;
                        }

                        netPay -= midPayDed;
                        if (netPay < 0 && midPayDed > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "midpay",
                                DR = carriedForwardValue,
                            });

                        var ECLO = (ECLOF.Sum(s => s.DR) - ECLOF.Sum(s => s.CR));
                        payroll.ECLOF = ECLO;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && ECLO > 0 && netPay < ECLO)
                            {
                                payroll.ECLOF = netPay;
                                carriedForwardValue = ECLO - netPay;
                            }
                            if (netPay < 0 && ECLO > 0)
                            {
                                payroll.ECLOF = 0;
                                carriedForwardValue = ECLO;
                            }
                        }

                        netPay -= ECLO;
                        if (netPay < 0 && ECLO > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Eclof",
                                DR = carriedForwardValue,
                            });

                        var Maendeleo = (saccoDed.Sum(s => s.DR) - saccoDed.Sum(s => s.CR));
                        payroll.saccoDed = Maendeleo;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && Maendeleo > 0 && netPay < Maendeleo)
                            {
                                payroll.saccoDed = netPay;
                                carriedForwardValue = Maendeleo - netPay;
                            }
                            if (netPay < 0 && Maendeleo > 0)
                            {
                                payroll.saccoDed = 0;
                                carriedForwardValue = Maendeleo;
                            }
                        }

                        netPay -= Maendeleo;
                        if (netPay < 0 && Maendeleo > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "MAENDELEO",
                                DR = carriedForwardValue,
                            });

                        var milk_recovery = milkRecovery.Sum(s => s.DR) - milkRecovery.Sum(s => s.CR);
                        payroll.MILK_RECOVERY = milk_recovery;
                        carriedForwardValue = 0;
                        if (netPay > 0 && milk_recovery > 0 && netPay < milk_recovery)
                        {
                            payroll.MILK_RECOVERY = netPay;
                            carriedForwardValue = milk_recovery - netPay;
                        }
                        if (netPay < 0 && milk_recovery > 0)
                        {
                            payroll.MILK_RECOVERY = 0;
                            carriedForwardValue = milk_recovery;
                        }

                        netPay -= milk_recovery;
                        if (netPay < 0 && milk_recovery > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "MILK RECOVERY",
                                DR = carriedForwardValue,
                            });

                        var other = (Others.Sum(s => s.DR) - Others.Sum(s => s.CR));
                        payroll.Others = other;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && other > 0 && netPay < other)
                            {
                                payroll.Others = netPay;
                                carriedForwardValue = other - netPay;
                            }
                            if (netPay < 0 && other > 0)
                            {
                                payroll.Others = 0;
                                carriedForwardValue = other;
                            }
                        }

                        netPay -= other;
                        if (netPay < 0 && other > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Others",
                                DR = carriedForwardValue,
                            });

                        var memberLoans = loan.Sum(s => s.DR) - loan.Sum(s => s.CR);
                        payroll.Fsa = memberLoans;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && memberLoans > 0 && netPay < memberLoans)
                            {
                                payroll.Fsa = netPay;
                                carriedForwardValue = memberLoans - netPay;
                            }
                            if (netPay < 0 && memberLoans > 0)
                            {
                                payroll.Fsa = 0;
                                carriedForwardValue = memberLoans;
                            }
                        }

                        netPay -= memberLoans;
                        if (netPay < 0 && memberLoans > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = supplier.Sno,
                                ProductType = "Loan",
                                DR = carriedForwardValue,
                            });

                        if (StrValues.Slopes == sacco)
                        {
                            var advanceCode = "L01";
                            var normalCode = "L02";
                            var insAdvCode = "L03";
                            var farmerLoans = loans.Where(l => l.MemberNo.ToUpper().Equals(supplier.Sno.ToUpper())).ToList();
                            //var types = _bosaDbContext.LOANTYPE.Where(t => t.CompanyCode == StrValues.SlopesCode).ToList();
                            if (!saccoLoansProcessed)
                            {
                                farmerLoans.ForEach(l =>
                                {
                                    if (l.Balance > 0 && netPay > 0 && l.Installments > 0)
                                    {
                                        var installments = l.Balance < l.Installments ? l.Balance : l.Installments;
                                        installments = netPay > installments ? installments : netPay;
                                        listSaccoLoans.Add(new SaccoLoans
                                        {
                                            LoanNo = l.LoanNo,
                                            LoanCode = l.LoanCode,
                                            Sno = l.MemberNo,
                                            Amount = installments,
                                            TransDate = monthsLastDate,
                                            AuditDate = DateTime.Now,
                                            Saccocode = sacco,
                                            AuditId = loggedInUser
                                        });
                                    }
                                });
                            }

                            listSaccoLoans.Where(l => l.Sno.ToUpper().Equals(supplier.Sno.ToUpper())).ForEach(l =>
                            {
                                if (l.LoanCode == normalCode)
                                    payroll.Fsa += l.Amount;
                                if (l.LoanCode == advanceCode)
                                    payroll.Advance += l.Amount;
                                if (l.LoanCode == insAdvCode)
                                    payroll.INST_ADVANCE += l.Amount;
                                netPay -= l.Amount;
                            });

                            var saccoStandingOrder = saccoStandingOrders.FirstOrDefault(o => o.MemberNo.ToUpper().Equals(supplier.Sno.ToUpper()));
                            if (saccoStandingOrder != null && !saccoSharesProcessed)
                            {
                                decimal? contributedShares = contribs.FirstOrDefault(s => s.MemberNo.ToUpper().Equals(supplier.Sno.ToUpper()) && s.Sharescode == shareCode)?.Amount ?? 0;
                                if (contributedShares < maxShares)
                                {
                                    var actualContributedShares = netPay > saccoStandingOrder.Installment ? saccoStandingOrder.Installment : netPay;
                                    actualContributedShares = actualContributedShares > 0 ? actualContributedShares : 0;
                                    var currentshares = contributedShares + actualContributedShares;
                                    if (currentshares > maxShares)
                                    {
                                        actualContributedShares = maxShares - contributedShares;
                                        var actualSavings = currentshares - maxShares;
                                        listSaccoShares.Add(new SaccoShares
                                        {
                                            SharesCode = savingsCode,
                                            Sno = saccoStandingOrder.MemberNo,
                                            Amount = actualSavings,
                                            TransDate = monthsLastDate,
                                            AuditDate = DateTime.Now,
                                            Saccocode = sacco,
                                            AuditId = loggedInUser
                                        });
                                    }

                                    if (actualContributedShares > 0)
                                        listSaccoShares.Add(new SaccoShares
                                        {
                                            SharesCode = shareCode,
                                            Sno = saccoStandingOrder.MemberNo,
                                            Amount = actualContributedShares,
                                            TransDate = monthsLastDate,
                                            AuditDate = DateTime.Now,
                                            Saccocode = sacco,
                                            AuditId = loggedInUser
                                        });
                                }
                                else
                                {
                                    var actualSavings = netPay > saccoStandingOrder.Installment ? saccoStandingOrder.Installment : netPay;
                                    actualSavings = actualSavings > 0 ? actualSavings : 0;
                                    listSaccoShares.Add(new SaccoShares
                                    {
                                        SharesCode = savingsCode,
                                        Sno = saccoStandingOrder.MemberNo,
                                        Amount = actualSavings,
                                        TransDate = monthsLastDate,
                                        AuditDate = DateTime.Now,
                                        Saccocode = sacco,
                                        AuditId = loggedInUser
                                    });
                                }
                            }
                        }

                        listSaccoShares.Where(l => l.Sno.ToUpper().Equals(supplier.Sno.ToUpper())).ForEach(l =>
                        {
                            if (l.SharesCode == shareCode)
                                payroll.SACCO_SHARES += l.Amount;
                            if (l.SharesCode == savingsCode)
                                payroll.SACCO_SAVINGS += l.Amount;

                            netPay -= l.Amount;
                        });

                        payroll.Tdeductions = grossPay - netPay;
                        if (StrValues.Slopes == sacco)
                            payroll.Tdeductions = payroll.Tdeductions > grossPay ? grossPay : payroll.Tdeductions;
                        //netPay -= debits;
                        payroll.Npay = netPay;
                        dPayrolls.Add(payroll);
                        midMonthDeductions = midMonthDeductions.Where(n => n.Sno.ToUpper().Equals(supplier.Sno.ToUpper().ToString())).ToList();
                        if (!midMonthDeductions.Any() && period.EndDate != monthsLastDate)
                            _context.ProductIntake.Add(new ProductIntake
                            {
                                Sno = supplier.Sno.ToUpper().ToString(),
                                TransDate = period.EndDate,
                                TransTime = DateTime.Now.TimeOfDay,
                                ProductType = "midpay",
                                Qsupplied = 0,
                                Ppu = 0,
                                CR = 0,
                                DR = p.Sum(s => s.CR) - debits - payroll.Tdeductions,
                                Balance = 0,
                                Description = "midpay",
                                TransactionType = TransactionType.Deduction,
                                Remarks = "",
                                AuditId = loggedInUser,
                                Auditdatetime = DateTime.Now,
                                Branch = supplier.Branch,
                                SaccoCode = sacco,
                                DrAccNo = "0",
                                CrAccNo = "0",
                            });
                    }
                }
            });
            if (dPayrolls.Any())
            {
                await _context.DPayrolls.AddRangeAsync(dPayrolls);
                await _context.SaveChangesAsync();
            }

            IQueryable<DTransporter> transporters = _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper()));
            //if (user.AccessLevel == AccessLevel.Branch)
            //    transporters = transporters.Where(p => p.Tbranch == saccoBranch);

            var transpoterCodes = await transporters.Select(s => s.TransCode.Trim().ToUpper()).ToListAsync();
            var productIntakes = await productIntakeslist.Where(p => transpoterCodes.Contains(p.Sno.Trim().ToUpper())).ToListAsync();
            intakes = productIntakes.GroupBy(p => p.Sno.Trim().ToUpper()).ToList();
            var dTransportersPayRolls = new List<DTransportersPayRoll>();
            intakes.ForEach(p =>
            {
                var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                var transport = p.Where(k => k.ProductType.ToLower().Contains("transport"));
                var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet") || k.ProductType.ToLower().Contains("store"));
                var variance = p.Where(k => k.ProductType.ToLower().Contains("variance"));
                var Others = p.Where(k => k.ProductType.ToLower().Contains("others"));
                var MIDPAY = p.Where(k => k.ProductType.ToLower().Contains("midpay"));
                var clinical = p.Where(k => k.ProductType.ToLower().Contains("clinical"));
                var carryforward = p.Where(k => k.ProductType.ToLower().Contains("carry forward"));
                var ai = p.Where(k => (k.ProductType.ToLower().Contains("ai") || k.ProductType.ToLower().Contains("a.i")));
                var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                var loan = p.Where(k => k.ProductType.ToLower().Contains("loan"));
                var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                var shares = p.Where(k => k.ProductType.ToLower().Contains("shares") || k.ProductType.ToLower().Contains("society shares"));
                var extension = p.Where(k => k.ProductType.ToLower().Contains("extension work"));
                var instantAdvance = p.Where(k => k.ProductType.ToUpper().Contains("INST ADV"));
                var SMS = p.Where(k => k.ProductType.ToLower().Contains("sms"));
                var ECLOF = p.Where(k => k.ProductType.ToLower().Contains("eclof"));
                var saccoDed = p.Where(k => k.ProductType.ToUpper().Contains("MAENDELEO"));
                var milkRecovery = p.Where(k => k.ProductType.ToUpper().Contains("MILK RECOVERY"));
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                //var payroll = new DTransportersPayRoll();
                var transporter = transporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(p.Key.ToUpper()));
                if (transporter != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    var amount = (p.Where(K => K.ProductType == "Transport").Sum(s => s.CR) - p.Where(K => K.ProductType == "Transport").Sum(s => s.DR));
                    if (StrValues.Mburugu == sacco)
                        amount = p.Sum(s => s.CR);

                    var totalSupplied = p.Sum(s => s.Qsupplied);
                    decimal subsidy = 0;
                    decimal selfkgspertransporter = 0;
                    if (StrValues.Slopes == sacco)
                    {
                        amount = totalSupplied * (decimal)transporter.Rate;
                        var daysInMonth = DateTime.DaysInMonth(period.EndDate.Year, period.EndDate.Month);
                        //var averageSupplied = totalSupplied / daysInMonth;
                        //CONFIRM IF HAS THE TARGETED KGS TO BE GIVEN TRADER FEE IF TRANSPORTER
                        if (transporter.SlopesIDNo != null)
                        {
                            var checkifgettraderfee = _context.DSuppliers.FirstOrDefault(m => m.Scode == sacco
                            && m.Sno.Trim() == transporter.SlopesIDNo.Trim() );
                            if (checkifgettraderfee != null)
                            {
                                var framersTotal = payrolls.FirstOrDefault(b => b.Sno.Trim().ToUpper()
                                .Equals(checkifgettraderfee.Sno.Trim().ToUpper()));
                                selfkgspertransporter = (decimal)framersTotal.KgsSupplied;

                                transporter.TraderRate = transporter?.TraderRate ?? 0;
                                if (selfkgspertransporter > 0)
                                {
                                    //GET OTHER MEMBERS KGS productIntakeslist
                                    var getsnoreceipt = productIntakeslist.Where(n => n.Sno.Trim().ToUpper()
                                    .Equals(checkifgettraderfee.Sno.Trim().ToUpper()) && n.Description == "Transport")
                                    .ToList().Select(b=>b.Remarks)
                                    .Distinct();
                                   // var othermemberskg = productIntakeslist.Where(k => k.Sno.Trim().ToUpper()
                                   //.Equals(transporter.TransCode.Trim().ToUpper()) && (getsnoreceipt.Contains(k.Remarks)
                                   //|| checkifgettraderfee.Sno.Trim().ToUpper().Contains(k.Remarks))
                                   //&& k.Description == "Transport").ToList();
                                   // //|| !checkifgettraderfee.Sno.Trim().ToUpper().Contains(k.Remarks)

                                   // var va = othermemberskg.Sum(c => c.Qsupplied);
                                    var othermemberskgs = productIntakeslist.Where(k => k.Sno.Trim().ToUpper()
                                    .Equals(transporter.TransCode.Trim().ToUpper()) && !getsnoreceipt.Contains(k.Remarks)
                                    && !k.Remarks.Contains(checkifgettraderfee.Sno.Trim().ToUpper()) 
                                    && k.Description == "Transport")
                                    .ToList();
                                    amount = selfkgspertransporter * (decimal)transporter.Rate;
                                    subsidy += (othermemberskgs.Sum(c => c.Qsupplied)) * (decimal)price.SubsidyPrice;
                                }
                            }
                        }
                        // Assigning trader rate means the transporter is a trader
                        //if (transporter.TraderRate > 0)
                        //{
                        //amount = totalSupplied * (decimal)transporter.TraderRate;
                        //amount = (totalSupplied - selfkgspertransporter) * (decimal)transporter.Rate;
                        //if (price != null && averageSupplied >= price.SubsidyQty)
                        //    subsidy += selfkgspertransporter * (decimal)transporter.TraderRate;
                        //subsidy += (totalSupplied - selfkgspertransporter) * (decimal)transporter.Rate;
                        //}
                    }

                    var intakeskGS = productIntakeslist.Where(i => i.Sno.ToUpper().Equals(transporter.TransCode.ToUpper()) && i.Branch == transporter.Tbranch).ToList()
                                        .Sum(m => m.Qsupplied);
                    if (StrValues.Elburgon == sacco)
                    {

                        subsidy = ((intakeskGS * (decimal)(0.5)));
                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = transporter.TransCode.Trim().ToUpper(),
                            TransDate = period.EndDate,
                            TransTime = DateTime.Now.TimeOfDay,
                            ProductType = "SUBSIDY",
                            Qsupplied = 0,
                            Ppu = 0,
                            CR = subsidy,
                            DR = 0,
                            Description = "SUBSIDY",
                            TransactionType = TransactionType.Deduction,
                            Remarks = "SUBSIDY FOR: " + period.EndDate.ToString("MMM/yyy"),
                            AuditId = loggedInUser,
                            Auditdatetime = DateTime.Now,
                            Branch = transporter.Tbranch,
                            SaccoCode = sacco,
                            DrAccNo = price?.TransportDrAccNo ?? "",
                            CrAccNo = price?.TransportCrAccNo ?? ""
                        });
                    }

                    var grossPay = amount + subsidy;
                    if (grossPay > 0)
                    {
                        var payRoll = new DTransportersPayRoll
                        {
                            Code = transporter.TransCode,
                            Amnt = amount,
                            Subsidy = subsidy,
                            GrossPay = grossPay,
                            QntySup = (double?)p.Sum(s => s.Qsupplied),
                            PhoneNo = transporter.Phoneno,
                            Advance = 0,
                            extension = 0,
                            SMS = 0,
                            CurryForward = 0,
                            Others = 0,
                            MIDPAY = 0,
                            AI = 0,
                            CLINICAL = 0,
                            Tractor = 0,
                            VARIANCE = 0,
                            Agrovet = 0,
                            Hshares = 0,
                            Fsa = 0,
                            Totaldeductions = 0,
                            NetPay = 0,
                            SACCO_SHARES = 0,
                            SACCO_SAVINGS = 0,
                            INST_ADVANCE = 0,
                            BankName = transporter.Bcode,
                            Yyear = period.EndDate.Year,
                            Mmonth = period.EndDate.Month,
                            AccNo = transporter.Accno,
                            BBranch = transporter.Bbranch,
                            EndPeriod = period.EndDate,
                            Rate = 0,
                            SaccoCode = sacco,
                            AuditId = loggedInUser,
                            Branch = transporter.Tbranch,
                            ECLOF = 0,
                            saccoDed = 0
                        };

                        decimal? netPay = grossPay;
                        decimal? carriedForwardValue = 0;
                        var CurryForwarddeduction = (carryforward.Sum(s => s.DR) - carryforward.Sum(s => s.CR));
                        payRoll.CurryForward = CurryForwarddeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && CurryForwarddeduction > 0 && netPay < CurryForwarddeduction)
                            {
                                payRoll.ECLOF = netPay;
                                carriedForwardValue = CurryForwarddeduction - netPay;
                            }
                            if (netPay < 0 && CurryForwarddeduction > 0)
                            {
                                payRoll.ECLOF = 0;
                                carriedForwardValue = CurryForwarddeduction;
                            }
                        }

                        netPay -= CurryForwarddeduction;
                        if (netPay < 0 && CurryForwarddeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Carry Forward",
                                DR = carriedForwardValue,
                            });

                        var agrovetdeduction = (agrovet.Sum(s => s.DR) - agrovet.Sum(s => s.CR));
                        payRoll.Agrovet = agrovetdeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && agrovetdeduction > 0 && netPay < agrovetdeduction)
                            {
                                payRoll.Agrovet = netPay;
                                carriedForwardValue = agrovetdeduction - netPay;
                            }
                            if (netPay < 0 && agrovetdeduction > 0)
                            {
                                payRoll.Agrovet = 0;
                                carriedForwardValue = agrovetdeduction;
                            }
                        }

                        netPay -= agrovetdeduction;
                        if (netPay < 0 && agrovetdeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Agrovet",
                                DR = carriedForwardValue,
                            });

                        var variancededuction = (variance.Sum(s => s.DR) - variance.Sum(s => s.CR));
                        payRoll.VARIANCE = variancededuction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && variancededuction > 0 && netPay < variancededuction)
                            {
                                payRoll.VARIANCE = netPay;
                                carriedForwardValue = variancededuction - netPay;
                            }
                            if (netPay < 0 && variancededuction > 0)
                            {
                                payRoll.VARIANCE = 0;
                                carriedForwardValue = variancededuction;
                            }
                        }

                        netPay -= variancededuction;
                        if (netPay < 0 && variancededuction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Variance",
                                DR = carriedForwardValue,
                            });

                        var milk_recovery = milkRecovery.Sum(s => s.DR) - milkRecovery.Sum(s => s.CR);
                        payRoll.MILK_RECOVERY = milk_recovery;
                        carriedForwardValue = 0;
                        if (netPay > 0 && milk_recovery > 0 && netPay < milk_recovery)
                        {
                            payRoll.MILK_RECOVERY = netPay;
                            carriedForwardValue = milk_recovery - netPay;
                        }
                        if (netPay < 0 && milk_recovery > 0)
                        {
                            payRoll.MILK_RECOVERY = 0;
                            carriedForwardValue = milk_recovery;
                        }

                        netPay -= milk_recovery;
                        if (netPay < 0 && milk_recovery > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "MILK RECOVERY",
                                DR = carriedForwardValue,
                            });

                        var instAdv = instantAdvance.Sum(s => s.DR) - instantAdvance.Sum(s => s.CR);
                        payRoll.INST_ADVANCE = instAdv;
                        carriedForwardValue = 0;
                        if (netPay > 0 && instAdv > 0 && netPay < instAdv)
                        {
                            payRoll.INST_ADVANCE = netPay;
                            carriedForwardValue = instAdv - netPay;
                        }
                        if (netPay < 0 && instAdv > 0)
                        {
                            payRoll.INST_ADVANCE = 0;
                            carriedForwardValue = instAdv;
                        }

                        netPay -= instAdv;
                        if (netPay < 0 && instAdv > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "INST ADV",
                                DR = carriedForwardValue,
                            });

                        var advancededuction = (advance.Sum(s => s.DR) - advance.Sum(s => s.CR));
                        payRoll.Advance = advancededuction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && advancededuction > 0 && netPay < advancededuction)
                            {
                                payRoll.Advance = netPay;
                                carriedForwardValue = advancededuction - netPay;
                            }
                            if (netPay < 0 && advancededuction > 0)
                            {
                                payRoll.Advance = 0;
                                carriedForwardValue = advancededuction;
                            }
                        }

                        netPay -= advancededuction;
                        if (netPay < 0 && advancededuction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Advance",
                                DR = carriedForwardValue,
                            });

                        var midpaydeduction = (MIDPAY.Sum(s => s.DR) - MIDPAY.Sum(s => s.CR));
                        payRoll.MIDPAY = midpaydeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && midpaydeduction > 0 && netPay < midpaydeduction)
                            {
                                payRoll.MIDPAY = netPay;
                                carriedForwardValue = midpaydeduction - netPay;
                            }
                            if (netPay < 0 && midpaydeduction > 0)
                            {
                                payRoll.MIDPAY = 0;
                                carriedForwardValue = midpaydeduction;
                            }
                        }

                        netPay -= midpaydeduction;
                        if (netPay < 0 && midpaydeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Midpay",
                                DR = carriedForwardValue,
                            });

                        var AIDeduction = (ai.Sum(s => s.DR) - ai.Sum(s => s.CR));
                        payRoll.AI = AIDeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && AIDeduction > 0 && netPay < AIDeduction)
                            {
                                payRoll.AI = netPay;
                                carriedForwardValue = AIDeduction - netPay;
                            }
                            if (netPay < 0 && AIDeduction > 0)
                            {
                                payRoll.AI = 0;
                                carriedForwardValue = AIDeduction;
                            }
                        }

                        netPay -= AIDeduction;
                        if (netPay < 0 && AIDeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Ai",
                                DR = carriedForwardValue,
                            });

                        var CLINICALdeduction = (clinical.Sum(s => s.DR) - clinical.Sum(s => s.CR));
                        payRoll.CLINICAL = CLINICALdeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && CLINICALdeduction > 0 && netPay < CLINICALdeduction)
                            {
                                payRoll.CLINICAL = netPay;
                                carriedForwardValue = CLINICALdeduction - netPay;
                            }
                            if (netPay < 0 && CLINICALdeduction > 0)
                            {
                                payRoll.CLINICAL = 0;
                                carriedForwardValue = CLINICALdeduction;
                            }
                        }

                        netPay -= CLINICALdeduction;
                        if (netPay < 0 && CLINICALdeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Clinical",
                                DR = carriedForwardValue,
                            });

                        var tractordeduction = (tractor.Sum(s => s.DR) - tractor.Sum(s => s.CR));
                        payRoll.Tractor = tractordeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && tractordeduction > 0 && netPay < tractordeduction)
                            {
                                payRoll.Tractor = netPay;
                                carriedForwardValue = tractordeduction - netPay;
                            }
                            if (netPay < 0 && tractordeduction > 0)
                            {
                                payRoll.Tractor = 0;
                                carriedForwardValue = tractordeduction;
                            }
                        }

                        netPay -= tractordeduction;
                        if (netPay < 0 && tractordeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Tractor",
                                DR = carriedForwardValue,
                            });

                        var societyShares = (shares.Sum(s => s.DR) - shares.Sum(s => s.CR));
                        payRoll.Hshares = societyShares;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && societyShares > 0 && netPay < societyShares)
                            {
                                payRoll.Tractor = netPay;
                                carriedForwardValue = societyShares - netPay;
                            }
                            if (netPay < 0 && societyShares > 0)
                            {
                                payRoll.Tractor = 0;
                                carriedForwardValue = societyShares;
                            }
                        }

                        netPay -= societyShares;
                        if (netPay < 0 && societyShares > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Society Shares",
                                DR = carriedForwardValue,
                            });

                        var extensionDed = extension.Sum(s => s.DR) - extension.Sum(s => s.CR);
                        payRoll.extension = extensionDed;
                        carriedForwardValue = 0;
                        if (netPay > 0 && extensionDed > 0 && netPay < extensionDed)
                        {
                            payRoll.extension = netPay;
                            carriedForwardValue = extensionDed - netPay;
                        }
                        if (netPay < 0 && extensionDed > 0)
                        {
                            payRoll.extension = 0;
                            carriedForwardValue = extensionDed;
                        }

                        netPay -= extensionDed;
                        if (netPay < 0 && extensionDed > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Extension Work",
                                DR = carriedForwardValue,
                            });

                        var smsDed = SMS.Sum(s => s.DR) - SMS.Sum(s => s.CR);
                        payRoll.SMS = smsDed;
                        carriedForwardValue = 0;
                        if (netPay > 0 && smsDed > 0 && netPay < smsDed)
                        {
                            payRoll.SMS = netPay;
                            carriedForwardValue = smsDed - netPay;
                        }
                        if (netPay < 0 && smsDed > 0)
                        {
                            payRoll.SMS = 0;
                            carriedForwardValue = smsDed;
                        }

                        netPay -= smsDed;
                        if (netPay < 0 && smsDed > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "SMS",
                                DR = carriedForwardValue,
                            });

                        var saccodeductions = (saccoDed.Sum(s => s.DR) - saccoDed.Sum(s => s.CR));
                        payRoll.saccoDed = saccodeductions;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && saccodeductions > 0 && netPay < saccodeductions)
                            {
                                payRoll.saccoDed = netPay;
                                carriedForwardValue = saccodeductions - netPay;
                            }
                            if (netPay < 0 && saccodeductions > 0)
                            {
                                payRoll.saccoDed = 0;
                                carriedForwardValue = saccodeductions;
                            }
                        }

                        netPay -= saccodeductions;
                        if (netPay < 0 && saccodeductions > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "MAENDELEO",
                                DR = carriedForwardValue,
                            });

                        var eclofdeduction = (ECLOF.Sum(s => s.DR) - ECLOF.Sum(s => s.CR));
                        payRoll.ECLOF = eclofdeduction;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && eclofdeduction > 0 && netPay < eclofdeduction)
                            {
                                payRoll.ECLOF = netPay;
                                carriedForwardValue = eclofdeduction - netPay;
                            }
                            if (netPay < 0 && eclofdeduction > 0)
                            {
                                payRoll.ECLOF = 0;
                                carriedForwardValue = eclofdeduction;
                            }
                        }

                        netPay -= eclofdeduction;
                        if (netPay < 0 && eclofdeduction > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Eclof",
                                DR = carriedForwardValue,
                            });

                        var memberLoans = loan.Sum(s => s.DR) - loan.Sum(s => s.CR);
                        payRoll.Fsa = memberLoans;
                        carriedForwardValue = 0;
                        if (netPay > 0 && memberLoans > 0 && netPay < memberLoans)
                        {
                            payRoll.ECLOF = netPay;
                            carriedForwardValue = memberLoans - netPay;
                        }
                        if (netPay < 0 && memberLoans > 0)
                        {
                            payRoll.ECLOF = 0;
                            carriedForwardValue = memberLoans;
                        }

                        netPay -= memberLoans;
                        if (netPay < 0 && memberLoans > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Loan",
                                DR = carriedForwardValue,
                            });

                        var Othersdedutcion = (Others.Sum(s => s.DR) - Others.Sum(s => s.CR));
                        payRoll.Others = Othersdedutcion;
                        if (StrValues.Slopes == sacco)
                        {
                            carriedForwardValue = 0;
                            if (netPay > 0 && Othersdedutcion > 0 && netPay < Othersdedutcion)
                            {
                                payRoll.ECLOF = netPay;
                                carriedForwardValue = Othersdedutcion - netPay;
                            }
                            if (netPay < 0 && Othersdedutcion > 0)
                            {
                                payRoll.ECLOF = 0;
                                carriedForwardValue = Othersdedutcion;
                            }
                        }

                        netPay -= Othersdedutcion;
                        if (netPay < 0 && Othersdedutcion > 0)
                            curriedForwardProducts.Add(new ProductIntake
                            {
                                Sno = transporter.TransCode,
                                ProductType = "Others",
                                DR = carriedForwardValue,
                            });

                        if (StrValues.Slopes == sacco)
                        {
                            var advanceCode = "L01";
                            var normalCode = "L02";
                            var insAdvCode = "L03";
                            var transportersLoans = loans.Where(l => l.MemberNo.ToUpper().Equals(transporter.TransCode.ToUpper())).ToList();

                            if (!saccoLoansProcessed)
                            {
                                transportersLoans.ForEach(l =>
                                {
                                    if (l.Balance > 0 && netPay > 0 && l.Installments > 0)
                                    {
                                        var installments = l.Balance < l.Installments ? l.Balance : l.Installments;
                                        installments = netPay > installments ? installments : netPay;
                                        listSaccoLoans.Add(new SaccoLoans
                                        {
                                            LoanNo = l.LoanNo,
                                            LoanCode = l.LoanCode,
                                            Sno = l.MemberNo,
                                            Amount = installments,
                                            TransDate = monthsLastDate,
                                            AuditDate = DateTime.Now,
                                            Saccocode = sacco,
                                            AuditId = loggedInUser
                                        });
                                    }
                                });
                            }

                            listSaccoLoans.Where(l => l.Sno.ToUpper().Equals(transporter.TransCode.ToUpper())).ForEach(l =>
                            {
                                if (l.LoanCode == normalCode)
                                    payRoll.Fsa += l.Amount;
                                if (l.LoanCode == advanceCode)
                                    payRoll.Advance += l.Amount;
                                if (l.LoanCode == insAdvCode)
                                    payRoll.INST_ADVANCE += l.Amount;
                                netPay -= l.Amount;
                            });

                            var saccoStandingOrder = saccoStandingOrders.FirstOrDefault(o => o.MemberNo.ToUpper().Equals(transporter.TransCode.ToUpper()));
                            if (saccoStandingOrder != null && !saccoSharesProcessed)
                            {
                                decimal? contributedShares = contribs.FirstOrDefault(s => s.MemberNo.ToUpper().Equals(transporter.TransCode.ToUpper()) && s.Sharescode == shareCode)?.Amount ?? 0;
                                if (contributedShares < maxShares)
                                {
                                    var actualContributedShares = netPay > saccoStandingOrder.Installment ? saccoStandingOrder.Installment : netPay;
                                    actualContributedShares = actualContributedShares > 0 ? actualContributedShares : 0;
                                    var currentshares = contributedShares + actualContributedShares;
                                    if (currentshares > maxShares)
                                    {
                                        actualContributedShares = maxShares - contributedShares;
                                        var actualSavings = currentshares - maxShares;
                                        listSaccoShares.Add(new SaccoShares
                                        {
                                            SharesCode = savingsCode,
                                            Sno = saccoStandingOrder.MemberNo,
                                            Amount = actualSavings,
                                            TransDate = monthsLastDate,
                                            AuditDate = DateTime.Now,
                                            Saccocode = sacco,
                                            AuditId = loggedInUser
                                        });
                                    }

                                    if (actualContributedShares > 0)
                                        listSaccoShares.Add(new SaccoShares
                                        {
                                            SharesCode = shareCode,
                                            Sno = saccoStandingOrder.MemberNo,
                                            Amount = actualContributedShares,
                                            TransDate = monthsLastDate,
                                            AuditDate = DateTime.Now,
                                            Saccocode = sacco,
                                            AuditId = loggedInUser
                                        });
                                }
                                else
                                {
                                    var actualSavings = netPay > saccoStandingOrder.Installment ? saccoStandingOrder.Installment : netPay;
                                    actualSavings = actualSavings > 0 ? actualSavings : 0;
                                    listSaccoShares.Add(new SaccoShares
                                    {
                                        SharesCode = savingsCode,
                                        Sno = saccoStandingOrder.MemberNo,
                                        Amount = actualSavings,
                                        TransDate = monthsLastDate,
                                        AuditDate = DateTime.Now,
                                        Saccocode = sacco,
                                        AuditId = loggedInUser
                                    });
                                }
                            }

                            listSaccoShares.Where(l => l.Sno.ToUpper().Equals(transporter.TransCode.ToUpper())).ForEach(l =>
                            {
                                if (l.SharesCode == shareCode)
                                    payRoll.SACCO_SHARES += l.Amount;
                                if (l.SharesCode == savingsCode)
                                    payRoll.SACCO_SAVINGS += l.Amount;

                                netPay -= l.Amount;
                            });
                        }

                        payRoll.Totaldeductions = grossPay - netPay;
                        if (StrValues.Slopes == sacco)
                            payRoll.Totaldeductions = payRoll.Totaldeductions > grossPay ? grossPay : payRoll.Totaldeductions;

                        //netPay -= debits;
                        payRoll.NetPay = netPay;
                        dTransportersPayRolls.Add(payRoll);
                    }
                }
            });

            if (dTransportersPayRolls.Any())
                await _context.DTransportersPayRolls.AddRangeAsync(dTransportersPayRolls);
            if (listSaccoLoans.Any() && !saccoLoansProcessed)
                await _context.SaccoLoans.AddRangeAsync(listSaccoLoans);
            if (listSaccoShares.Any() && !saccoSharesProcessed)
                await _context.SaccoShares.AddRangeAsync(listSaccoShares);

            if (StrValues.Slopes == sacco)
            {
                curriedForwardProducts.ForEach(f =>
                {
                    // Credit last month
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = f.Sno,
                        TransDate = monthsLastDate,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = f.ProductType,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = f.DR,
                        DR = 0,
                        Balance = 0,
                        Description = "Carry Forward",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Carry Forward",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });

                    // Debit next month
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = f.Sno,
                        TransDate = nextMonthStartDate,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = f.ProductType,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = 0,
                        DR = f.DR,
                        Balance = 0,
                        Description = "Carry Forward",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = "Carry Forward",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = saccoBranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                });
            }

            await _context.SaveChangesAsync();
            _notyf.Success("Payroll processed successfully");
            return RedirectToAction(nameof(Index));
        }

        private void deleteTranspoterIntakes(DateTime startDate, DateTime endDate, string sacco, string branchName, string loggedInUser)
        {
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var supp = _context.DSuppliers.Where(n => n.Scode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Sno).ToList();

            var getsuppliers = productIntakeslist.Where(n => n.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && n.TransDate >= startDate
             && n.TransDate <= endDate && n.Description == "Transport" && supp.Contains(n.Sno.ToUpper())
             && n.TransactionType == TransactionType.Deduction).ToList();

            _context.RemoveRange(getsuppliers);

            //getsuppliers.ForEach(s =>
            //{
            //    var deletetransport = productIntakeslist.Where(n => n.Sno.ToUpper().Equals(s.ToUpper())
            //  && n.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            //  && n.Description == "Transport" && n.TransactionType == TransactionType.Deduction && n.TransDate >= startDate
            //  && n.TransDate <= endDate).ToList();
            //    _context.RemoveRange(deletetransport);
            //});
        }
        private void clearTranspoterIntakes(DateTime startDate, DateTime endDate, string sacco, string branchName, string loggedInUser)
        {
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;

            var getsuppliers = productIntakeslist.Where(n => n.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && n.TransDate >= startDate
             && n.TransDate <= endDate &&
             (n.TransactionType == TransactionType.Correction || n.TransactionType == TransactionType.Intake))
                .Select(b => b.Sno.ToUpper()).Distinct().ToList();
            getsuppliers.ForEach(s =>
            {
                var getpricegls = _context.DPrices.FirstOrDefault(j => j.SaccoCode.ToUpper().Equals(sacco.ToUpper()));

                var gettransportersrate = _context.DTransports.FirstOrDefault(h => h.Sno.ToUpper().Equals(s.ToUpper())
                && h.saccocode.ToUpper().Equals(sacco.ToUpper()));
                if (gettransportersrate != null)
                {
                    decimal Rate = 0;
                    Rate = (decimal)gettransportersrate.Rate;

                    var sumkgs = productIntakeslist.Where(i => i.Sno.ToUpper().Equals(s.ToUpper())
                    && i.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                    && (i.TransactionType == TransactionType.Intake || i.TransactionType == TransactionType.Correction)
                    && i.TransDate >= startDate && i.TransDate <= endDate).ToList().Sum(n => n.Qsupplied);
                    var actualrate = Rate * sumkgs;

                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = s.ToUpper(),
                        TransDate = (DateTime)endDate,
                        TransTime = DateTime.Now.TimeOfDay,
                        ProductType = getpricegls.Products,
                        Qsupplied = sumkgs,
                        Ppu = Rate,
                        CR = 0,
                        DR = actualrate,
                        Balance = actualrate,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = "",
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = branchName,
                        SaccoCode = sacco,
                        DrAccNo = getpricegls.TransportCrAccNo,
                        CrAccNo = getpricegls.TransportDrAccNo,

                    });
                }
            });
        }

        private async Task CalcDefaultdeductions(DateTime startDate, DateTime endDate, List<ProductIntake> productIntakes)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var dcodes = await _context.DDcodes.Where(d => d.Dcode == sacco).ToListAsync();
            ViewBag.isElburgon = StrValues.Elburgon == sacco;

            var intakes = productIntakes.GroupBy(p => p.Sno.Trim().ToUpper()).ToList();
            var preSets = await _context.d_PreSets.Where(l => !l.Stopped && l.saccocode == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                preSets = preSets.Where(p => p.BranchCode == saccoBranch).ToList();
            var shares = await _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
            && f.TransDate >= startDate && f.TransDate <= endDate && f.Type != "Checkoff" && f.Pmode != "Checkoff").ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                shares = shares.Where(p => p.Branch == saccoBranch).ToList();

            var getpricegls = _context.DPrices.FirstOrDefault(j => j.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
            var dShares = new List<DShare>();
            var products = new List<ProductIntake>();
            var gltransactions = new List<Gltransaction>();
            if (StrValues.Elburgon != sacco)
            {
                intakes.ForEach(g =>
                {
                    var totalkgs = g.Where(l => l.TransactionType == TransactionType.Intake || l.TransactionType == TransactionType.Correction).Sum(w => w.Qsupplied);
                    if (totalkgs > 0)
                    {
                        var eachdeductions = preSets.Where(l => l.Sno == g.Key).ToList();
                        if (eachdeductions.Any())
                        {
                            var eachdeductionsselect = eachdeductions.GroupBy(p => p.Deduction.Trim().ToUpper()).Distinct().ToList();
                            eachdeductionsselect.ForEach(n =>
                            {
                                var Checkanydefaultdeduction = n.FirstOrDefault();
                                var bonus = eachdeductions.FirstOrDefault(m => m.Deduction == Checkanydefaultdeduction.Deduction).Rate;
                                if (Checkanydefaultdeduction.Rated == true)
                                    bonus = totalkgs * Checkanydefaultdeduction.Rate;

                                var glbonus = bonus;

                                if (Checkanydefaultdeduction.Deduction.ToUpper().Equals("Shares".ToUpper()))
                                {

                                    var sharespaid = shares.Where(f => f.Sno.ToUpper().Equals(g.Key.ToUpper())).Sum(m => m.Amount);
                                    if (sharespaid > 0)
                                    {
                                        if (sharespaid < bonus)
                                        {
                                            bonus = bonus - sharespaid;
                                            glbonus = bonus;

                                            dShares.Add(new DShare
                                            {
                                                Sno = g.Key.Trim().ToUpper(),
                                                Type = "Checkoff",
                                                Pmode = "Checkoff",
                                                Amount = (decimal)bonus,
                                                Period = DateTime.Today.Month.ToString(),
                                                TransDate = endDate,
                                                AuditDateTime = DateTime.Now,
                                                SaccoCode = sacco,
                                                zone = Checkanydefaultdeduction.BranchCode,
                                                AuditId = loggedInUser,
                                                Branch = saccoBranch
                                            });
                                        }
                                        else
                                        {
                                            bonus = 0;
                                            glbonus = sharespaid;
                                        }

                                    }

                                }

                                if (bonus > 0)
                                {
                                    products.Add(new ProductIntake
                                    {
                                        Sno = g.Key.Trim().ToUpper(),
                                        TransDate = endDate,
                                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                                        ProductType = Checkanydefaultdeduction.Deduction,
                                        Qsupplied = 0,
                                        Ppu = 0,
                                        CR = 0,
                                        DR = bonus,
                                        Balance = 0,
                                        Description = Checkanydefaultdeduction.Remark,
                                        TransactionType = TransactionType.Deduction,
                                        Remarks = Checkanydefaultdeduction.Remark,
                                        AuditId = loggedInUser,
                                        Auditdatetime = DateTime.Now,
                                        Branch = saccoBranch,
                                        SaccoCode = sacco,
                                        DrAccNo = "0",
                                        CrAccNo = "0"
                                    });
                                }

                                var glsforbonus = dcodes.FirstOrDefault(m => m.Description.ToUpper().Equals(Checkanydefaultdeduction.Deduction.ToUpper()));
                                gltransactions.Add(new Gltransaction
                                {
                                    AuditId = loggedInUser,
                                    TransDate = endDate,
                                    Amount = (decimal)glbonus,
                                    AuditTime = DateTime.Now,
                                    Source = g.Key.Trim().ToUpper(),
                                    TransDescript = Checkanydefaultdeduction.Deduction,
                                    Transactionno = $"{loggedInUser}{DateTime.Now}",
                                    SaccoCode = sacco,
                                    DrAccNo = glsforbonus.Dedaccno,
                                    CrAccNo = glsforbonus.Contraacc,
                                    Branch = saccoBranch,
                                });
                            });
                        };
                    }
                });

            }
            else
            {
                var getsuppliers = preSets.GroupBy(b => b.Sno.ToUpper()).Distinct().ToList();
                getsuppliers.ForEach(n =>
                {
                    var supplierDetails = n.FirstOrDefault();
                    var Selectdetails = productIntakes.Where(l => l.Sno.ToUpper().Equals(supplierDetails.Sno.ToUpper())
                    && (l.TransactionType == TransactionType.Intake || l.TransactionType == TransactionType.Correction)).ToList();
                    var kilos = Selectdetails.Sum(w => w.Qsupplied);
                    if (kilos > 0)
                    {
                        var totalshare = shares.Where(f => f.Sno.ToUpper().Equals(supplierDetails.Sno.ToUpper())).Sum(n => n.Amount);
                        if (totalshare < 20000)
                        {
                            var checkifhasnegative = Selectdetails.Sum(f => f.CR) - Selectdetails.Sum(g => g.DR);
                            if (checkifhasnegative > 0)
                            {
                                dShares.Add(new DShare
                                {
                                    Sno = supplierDetails.Sno.Trim().ToUpper(),
                                    Type = "SHARES",
                                    Pmode = "Checkoff",
                                    Amount = (decimal)kilos,
                                    Period = DateTime.Today.Month.ToString(),
                                    TransDate = endDate,
                                    AuditDateTime = DateTime.Now,
                                    SaccoCode = sacco,
                                    zone = "",
                                    AuditId = loggedInUser,
                                    Branch = saccoBranch
                                });

                                products.Add(new ProductIntake
                                {
                                    Sno = supplierDetails.Sno.Trim().ToUpper(),
                                    TransDate = (DateTime)endDate,
                                    TransTime = DateTime.Now.TimeOfDay,
                                    ProductType = "SHARES",
                                    Qsupplied = 0,
                                    Ppu = 0,
                                    CR = 0,
                                    DR = (decimal)kilos,
                                    Balance = 0,
                                    Description = "SHARES",
                                    TransactionType = TransactionType.Deduction,
                                    Remarks = "5",
                                    AuditId = loggedInUser,
                                    Auditdatetime = DateTime.Now,
                                    Branch = saccoBranch,
                                    SaccoCode = sacco,
                                    DrAccNo = getpricegls.TransportCrAccNo,
                                    CrAccNo = getpricegls.TransportDrAccNo,

                                });

                                var glsforbonus = dcodes.FirstOrDefault(m => m.Description.ToUpper().Equals("Shares".ToUpper()));
                                gltransactions.Add(new Gltransaction
                                {
                                    AuditId = loggedInUser,
                                    TransDate = endDate,
                                    Amount = (decimal)kilos,
                                    AuditTime = DateTime.Now,
                                    Source = supplierDetails.Sno.Trim().ToUpper(),
                                    TransDescript = "SHARES",
                                    Transactionno = $"{loggedInUser}{DateTime.Now}",
                                    SaccoCode = sacco,
                                    DrAccNo = glsforbonus.Dedaccno,
                                    CrAccNo = glsforbonus.Contraacc,
                                    Branch = saccoBranch,
                                });
                            }
                        }
                    }
                });
            }

            if (dShares.Any())
                await _context.DShares.AddRangeAsync(dShares);
            if (products.Any())
                await _context.ProductIntake.AddRangeAsync(products);
            if (gltransactions.Any())
                await _context.Gltransactions.AddRangeAsync(gltransactions);
        }

        private async Task ConsolidateTranspoterIntakes(DateTime startDate, DateTime endDate, List<ProductIntake> productIntakeslist)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var transporters = await _context.DTransporters.Where(t => t.ParentT == sacco).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(p => p.Tbranch == saccoBranch).ToList();
            var assignedTranporters = await _context.DTransports.Where(t => t.saccocode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                assignedTranporters = assignedTranporters.Where(p => p.Branch == saccoBranch).ToList();
            var branchIntakes = productIntakeslist.Where(i => (i.TransactionType == TransactionType.Correction || i.TransactionType == TransactionType.Intake)).ToList();
            var prices = await _context.DPrices.Where(p => p.SaccoCode == sacco).ToListAsync();
            foreach (var transporter in transporters)
            {
                transporter.TransCode = transporter?.TransCode ?? "";
                var transAssignments = assignedTranporters.Where(t => t.TransCode.Trim().ToUpper().Equals(transporter.TransCode.Trim().ToUpper())).ToList();
                var transporterIntakes = branchIntakes.Join(transAssignments,
                    i => i.Sno.Trim().ToUpper(),
                    t => t.Sno.Trim().ToUpper(),
                    (i, t) => new
                    {
                        i.Qsupplied,
                        t.Sno,
                        t.TransCode,
                        i.SaccoCode,
                        i.Remarks,
                        i.TransTime,
                        i.Ppu,
                        i.Branch,
                        i.ProductType,
                        i.CR,
                        i.DR,
                        i.DrAccNo,
                        i.CrAccNo,
                        i.TransDate,
                        t.Rate,
                        t.Startdate,
                        t.DateInactivate,
                        i.TransactionType
                    }).ToList();

                // Debit supplier transport amount
                var supplierIntakes = transporterIntakes.Distinct().GroupBy(s => s.Sno).ToList();
                supplierIntakes.ForEach(s =>
                {
                    var intake = s.FirstOrDefault();
                    var product = intake?.ProductType ?? "";
                    var price = prices.FirstOrDefault(s => s.Products.ToUpper().Equals(product.ToUpper()));
                    decimal? cr = 0;
                    var framersTotal = s.Sum(t => t.Qsupplied);
                    var dr = framersTotal * intake.Rate;
                    if (intake.Rate > 0)
                    {
                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = intake.Sno,
                            TransDate = endDate,
                            TransTime = intake.TransTime,
                            ProductType = "Transport",
                            Qsupplied = s.Sum(t => t.Qsupplied),
                            Ppu = intake.Rate,
                            CR = cr,
                            DR = dr,
                            Description = "Transport",
                            TransactionType = TransactionType.Deduction,
                            Remarks = "Transport For: " + intake.TransCode,
                            AuditId = loggedInUser,
                            Auditdatetime = DateTime.Now,
                            Branch = intake.Branch,
                            SaccoCode = intake.SaccoCode,
                            DrAccNo = intake.DrAccNo,
                            CrAccNo = intake.CrAccNo
                        });
                    }

                    // Credit transpoter transport amount
                    dr = 0;
                    transporter.Rate = transporter?.Rate ?? 0;
                    cr = framersTotal * (decimal)transporter.Rate;
                    if (intake.Rate > 0)
                        cr = framersTotal * intake.Rate;

                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = intake.TransCode.Trim().ToUpper(),
                        TransDate = endDate,
                        TransTime = intake.TransTime,
                        ProductType = "Transport",
                        Qsupplied = s.Sum(t => t.Qsupplied),
                        Ppu = intake.Rate,
                        CR = cr,
                        DR = dr,
                        Description = "Transport",
                        TransactionType = TransactionType.Deduction,
                        Remarks = "Intake for: " + intake.Sno,
                        AuditId = loggedInUser,
                        Auditdatetime = DateTime.Now,
                        Branch = intake.Branch,
                        SaccoCode = intake.SaccoCode,
                        DrAccNo = price?.TransportDrAccNo ?? "",
                        CrAccNo = price?.TransportCrAccNo ?? ""
                    });
                });

            }
        }

        // GET: DPayrolls/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dPayroll = await _context.DPayrolls.FindAsync(id);
            if (dPayroll == null)
            {
                return NotFound();
            }
            return View(dPayroll);
        }

        // POST: DPayrolls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,Transport,Agrovet,Bonus,Tmshares,Fsa,Hshares,Advance,Others,Tdeductions,KgsSupplied,Gpay,Npay,Yyear,Mmonth,Bank,AccountNumber,Bbranch,Trader,Sbranch,EndofPeriod,Auditid,Auditdatetime,Mainaccno,Transportaccno,Agrovetaccno,Aiaccno,Tmsharesaccno,Fsaaccno,Hsharesaccno,Advanceaccno,Otheraccno,Netaccno,Subsidy,Cbo,IdNo,Tchp,Midmonth,Deduct12,Mpesa")] DPayroll dPayroll)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != dPayroll.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dPayroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPayrollExists(dPayroll.Id))
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
            return View(dPayroll);
        }

        // GET: DPayrolls/Delete/5
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

            var dPayroll = await _context.DPayrolls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPayroll == null)
            {
                return NotFound();
            }

            return View(dPayroll);
        }

        // POST: DPayrolls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dPayroll = await _context.DPayrolls.FindAsync(id);
            _context.DPayrolls.Remove(dPayroll);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPayrollExists(long id)
        {
            return _context.DPayrolls.Any(e => e.Id == id);
        }

        [HttpGet]
        public JsonResult CurryForward(DateTime period)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var startDate = new DateTime(period.Year, period.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var nextMonth = startDate.AddMonths(1);
            var productty = "Carry Forward";
            var thismonthdescription = "Carry Forward";
            var forlastmonthremarks = "Carry Forward";

            if (endDate == null)
            {
                _notyf.Error("Please provide Date.");
                return Json(new { success = false });
            }
            IQueryable<DPayroll> dPayrolls = _context.DPayrolls.Where(p => p.SaccoCode == sacco && p.EndofPeriod == endDate && p.Npay < 0);
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake.Where(i => i.SaccoCode == sacco);
            IQueryable<DTransportersPayRoll> dTransportersPayRolls = _context.DTransportersPayRolls.Where(p => p.SaccoCode == sacco && p.EndPeriod == endDate && p.NetPay < 0);
            IQueryable<DSupplier> dSuppliers = _context.DSuppliers.Where(s => s.Scode == sacco);
            IQueryable<DTransporter> dTransporters = _context.DTransporters.Where(t => t.ParentT == sacco);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(auditId.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                dPayrolls = dPayrolls.Where(p => p.Branch == saccoBranch);
                productIntakes = productIntakes.Where(i => i.Branch == saccoBranch);
                dTransportersPayRolls = dTransportersPayRolls.Where(p => p.Branch == saccoBranch);
                dSuppliers = dSuppliers.Where(s => s.Branch == saccoBranch);
                dTransporters = dTransporters.Where(t => t.Bbranch == saccoBranch);
            }

            if (productIntakes.Any(i => i.TransDate == endDate && i.Description == "Carry Forward" && i.CR > 0))
            {
                _notyf.Error("Sorry, Carry Forward already processed");
                return Json(new { success = false });
            }

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if (StrValues.Elburgon == sacco)
            {
                productty = "Others";
                thismonthdescription = endDate.Month.ToString() + endDate.Year.ToString() + "Arrears";
                forlastmonthremarks = endDate.Month.ToString() + endDate.Year.ToString() + "Arrears CF";
            }

            dPayrolls.ForEach(s =>
            {
                //credit privious month
                _context.ProductIntake.Add(new ProductIntake
                {
                    Sno = s.Sno.ToString(),
                    TransDate = endDate,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productty,
                    Qsupplied = 0,
                    Ppu = 0,
                    CR = -s.Npay,
                    DR = 0,
                    Balance = 0,
                    Description = "Carry Forward",
                    TransactionType = TransactionType.Deduction,
                    Paid = false,
                    Remarks = forlastmonthremarks,
                    AuditId = auditId,
                    Auditdatetime = DateTime.Now,
                    Branch = s.Branch,
                    SaccoCode = sacco,
                    DrAccNo = "",
                    CrAccNo = "",
                    Posted = false
                });
                //debit next month
                _context.ProductIntake.Add(new ProductIntake
                {
                    Sno = s.Sno.ToString(),
                    TransDate = nextMonth,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productty,
                    Qsupplied = 0,
                    Ppu = 0,
                    CR = 0,
                    DR = -s.Npay,
                    Balance = 0,
                    Description = "Carry Forward",
                    TransactionType = TransactionType.Deduction,
                    Paid = false,
                    Remarks = thismonthdescription,
                    AuditId = auditId,
                    Auditdatetime = DateTime.Now,
                    Branch = s.Branch,
                    SaccoCode = sacco,
                    DrAccNo = "",
                    CrAccNo = "",
                    Posted = false
                });
            });

            dTransportersPayRolls.ForEach(s =>
            {
                //INSERT TO LAST MONTH
                _context.ProductIntake.Add(new ProductIntake
                {
                    Sno = s.Code,
                    TransDate = endDate,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productty,
                    Qsupplied = 0,
                    Ppu = 0,
                    CR = -s.NetPay,
                    DR = 0,
                    Balance = 0,
                    Description = "Carry Forward",
                    TransactionType = TransactionType.Deduction,
                    Paid = false,
                    Remarks = forlastmonthremarks,
                    AuditId = auditId,
                    Auditdatetime = DateTime.Now,
                    Branch = s.Branch,
                    SaccoCode = sacco,
                    DrAccNo = "",
                    CrAccNo = "",
                    Posted = false
                });
                //INSERT TO NEXT MONTH
                _context.ProductIntake.Add(new ProductIntake
                {
                    Sno = s.Code,
                    TransDate = nextMonth,
                    TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                    ProductType = productty,
                    Qsupplied = 0,
                    Ppu = 0,
                    CR = 0,
                    DR = -s.NetPay,
                    Balance = 0,
                    Description = "Carry Forward",
                    TransactionType = TransactionType.Deduction,
                    Paid = false,
                    Remarks = thismonthdescription,
                    AuditId = auditId,
                    Auditdatetime = DateTime.Now,
                    Branch = s.Branch,
                    SaccoCode = sacco,
                    DrAccNo = "",
                    CrAccNo = "",
                    Posted = false
                });
            });

            _context.SaveChanges();
            _notyf.Success("Curry Forward processed successfully");
            return Json("");
        }
    }
}
