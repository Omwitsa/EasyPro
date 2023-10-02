using System;
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

namespace EasyPro.Controllers
{
    public class DPayrollsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DPayrollsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
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
            var payrolls = await _context.DPayrolls.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.EndofPeriod == endDate).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
            {
                suppliers = suppliers.Where(i => i.Branch == saccoBranch).ToList();
                payrolls = payrolls.Where(i => i.Branch == saccoBranch).ToList();
            }
                
            var payroll = payrolls.Join(suppliers,
                p => p.Sno.ToUpper(),
                s => s.Sno.ToUpper(),
                (p, s) => new PayrollVm
                {
                    Sno = s.Sno,
                    Names = s.Names,
                    PhoneNo = s.PhoneNo,
                    IdNo = s.IdNo,
                    Bank = s.Bcode,
                    AccNo = s.AccNo,
                    Branch = s.Branch,
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
                }).OrderBy(p => p.Bank).ToList();

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
            var startDate = new DateTime(period.EndDate.Year, period.EndDate.Month, 1);
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var payrolls = await _context.DPayrolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                payrolls = payrolls.Where(p => p.Branch == saccoBranch).ToList();
            if (payrolls.Any())
            {
                _context.DPayrolls.RemoveRange(payrolls);
                _context.SaveChanges();
            }
            var transportersPayRolls = await _context.DTransportersPayRolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                transportersPayRolls = transportersPayRolls.Where(p => p.Branch == saccoBranch).ToList();
            if (transportersPayRolls.Any())
            {
                _context.DTransportersPayRolls.RemoveRange(transportersPayRolls);
                _context.SaveChanges();
            }

            var productIntakeslist = await _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate && i.TransDate <= period.EndDate).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                productIntakeslist = productIntakeslist.Where(p => p.Branch == saccoBranch).ToList();
            if (sacco != StrValues.Mburugu && sacco != StrValues.Slopes)
            {
                var transpoterIntakes = productIntakeslist.Where(i => i.Description.ToLower().Equals("transport")).ToList();
                if (transpoterIntakes.Any())
                {
                    _context.ProductIntake.RemoveRange(transpoterIntakes);
                    _context.SaveChanges();
                }

                var checkifanydeduction = productIntakeslist.Where(n => n.Description.ToLower().Contains("midpay")).ToList();
                if (checkifanydeduction.Any())
                {
                    _context.ProductIntake.RemoveRange(checkifanydeduction);
                    _context.SaveChanges();
                }
            }

            var deletestandingorder = productIntakeslist.Where(i => i.Remarks == "Standing Order");
            if (deletestandingorder.Any())
            {
                _context.ProductIntake.RemoveRange(deletestandingorder);
                _context.SaveChanges();
            }

            var preSets = await _context.d_PreSets.Where(b => b.saccocode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                preSets = preSets.Where(p => p.BranchCode == saccoBranch).ToList();
            var selectdistinctdedname = preSets.Select(b => b.Deduction.ToUpper()).Distinct();
            foreach (var dedtype in selectdistinctdedname)
            {
                var deletesdefaultded = productIntakeslist.Where(i => i.ProductType.ToUpper().Equals(dedtype.ToUpper())).ToList();
                if (deletesdefaultded.Any())
                    _context.ProductIntake.RemoveRange(deletesdefaultded);
            }

            var checkgls = await _context.Gltransactions
               .Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= startDate && f.TransDate <= period.EndDate 
               && (f.TransDescript.ToUpper().Equals("Bonus".ToUpper()) || f.TransDescript.ToUpper().Equals("Shares".ToUpper()))).ToListAsync();

            if(user.AccessLevel == AccessLevel.Branch)
                checkgls = checkgls.Where(p => p.Branch == saccoBranch).ToList();
            if (checkgls.Any())
                _context.Gltransactions.RemoveRange(checkgls);
            var checksharespaid = await _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                            && f.TransDate >= startDate && f.TransDate <= period.EndDate && f.Type == "Checkoff"
                            && f.Pmode == "Checkoff").ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                checksharespaid = checksharespaid.Where(p => p.Branch == saccoBranch).ToList();
            if (checksharespaid.Any())
                _context.DShares.RemoveRange(checksharespaid);

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if(StrValues.Elburgon != sacco)
            {
                var preSet = preSets.FirstOrDefault(n => n.saccocode == sacco);
                //update default deductions if any like bonus
                if (preSet != null)
                    await CalcDefaultdeductions(startDate, period.EndDate, productIntakeslist);
            }
            else
            {
                await CalcDefaultdeductions(startDate, period.EndDate, productIntakeslist);
            }

            if (sacco != StrValues.Mburugu && sacco != StrValues.Slopes)
                await ConsolidateTranspoterIntakes(startDate, period.EndDate, productIntakeslist);

            _context.SaveChanges();
            var dcodes = await _context.DDcodes.Where(c => c.Description.ToLower().Equals("advance") || c.Description.ToLower().Equals("transport")
               || c.Description.ToLower().Equals("agrovet") || c.Description.ToLower().Equals("store")
               || c.Description.ToLower().Equals("bonus") || c.Description.ToLower().Equals("shares")
               || c.Description.ToLower().Equals("loan") || c.Description.ToLower().Equals("carry forward") || c.Description.ToLower().Equals("clinical")
               || c.Description.ToLower().Equals("a.i") || c.Description.ToLower().Equals("ai") || c.Description.ToLower().Equals("tractor")
               || c.Description.ToLower().Equals("sms") || c.Description.ToLower().Equals("extension work")
               || c.Description.ToLower().Equals("registration") || c.Description.ToLower().Equals("midpay")).Select(c => c.Description.ToLower()).ToListAsync();

            var suppliers = await _context.DSuppliers.Where(s => s.Scode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(t => t.Branch == saccoBranch).ToList();

            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == sacco);
            var supplierNos = suppliers.Select(s => s.Sno);
            var supplierIntakes = productIntakeslist.Where(p => supplierNos.Contains(p.Sno)).ToList();
            var intakes = supplierIntakes.GroupBy(p => p.Sno.ToUpper()).ToList();
            intakes.ForEach(p =>
            {
                var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                var transport = p.Where(k => k.Description.ToLower().Contains("transport"));
                var agrovet = p.Where(k => k.ProductType.ToLower().Contains("agrovet") || k.ProductType.ToLower().Contains("store"));
                var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                var loan = p.Where(k => k.ProductType.ToLower().Contains("loan"));
                var carryforward = p.Where(k => k.ProductType.ToLower().Contains("carry forward"));
                var clinical = p.Where(k => k.ProductType.ToLower().Contains("clinical"));
                var ai = p.Where(k => (k.ProductType.ToLower().Contains("ai") || k.ProductType.ToLower().Contains("a.i")));
                var tractor = p.Where(k => k.ProductType.ToLower().Contains("tractor"));
                var extension = p.Where(k => k.ProductType.ToLower().Contains("extension work"));
                var SMS = p.Where(k => k.ProductType.ToLower().Contains("sms"));
                var registration = p.Where(k => k.ProductType.ToLower().Contains("registration"));
                var MIDPAY = p.Where(k => k.ProductType.ToLower().Contains("midpay"));
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                var Others = p.Where(k => !dcodes.Contains(k.ProductType.ToLower()) && !k.ProductType.ToUpper().Equals("AGROVET")
                && !k.ProductType.ToUpper().Equals("MILK") && k.TransactionType == TransactionType.Deduction);
                var supplier = suppliers.FirstOrDefault(s => s.Sno.ToUpper().Equals(p.Key));
                if (supplier != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    var credited = p.Sum(s => s.CR);
                    var framersTotal = p.Sum(s => s.Qsupplied);
                    decimal? subsidy = 0;
                    if (StrValues.Slopes == sacco)
                    {
                        credited = framersTotal * price.Price;
                        var daysInMonth = DateTime.DaysInMonth(period.EndDate.Year, period.EndDate.Month);
                        var averageSupplied = framersTotal / daysInMonth;
                        if (price != null && averageSupplied >= price.SubsidyQty)
                            subsidy += framersTotal * price.SubsidyPrice;
                    }

                    var Tot = advance.Sum(s => s.DR) + agrovet.Sum(s => s.DR) + bonus.Sum(s => s.DR) + shares.Sum(s => s.DR)
                    + Others.Sum(s => s.DR) + clinical.Sum(s => s.DR) + ai.Sum(s => s.DR) + tractor.Sum(s => s.DR) + transport.Sum(s => s.DR)
                    + carryforward.Sum(s => s.DR) + loan.Sum(s => s.DR) + extension.Sum(s => s.DR) + SMS.Sum(s => s.DR)
                    + registration.Sum(s => s.DR) + MIDPAY.Sum(s => s.DR);

                    var grossPay = credited + subsidy;
                    if (supplier.TransCode == "Weekly" || (supplier.TransCode == "Monthly" && period.EndDate == monthsLastDate))
                    {
                        _context.DPayrolls.Add(new DPayroll
                        {
                            Sno = supplier.Sno,
                            Subsidy = subsidy,
                            Gpay = grossPay,
                            KgsSupplied = (double?)milk.Sum(s => s.Qsupplied),
                            Advance = advance.Sum(s => s.DR),
                            CurryForward = carryforward.Sum(s => s.DR),
                            Others = Others.Sum(s => s.DR),
                            CLINICAL = clinical.Sum(s => s.DR),
                            AI = ai.Sum(s => s.DR),
                            Tractor = tractor.Sum(s => s.DR),
                            Transport = transport.Sum(s => s.DR),
                            Registration = registration.Sum(s => s.DR),
                            extension = extension.Sum(s => s.DR),
                            SMS = SMS.Sum(s => s.DR),
                            Agrovet = agrovet.Sum(s => s.DR),
                            Bonus = bonus.Sum(s => s.DR),
                            Fsa = loan.Sum(s => s.DR),
                            Hshares = shares.Sum(s => s.DR),
                            MIDPAY = MIDPAY.Sum(s => s.DR),
                            Tdeductions = Tot,
                            Npay = grossPay - (debits + Tot),
                            Yyear = period.EndDate.Year,
                            Mmonth = period.EndDate.Month,
                            Bank = supplier.Bcode,
                            AccountNumber = supplier.AccNo,
                            Bbranch = supplier.Bbranch,
                            IdNo = supplier.IdNo,
                            EndofPeriod = period.EndDate,
                            SaccoCode = sacco,
                            Auditid = loggedInUser,
                            Branch = supplier.Branch
                        });

                        var checkifanydeduction = productIntakeslist.Where(n => n.Branch == supplier.Branch && n.TransDate == period.EndDate
                        && n.Sno.ToUpper().Equals(supplier.Sno.ToUpper().ToString()) && n.Description.ToLower().Contains("midpay")).ToList();
                        if (!checkifanydeduction.Any() && period.EndDate != monthsLastDate)
                            _context.ProductIntake.Add(new ProductIntake
                            {
                                Sno = supplier.Sno.ToUpper().ToString(),
                                TransDate = period.EndDate,
                                TransTime = DateTime.Now.TimeOfDay,
                                ProductType = "midpay",
                                Qsupplied = 0,
                                Ppu = 0,
                                CR = 0,
                                DR = p.Sum(s => s.CR) - debits - Tot,
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

            await _context.SaveChangesAsync();
            var transporters = await _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(p => p.Tbranch == saccoBranch).ToList();
            var transpoterCodes = transporters.Select(s => s.TransCode.Trim().ToUpper()).ToList();
            var productIntakes = productIntakeslist.Where(p => transpoterCodes.Contains(p.Sno.Trim().ToUpper())).ToList();
            intakes = productIntakes.GroupBy(p => p.Sno.Trim().ToUpper()).ToList();
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
                var bonus = p.Where(k => k.ProductType.ToLower().Contains("bonus"));
                var shares = p.Where(k => k.ProductType.ToLower().Contains("shares"));
                var extension = p.Where(k => k.ProductType.ToLower().Contains("extension work"));
                var SMS = p.Where(k => k.ProductType.ToLower().Contains("sms"));
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                //var payroll = new DTransportersPayRoll();
                var transporter = transporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(p.Key.ToUpper()));
                if (transporter != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    var amount = p.Sum(s => s.CR);
                    var totalSupplied = p.Sum(s => s.Qsupplied);
                    decimal subsidy = 0;
                    if (StrValues.Slopes == sacco)
                    {
                        var daysInMonth = DateTime.DaysInMonth(period.EndDate.Year, period.EndDate.Month);
                        var averageSupplied = totalSupplied / daysInMonth;
                        transporter.TraderRate = transporter?.TraderRate ?? 0;
                        // Assigning trader rate means the transporter is a trader
                        if (transporter.TraderRate > 0)
                        {
                            amount = totalSupplied * (decimal)transporter.TraderRate;
                            if (price != null && averageSupplied >= price.SubsidyQty)
                                subsidy += totalSupplied * (decimal)transporter.Rate;
                        }
                    }

                    var Tot = advance.Sum(s => s.DR) + agrovet.Sum(s => s.DR) + shares.Sum(s => s.DR)
                    + Others.Sum(s => s.DR) + clinical.Sum(s => s.DR) + ai.Sum(s => s.DR) + MIDPAY.Sum(s => s.DR)
                    + tractor.Sum(s => s.DR) + variance.Sum(s => s.DR) + carryforward.Sum(s => s.DR) + extension.Sum(s => s.DR) + SMS.Sum(s => s.DR);

                    var grossPay = amount + subsidy;
                    _context.DTransportersPayRolls.Add(new DTransportersPayRoll
                    {
                        Code = transporter.TransCode,
                        Amnt = amount,
                        Subsidy = subsidy,
                        GrossPay = grossPay,
                        QntySup = (double?)p.Sum(s => s.Qsupplied),
                        Advance = advance.Sum(s => s.DR),
                        extension = extension.Sum(s => s.DR),
                        SMS = SMS.Sum(s => s.DR),
                        CurryForward = carryforward.Sum(s => s.DR),
                        Others = Others.Sum(s => s.DR),
                        MIDPAY = MIDPAY.Sum(s => s.DR),
                        AI = ai.Sum(s => s.DR),
                        CLINICAL = clinical.Sum(s => s.DR),
                        Tractor = tractor.Sum(s => s.DR),
                        VARIANCE = variance.Sum(s => s.DR),
                        Agrovet = agrovet.Sum(s => s.DR),
                        Hshares = shares.Sum(s => s.DR),
                        Totaldeductions = Tot,
                        NetPay = grossPay - debits - Tot,
                        BankName = transporter.Bcode,
                        Yyear = period.EndDate.Year,
                        Mmonth = period.EndDate.Month,
                        AccNo = transporter.Accno,
                        BBranch = transporter.Bbranch,
                        EndPeriod = period.EndDate,
                        Rate = 0,
                        SaccoCode = sacco,
                        AuditId = loggedInUser,
                        Branch = transporter.Tbranch
                    });
                }
            });

            _context.SaveChanges();
            //await _context.SaveChangesAsync();
            _notyf.Success("Payroll processed successfully");
            return RedirectToAction(nameof(Index));
        }
        private void deleteTranspoterIntakes(DateTime startDate, DateTime endDate, string sacco, string branchName, string loggedInUser)
        {
            IQueryable<ProductIntake> productIntakeslist = _context.ProductIntake;
            var supp = _context.DSuppliers.Where(n => n.Scode.ToUpper().Equals(sacco.ToUpper())).Select(b=>b.Sno).ToList();

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
        
        private async Task CalcDefaultdeductions(DateTime startDate, DateTime endDate, List<ProductIntake> productIntakeslist)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var dcodes = await _context.DDcodes.Where(d => d.Dcode == sacco).ToListAsync();

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if (StrValues.Elburgon != sacco)
            {

                var productIntakes = productIntakeslist.Where(p => p.Branch.ToUpper().Equals(saccoBranch.ToUpper())).ToList();
                var intakes = productIntakes.GroupBy(p => p.Sno.Trim().ToUpper()).ToList();
                var preSets = await _context.d_PreSets.Where(l => !l.Stopped && l.saccocode == sacco).ToListAsync();
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    preSets = preSets.Where(p => p.BranchCode == saccoBranch).ToList();
                var shares = await _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                && f.TransDate >= startDate && f.TransDate <= endDate && f.Type != "Checkoff" && f.Pmode != "Checkoff").ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    shares = shares.Where(p => p.Branch == saccoBranch).ToList();

               
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

                                            _context.DShares.Add(new DShare
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
                                    _context.ProductIntake.Add(new ProductIntake
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
                                _context.Gltransactions.Add(new Gltransaction
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
            {//n.Status1 &&
                //var getsuppliers = _context.DSuppliers.Where(n => n.Scode == sacco  && n.Branch == saccoBranch).ToList().GroupBy(b => b.Sno.ToUpper()).Distinct().ToList();
                var getsuppliers = _context.d_PreSets.Where(l => !l.Stopped && l.saccocode == sacco && l.BranchCode == saccoBranch).ToList().GroupBy(b => b.Sno.ToUpper()).Distinct().ToList();
                getsuppliers.ForEach(n =>
                {
                    var supplierDetails = n.FirstOrDefault();
                    var kilos = productIntakeslist.Where(l =>l.TransDate>= startDate  && l.TransDate<= endDate && l.Sno.ToUpper().Equals(supplierDetails.Sno.ToUpper()) && l.TransactionType == TransactionType.Intake || l.TransactionType == TransactionType.Correction).Sum(w => w.Qsupplied);
                    if (kilos > 0)
                    {
                        var totalshare = _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.Branch == saccoBranch && f.Sno.ToUpper().Equals(supplierDetails.Sno.ToUpper())).Sum(n => n.Amount);
                    if (totalshare < 20000)
                    {
                        _context.DShares.Add(new DShare
                        {
                            Sno = supplierDetails.Sno.Trim().ToUpper(),
                            Type = "Checkoff",
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

                        var getpricegls = _context.DPrices.FirstOrDefault(j => j.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
                        _context.ProductIntake.Add(new ProductIntake
                        {
                            Sno = supplierDetails.Sno.Trim().ToUpper(),
                            TransDate = (DateTime)endDate,
                            TransTime = DateTime.Now.TimeOfDay,
                            ProductType = getpricegls.Products,
                            Qsupplied = 0,
                            Ppu = 0,
                            CR = 0,
                            DR = (decimal)kilos,
                            Balance = 0,
                            Description = "SHARES",
                            TransactionType = TransactionType.Deduction,
                            Remarks = "",
                            AuditId = loggedInUser,
                            Auditdatetime = DateTime.Now,
                            Branch = saccoBranch,
                            SaccoCode = sacco,
                            DrAccNo = getpricegls.TransportCrAccNo,
                            CrAccNo = getpricegls.TransportDrAccNo,

                        });

                        var glsforbonus = dcodes.FirstOrDefault(m => m.Description.ToUpper().Equals("Shares".ToUpper()));
                        _context.Gltransactions.Add(new Gltransaction
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
                });
            }
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
                var supplierIntakes = transporterIntakes.GroupBy(s => s.Sno).ToList();
                supplierIntakes.ForEach(s =>
                {
                    var intake = s.FirstOrDefault();
                    var product = intake?.ProductType ?? "";
                    var price = prices.FirstOrDefault(s => s.Products.ToUpper().Equals(product.ToUpper()));
                    decimal? cr = 0;
                    var framersTotal = s.Sum(t => t.Qsupplied);
                    var dr = framersTotal  * intake.Rate;
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
                            Remarks = "Transport",
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
                    if(intake.Rate > 0)
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
                        Remarks = "Intake for" + intake.Sno,
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
            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var startDate = new DateTime(period.Year, period.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var nextMonth = startDate.AddMonths(1);
            var productty = "Carry Forward";
            var thismonthdescription = "Carry Forward";
            var forlastmonthremarks = "Carry Forward";

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if (StrValues.Elburgon == sacco)
            {
                productty = "Others";
                thismonthdescription = endDate.Month.ToString() + endDate.Year.ToString() + "Arrears";
                forlastmonthremarks = endDate.Month.ToString() + endDate.Year.ToString() + "Arrears CF";
            }

            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco).ToList();
            suppliers.ForEach(s =>
            {
                var payrolls = _context.DPayrolls.Where(p => p.SaccoCode == sacco
                && p.EndofPeriod == endDate && p.Sno.ToUpper().Equals(s.Sno.ToUpper()) && p.Branch.ToUpper().Equals(s.Branch.ToUpper())).ToList();

                var netPay = payrolls.Sum(p => p.Npay);
                var debited = _context.ProductIntake.Any(i => i.SaccoCode == sacco && i.Sno.ToUpper().Equals(s.Sno.ToUpper())
                && i.TransDate >= nextMonth && i.Branch.ToUpper().Equals(s.Branch.ToUpper()) && i.Description == "Carry Forward" );
                if (netPay < 0 && !debited)
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
                        CR = netPay,
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
                        DR = netPay,
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
                }
            });

            var Transporters = _context.DTransporters.Where(s => s.ParentT == sacco).ToList();
            Transporters.ForEach(s =>
            {
                var payrolls = _context.DTransportersPayRolls.Where(p => p.SaccoCode == sacco
                && p.EndPeriod == endDate && p.Code.ToUpper().Equals(s.TransCode.ToUpper()) && p.Branch.ToUpper().Equals(s.Tbranch.ToUpper())).ToList();

                var netPay = payrolls.Sum(p => p.NetPay);
                var debited = _context.ProductIntake.Any(i => i.SaccoCode == sacco && i.Sno.ToUpper().Equals(s.TransCode.ToUpper())
                && i.TransDate >= nextMonth && i.Description == "Carry Forward" && i.Branch.ToUpper().Equals(s.Tbranch.ToUpper()));
                if (netPay < 0 && !debited)
                {
                    //INSERT TO LAST MONTH
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = s.TransCode.ToString(),
                        TransDate = endDate,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = productty,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = netPay,
                        DR = 0,
                        Balance = 0,
                        Description = "Carry Forward",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = forlastmonthremarks,
                        AuditId = auditId,
                        Auditdatetime = DateTime.Now,
                        Branch = s.Tbranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                    //INSERT TO NEXT MONTH
                    _context.ProductIntake.Add(new ProductIntake
                    {
                        Sno = s.TransCode.ToString(),
                        TransDate = nextMonth,
                        TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay,
                        ProductType = productty,
                        Qsupplied = 0,
                        Ppu = 0,
                        CR = netPay,
                        DR = 0,
                        Balance = 0,
                        Description = "Carry Forward",
                        TransactionType = TransactionType.Deduction,
                        Paid = false,
                        Remarks = thismonthdescription,
                        AuditId = auditId,
                        Auditdatetime = DateTime.Now,
                        Branch = s.Tbranch,
                        SaccoCode = sacco,
                        DrAccNo = "",
                        CrAccNo = "",
                        Posted = false
                    });
                }
            });

            _context.SaveChanges();
            _notyf.Success("Curry Forward processed successfully");
            return Json("");
        }
    }
}
