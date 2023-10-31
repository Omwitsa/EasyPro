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
using EasyPro.Models.BosaModels;
using Syncfusion.EJ2.Diagrams;

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
            var startDate = new DateTime(period.EndDate.Year, period.EndDate.Month, 1);
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var payrolls = await _context.DPayrolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var transportersPayRolls = await _context.DTransportersPayRolls
                .Where(p => p.Yyear == period.EndDate.Year && p.Mmonth == period.EndDate.Month
                && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var activeorders = await _context.StandingOrder.Where(o => o.SaccoCode == sacco && o.Paid < o.Amount && !o.Status).ToListAsync();
            var productIntakeslist = await _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate
            && i.TransDate <= period.EndDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            {
                payrolls = payrolls.Where(p => p.Branch == saccoBranch).ToList();
                transportersPayRolls = transportersPayRolls.Where(p => p.Branch == saccoBranch).ToList();
                productIntakeslist = productIntakeslist.Where(p => p.Branch == saccoBranch).ToList();
                activeorders = activeorders.Where(o => o.Branch == saccoBranch).ToList();
            }
                
            if (payrolls.Any())
                _context.DPayrolls.RemoveRange(payrolls);
            if (transportersPayRolls.Any())
                _context.DTransportersPayRolls.RemoveRange(transportersPayRolls);

            if (sacco != StrValues.Mburugu && sacco != StrValues.Slopes)
            {
                var transpoterIntakes = productIntakeslist.Where(i => i.Description.ToLower().Equals("transport")).ToList();
                if (transpoterIntakes.Any())
                    _context.ProductIntake.RemoveRange(transpoterIntakes);

                var checkifanydeduction = productIntakeslist.Where(n => n.Description.ToLower().Contains("midpay")).ToList();
                if (checkifanydeduction.Any())
                    _context.ProductIntake.RemoveRange(checkifanydeduction);

                var deletesubsidy = productIntakeslist.Where(i => i.ProductType == "SUBSIDY");
                if (deletesubsidy.Any())
                    _context.ProductIntake.RemoveRange(deletesubsidy);
            }

            var deletestandingorder = productIntakeslist.Where(i => i.Remarks == "Standing Order");
            if (deletestandingorder.Any())
                _context.ProductIntake.RemoveRange(deletestandingorder);

            var preSets = await _context.d_PreSets.Where(b => b.saccocode == sacco).ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                preSets = preSets.Where(p => p.BranchCode == saccoBranch).ToList();
            var selectdistinctdedname = preSets.Select(b => b.Deduction.ToUpper()).Distinct();
            foreach (var dedtype in selectdistinctdedname)
            {
                var deletesdefaultded = productIntakeslist.Where(i => i.ProductType.ToUpper().Equals(dedtype.ToUpper())).ToList();
                if (StrValues.Elburgon == sacco)
                    deletesdefaultded = deletesdefaultded.Where(d=>d.AuditId != "admin").ToList();

                if (deletesdefaultded.Any())
                    _context.ProductIntake.RemoveRange(deletesdefaultded);
            }

            var checkgls = await _context.Gltransactions
               .Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.TransDate >= startDate && f.TransDate <= period.EndDate
               && (f.TransDescript.ToUpper().Equals("Bonus".ToUpper()) || f.TransDescript.ToUpper().Equals("Shares".ToUpper()))).ToListAsync();

            if (user.AccessLevel == AccessLevel.Branch)
                checkgls = checkgls.Where(p => p.Branch == saccoBranch).ToList();
            if (checkgls.Any())
                _context.Gltransactions.RemoveRange(checkgls);
            var checksharespaid = await _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                            && f.TransDate >= startDate && f.TransDate <= period.EndDate && f.Type.ToUpper().Equals("SHARES")
                            && f.Pmode == "Checkoff").ToListAsync();
            if (user.AccessLevel == AccessLevel.Branch)
                checksharespaid = checksharespaid.Where(p => p.Branch == saccoBranch).ToList();
            if (checksharespaid.Any())
                _context.DShares.RemoveRange(checksharespaid);

            if (sacco != StrValues.Mburugu && sacco != StrValues.Slopes)
                await ConsolidateTranspoterIntakes(startDate, period.EndDate, productIntakeslist);

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if (StrValues.Elburgon != sacco)
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

            var societyStandingOrders = productIntakeslist.Where(i => i.Remarks == "Society Standing Order").ToList();
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

            if(orderDeductions.Any())
                await _context.ProductIntake.AddRangeAsync(orderDeductions);
            await _context.SaveChangesAsync();
            productIntakeslist = await _context.ProductIntake.Where(i => i.SaccoCode == sacco && i.TransDate >= startDate
            && i.TransDate <= period.EndDate).ToListAsync();
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
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(t => t.Branch == saccoBranch).ToList();

            var price = _context.DPrices.FirstOrDefault(p => p.SaccoCode == sacco);
            var supplierNos = suppliers.Select(s => s.Sno);
            var supplierIntakes = productIntakeslist.Where(p => supplierNos.Contains(p.Sno)).ToList();
            var intakes = supplierIntakes.GroupBy(p => p.Sno.ToUpper()).ToList();

            var loans = _bosaDbContext.LOANBAL.Where(l => l.Balance > 0 && l.LastDate <= monthsLastDate && l.Companycode == StrValues.SlopesCode).ToList();
            var saccoStandingOrders = _bosaDbContext.CONTRIB_standingOrder.Where(o => o.CompanyCode == StrValues.SlopesCode);
            var contribs = _bosaDbContext.CONTRIB.Where(c => c.CompanyCode == StrValues.SlopesCode).ToList();
            var listSaccoLoans = new List<SaccoLoans>();
            var listSaccoShares = new List<SaccoShares>();

            var dPayrolls = new List<DPayroll>();
            intakes.ForEach(p =>
            {
                var advance = p.Where(k => k.ProductType.ToLower().Contains("advance"));
                var transport = p.Where(k => k.Description.ToLower().Contains("transport"));
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
                        var getCrDr  = p.Where(s => s.Description == "Intake" || s.Description == "Correction");
                        var getDebitsCrDr  = p.Where(s => s.Description != "Intake" || s.Description != "Correction");
                        grossPay = getCrDr.Where(n=>n.CR>0).Sum(x=>x.CR)- getCrDr.Where(n => n.DR > 0).Sum(x => x.DR);
                        debits = getDebitsCrDr.Where(n => n.DR > 0).Sum(x => x.DR) - getDebitsCrDr.Where(n => n.CR > 0).Sum(x => x.CR);
                        subsidy =0;
                    }

                   if (supplier.TransCode == "Weekly" || (supplier.TransCode == "Monthly" && period.EndDate == monthsLastDate))
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
                        var regis = (registration.Sum(s => s.DR) - registration.Sum(s => s.CR));
                        if(StrValues.Slopes == sacco)
                        {
                            payroll.Registration = netPay > regis ? regis : netPay;
                            payroll.Registration = payroll.Registration > 0 ? payroll.Registration : 0;
                        }
                        else
                        {
                            payroll.Registration = regis;
                        }
                        
                        netPay -= regis;

                        var shar = (shares.Sum(s => s.DR) - shares.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Hshares = netPay > shar ? shar : netPay;
                            payroll.Hshares = payroll.Hshares > 0 ? payroll.Hshares : 0;
                        }
                        else
                        {
                            payroll.Hshares = shar;
                        }
                            
                        netPay -= shar;

                        payroll.KIIGA = netPay > kiiga.Sum(s => s.DR) ? kiiga.Sum(s => s.DR) : netPay;
                        payroll.KIIGA = payroll.KIIGA > 0 ? payroll.KIIGA : 0;
                        netPay -= kiiga.Sum(s => s.DR);
                        
                        payroll.KIROHA = netPay > kiroha.Sum(s => s.DR) ? kiroha.Sum(s => s.DR) : netPay;
                        payroll.KIROHA = payroll.KIROHA > 0 ? payroll.KIROHA : 0;
                        netPay -= (kiroha.Sum(s => s.DR)- kiroha.Sum(s => s.CR));

                        var AIs = (ai.Sum(s => s.DR) - ai.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.AI = netPay > AIs ? AIs : netPay;
                            payroll.AI = payroll.AI > 0 ? payroll.AI : 0;
                        }
                        else
                        {
                            payroll.AI = AIs;
                        }
                            
                        netPay -= AIs;

                        var agro = (agrovet.Sum(s => s.DR) - agrovet.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Agrovet = netPay > agro ? agro : netPay;
                            payroll.Agrovet = payroll.Agrovet > 0 ? payroll.Agrovet : 0;
                        }
                        else
                        {
                            payroll.Agrovet = agro;
                        }
                            
                        netPay -= agro;

                        payroll.NOV_OVPMNT = netPay > overpayment.Sum(s => s.DR) ? overpayment.Sum(s => s.DR) : netPay;
                        payroll.NOV_OVPMNT = payroll.NOV_OVPMNT > 0 ? payroll.NOV_OVPMNT : 0;
                        netPay -= (overpayment.Sum(s => s.DR)- overpayment.Sum(s => s.CR));

                        var carryfw = (carryforward.Sum(s => s.DR) - carryforward.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.CurryForward = netPay > carryfw ? carryfw : netPay;
                            payroll.CurryForward = payroll.CurryForward > 0 ? payroll.CurryForward : 0;
                        }
                        else
                        {
                            payroll.CurryForward = carryfw;
                        }
                           
                        netPay -= carryfw;

                        var adva = (advance.Sum(s => s.DR) - advance.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Advance = netPay > adva ? adva : netPay;
                            payroll.Advance = payroll.Advance > 0 ? payroll.Advance : 0;
                        }
                        else
                        {
                            payroll.Advance = adva;
                        }
                           
                        netPay -= adva;

                        payroll.INST_ADVANCE = netPay > instantAdvance.Sum(s => s.DR) ? instantAdvance.Sum(s => s.DR) : netPay;
                        payroll.INST_ADVANCE = payroll.INST_ADVANCE > 0 ? payroll.INST_ADVANCE : 0;
                        netPay -= (instantAdvance.Sum(s => s.DR)- instantAdvance.Sum(s => s.CR));

                        var trans = (transport.Sum(s => s.DR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Transport = netPay > trans ? trans : netPay;
                            payroll.Transport = payroll.Transport > 0 ? payroll.Transport : 0;
                        }
                        else
                        {
                            payroll.Transport = trans;
                        }
                            
                        netPay -= trans;
                        
                        var clini = (clinical.Sum(s => s.DR) - clinical.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.CLINICAL = netPay > clini ? clini : netPay;
                            payroll.CLINICAL = payroll.CLINICAL > 0 ? payroll.CLINICAL : 0;
                        }
                        else
                        {
                            payroll.CLINICAL = clini;
                        }
                           
                        netPay -= clini;

                        var Tract = (tractor.Sum(s => s.DR) - tractor.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Tractor = netPay > Tract ? Tract : netPay;
                            payroll.Tractor = payroll.Tractor > 0 ? payroll.Tractor : 0;
                        }
                        else
                        {
                            payroll.Tractor = Tract;
                        }
                           
                        netPay -= Tract;

                        payroll.extension = netPay > extension.Sum(s => s.DR) ? extension.Sum(s => s.DR) : netPay;
                        payroll.extension = payroll.extension > 0 ? payroll.extension : 0;
                        netPay -= (extension.Sum(s => s.DR)- extension.Sum(s => s.CR));

                        payroll.SMS = netPay > SMS.Sum(s => s.DR) ? SMS.Sum(s => s.DR) : netPay;
                        payroll.SMS = payroll.SMS > 0 ? payroll.SMS : 0;
                        netPay -= (SMS.Sum(s => s.DR)- SMS.Sum(s => s.CR));

                        payroll.Bonus = netPay > bonus.Sum(s => s.DR) ? bonus.Sum(s => s.DR) : netPay;
                        payroll.Bonus = payroll.Bonus > 0 ? payroll.Bonus : 0;
                        netPay -= (bonus.Sum(s => s.DR)- bonus.Sum(s => s.CR));

                        payroll.MIDPAY = netPay > MIDPAY.Sum(s => s.DR) ? MIDPAY.Sum(s => s.DR) : netPay;
                        payroll.MIDPAY = payroll.MIDPAY > 0 ? payroll.MIDPAY : 0;
                        netPay -= (MIDPAY.Sum(s => s.DR)- MIDPAY.Sum(s => s.CR));

                        var ECLO = (ECLOF.Sum(s => s.DR) - ECLOF.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.ECLOF = netPay > ECLO ? ECLO : netPay;
                            payroll.ECLOF = payroll.ECLOF > 0 ? payroll.ECLOF : 0;
                        }
                        else
                        {
                            payroll.ECLOF = ECLO;
                        }
                            
                        netPay -= ECLO;

                        var Maendeleo = (saccoDed.Sum(s => s.DR) - saccoDed.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.saccoDed = netPay > Maendeleo ? Maendeleo : netPay;
                            payroll.saccoDed = payroll.saccoDed > 0 ? payroll.saccoDed : 0;
                        }
                        else
                        {
                            payroll.saccoDed = Maendeleo;
                        }
                            
                        netPay -= Maendeleo;

                        payroll.MILK_RECOVERY = netPay > milkRecovery.Sum(s => s.DR) ? milkRecovery.Sum(s => s.DR) : netPay;
                        payroll.MILK_RECOVERY = payroll.MILK_RECOVERY > 0 ? payroll.MILK_RECOVERY : 0;
                        netPay -= (milkRecovery.Sum(s => s.DR)- milkRecovery.Sum(s => s.CR));

                        var other = (Others.Sum(s => s.DR) - Others.Sum(s => s.CR));
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Others = netPay > other ? other : netPay;
                            payroll.Others = payroll.Others > 0 ? payroll.Others : 0;
                        }
                        else
                        {
                            payroll.Others = other;
                        }
                           
                        netPay -= other;

                        var memberLoans = loan.Sum(s => s.DR) - loan.Sum(s => s.CR);
                        if (StrValues.Slopes == sacco)
                        {
                            payroll.Fsa = netPay > memberLoans ? memberLoans : netPay;
                            payroll.Fsa = payroll.Fsa > 0 ? payroll.Fsa : 0;
                        }
                        else
                        {
                            payroll.Fsa = memberLoans;
                        }
                           
                        netPay -= memberLoans;
                        if (StrValues.Slopes == sacco)
                        {
                            var farmerLoans = loans.Where(l => l.MemberNo.ToUpper().Equals(supplier.Sno.ToUpper())).ToList();
                            //var types = _bosaDbContext.LOANTYPE.Where(t => t.CompanyCode == StrValues.SlopesCode).ToList();
                            farmerLoans.ForEach(l =>
                            {
                                if(l.Installments > 0 && netPay > 0)
                                {
                                    var installments = l.Balance < l.Installments ? l.Balance : l.Installments;
                                    installments = netPay > installments ? installments : netPay;
                                    payroll.Fsa += installments;
                                    netPay -= installments;
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

                            payroll.Fsa = netPay > listSaccoLoans.Sum(l => l.Amount) ? listSaccoLoans.Sum(l => l.Amount) : netPay;
                            payroll.Fsa = payroll.Fsa > 0 ? payroll.Fsa : 0;

                            var saccoStandingOrder = saccoStandingOrders.FirstOrDefault(o => o.MemberNo.ToUpper().Equals(supplier.Sno.ToUpper()));
                            if (saccoStandingOrder != null)
                            {
                                payroll.SACCO_SHARES = netPay > saccoStandingOrder.Installment ? saccoStandingOrder.Installment : netPay;
                                payroll.SACCO_SHARES = payroll.SACCO_SHARES > 0 ? payroll.SACCO_SHARES : 0;
                                netPay -= saccoStandingOrder.Installment;
                                if(payroll.SACCO_SHARES > 0)
                                    listSaccoShares.Add(new SaccoShares
                                    {
                                        SharesCode = saccoStandingOrder.Sharescode,
                                        Sno = saccoStandingOrder.MemberNo,
                                        Amount = payroll.SACCO_SHARES,
                                        TransDate = monthsLastDate,
                                        AuditDate = DateTime.Now,
                                        Saccocode = sacco,
                                        AuditId = loggedInUser
                                    });

                                decimal saccoSavings = 0;
                                payroll.SACCO_SAVINGS = netPay > saccoSavings ? saccoSavings : netPay;
                                payroll.SACCO_SAVINGS = payroll.SACCO_SAVINGS > 0 ? payroll.SACCO_SAVINGS : 0;
                                netPay -= saccoSavings;
                            }
                        }

                        payroll.Tdeductions = grossPay - netPay;
                        if (StrValues.Slopes == sacco)
                            payroll.Tdeductions = payroll.Tdeductions > grossPay ? grossPay : payroll.Tdeductions;
                        //netPay -= debits;
                        payroll.Npay = netPay;
                        dPayrolls.Add(payroll);
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
           
            var transporters = await _context.DTransporters.Where(s => s.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            if (StrValues.Slopes == sacco)
            {
                if (user.AccessLevel == AccessLevel.Branch)
                    transporters = transporters.Where(p => p.Tbranch == saccoBranch).ToList();
            }
            else
            {
                transporters = transporters.Where(p => p.Tbranch == saccoBranch).ToList();
            }
            var transpoterCodes = transporters.Select(s => s.TransCode.Trim().ToUpper()).ToList();
            var productIntakes = productIntakeslist.Where(p => transpoterCodes.Contains(p.Sno.Trim().ToUpper())).ToList();
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
                var corrections = p.Where(k => k.TransactionType == TransactionType.Correction);
                var milk = p.Where(k => (k.TransactionType == TransactionType.Correction || k.TransactionType == TransactionType.Intake));

                //var payroll = new DTransportersPayRoll();
                var transporter = transporters.FirstOrDefault(s => s.TransCode.ToUpper().Equals(p.Key.ToUpper()));
                if (transporter != null)
                {
                    var debits = corrections.Sum(s => s.DR);
                    //var amount = p.Sum(s => s.CR);
                    var amount = (p.Where(K=>K.ProductType == "Transport").Sum(s => s.CR)- p.Where(K => K.ProductType == "Transport").Sum(s => s.DR));

                    var totalSupplied = p.Sum(s => s.Qsupplied);
                    decimal subsidy = 0;
                    if (StrValues.Slopes == sacco)
                    {
                        amount = totalSupplied * (decimal)transporter.Rate;
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

                    var saccodeductions = (saccoDed.Sum(s => s.DR) - saccoDed.Sum(s => s.CR));

                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.saccoDed = netPay > saccodeductions ? saccodeductions : netPay;
                        payRoll.saccoDed = payRoll.saccoDed > 0 ? payRoll.saccoDed : 0;
                    }
                    else
                    {
                        payRoll.saccoDed = saccodeductions;
                    }
                        
                    netPay -= saccodeductions;

                    var eclofdeduction = (ECLOF.Sum(s => s.DR) - ECLOF.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.ECLOF = netPay > eclofdeduction ? eclofdeduction : netPay;
                        payRoll.ECLOF = payRoll.ECLOF > 0 ? payRoll.ECLOF : 0;
                    }
                    else
                    {
                        payRoll.ECLOF = eclofdeduction;
                    }
                        
                    netPay -= eclofdeduction;

                    var variancededuction = (variance.Sum(s => s.DR) - variance.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.VARIANCE = netPay > variancededuction ? variancededuction : netPay;
                        payRoll.VARIANCE = payRoll.VARIANCE > 0 ? payRoll.VARIANCE : 0;
                    }
                    else
                    {
                    payRoll.VARIANCE = variancededuction;
                    }
                        
                    netPay -= variancededuction;

                    var agrovetdeduction = (agrovet.Sum(s => s.DR) - agrovet.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.Agrovet = netPay > agrovetdeduction ? agrovetdeduction : netPay;
                        payRoll.Agrovet = payRoll.Agrovet > 0 ? payRoll.Agrovet : 0;
                    }
                    else
                    {
                        payRoll.Agrovet = agrovetdeduction;
                    }

                        
                    netPay -= agrovetdeduction;

                    var advancededuction = (advance.Sum(s => s.DR) - advance.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.Advance = netPay > advancededuction ? advancededuction : netPay;
                        payRoll.Advance = payRoll.Advance > 0 ? payRoll.Advance : 0;
                    }
                    else
                    {
                        payRoll.Advance = advancededuction;
                    }
                       
                    netPay -= advancededuction;

                    payRoll.INST_ADVANCE = netPay > instantAdvance.Sum(s => s.DR) ? instantAdvance.Sum(s => s.DR) : netPay;
                    payRoll.INST_ADVANCE = payRoll.INST_ADVANCE > 0 ? payRoll.INST_ADVANCE : 0;
                    netPay -= instantAdvance.Sum(s => s.DR);

                    payRoll.extension = netPay > extension.Sum(s => s.DR) ? extension.Sum(s => s.DR) : netPay;
                    payRoll.extension = payRoll.extension > 0 ? payRoll.extension : 0;
                    netPay -= extension.Sum(s => s.DR);

                    payRoll.SMS = netPay > SMS.Sum(s => s.DR) ? SMS.Sum(s => s.DR) : netPay;
                    payRoll.SMS = payRoll.SMS > 0 ? payRoll.SMS : 0;
                    netPay -= SMS.Sum(s => s.DR);

                    var CurryForwarddeduction = (carryforward.Sum(s => s.DR) - carryforward.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.CurryForward = netPay > CurryForwarddeduction ? CurryForwarddeduction : netPay;
                        payRoll.CurryForward = payRoll.CurryForward > 0 ? payRoll.CurryForward : 0;
                    }
                    else
                    {
                        payRoll.CurryForward = CurryForwarddeduction;
                    }
                        
                    netPay -= CurryForwarddeduction;

                    var midpaydeduction = (MIDPAY.Sum(s => s.DR) - MIDPAY.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.MIDPAY = netPay > midpaydeduction ? midpaydeduction : netPay;
                        payRoll.MIDPAY = payRoll.MIDPAY > 0 ? payRoll.MIDPAY : 0;
                    }
                    else
                    {
                        payRoll.MIDPAY = midpaydeduction;
                    }
                        
                    netPay -= midpaydeduction;

                    var AIDeduction = (ai.Sum(s => s.DR) - ai.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.AI = netPay > AIDeduction ? AIDeduction : netPay;
                        payRoll.AI = payRoll.AI > 0 ? payRoll.AI : 0;
                    }
                    else
                    {
                        payRoll.AI = AIDeduction;
                    }
                       
                    netPay -= AIDeduction;

                    var CLINICALdeduction = (clinical.Sum(s => s.DR) - clinical.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.CLINICAL = netPay > CLINICALdeduction ? CLINICALdeduction : netPay;
                        payRoll.CLINICAL = payRoll.CLINICAL > 0 ? payRoll.CLINICAL : 0;
                    }
                    else
                    {
                        payRoll.CLINICAL = CLINICALdeduction;
                    }
                       
                    netPay -= CLINICALdeduction;

                    var tractordeduction = (tractor.Sum(s => s.DR) - tractor.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.Tractor = netPay > tractordeduction ? tractordeduction : netPay;
                        payRoll.Tractor = payRoll.Tractor > 0 ? payRoll.Tractor : 0;
                    }
                    else
                    {
                        payRoll.Tractor = tractordeduction;
                    }
                       
                    netPay -= tractordeduction;

                    var societyShares = (shares.Sum(s => s.DR) - shares.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.Hshares = netPay > societyShares ? societyShares : netPay;
                        payRoll.Hshares = payRoll.Hshares > 0 ? payRoll.Hshares : 0;
                    }
                    else
                    {
                        payRoll.Hshares = societyShares;
                    }
                       
                    netPay -= societyShares;

                    var memberLoans = loan.Sum(s => s.DR);
                    payRoll.Fsa = netPay > memberLoans ? memberLoans : netPay;
                    payRoll.Fsa = payRoll.Fsa > 0 ? payRoll.Fsa : 0;
                    netPay -= memberLoans;

                    var Othersdedutcion = (Others.Sum(s => s.DR)- Others.Sum(s => s.CR));
                    if (StrValues.Slopes == sacco)
                    {
                        payRoll.Others = netPay > Othersdedutcion ? Othersdedutcion : netPay;
                        payRoll.Others = payRoll.Others > 0 ? payRoll.Others : 0;
                    }
                    else
                    {
                        payRoll.Others = Othersdedutcion;
                    }
                       
                    netPay -= Othersdedutcion;

                    if (StrValues.Slopes == sacco)
                    {
                        var transportersLoans = loans.Where(l => l.MemberNo.ToUpper().Equals(transporter.TransCode.ToUpper())).ToList();
                        transportersLoans.ForEach(l =>
                        {
                            if (l.Installments > 0 && netPay > 0)
                            {
                                var installments = l.Balance < l.Installments ? l.Balance : l.Installments;
                                installments = netPay > installments ? installments : netPay;
                                payRoll.Fsa += installments;
                                netPay -= installments;
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

                        payRoll.Fsa = netPay > listSaccoLoans.Sum(l => l.Amount) ? listSaccoLoans.Sum(l => l.Amount) : netPay;
                        payRoll.Fsa = payRoll.Fsa > 0 ? payRoll.Fsa : 0;
                        var saccoStandingOrder = saccoStandingOrders.FirstOrDefault(o => o.MemberNo.ToUpper().Equals(transporter.TransCode.ToUpper()));
                        if (saccoStandingOrder != null)
                        {
                            payRoll.SACCO_SHARES = netPay > saccoStandingOrder.Installment ? saccoStandingOrder.Installment : netPay;
                            payRoll.SACCO_SHARES = payRoll.SACCO_SHARES > 0 ? payRoll.SACCO_SHARES : 0;
                            netPay -= saccoStandingOrder.Installment;
                            if (payRoll.SACCO_SHARES > 0)
                                listSaccoShares.Add(new SaccoShares
                                {
                                    SharesCode = saccoStandingOrder.Sharescode,
                                    Sno = saccoStandingOrder.MemberNo,
                                    Amount = payRoll.SACCO_SHARES,
                                    TransDate = monthsLastDate,
                                    AuditDate = DateTime.Now,
                                    Saccocode = sacco,
                                    AuditId = loggedInUser
                                });

                            decimal saccoSavings = 0;
                            payRoll.SACCO_SAVINGS = netPay > saccoSavings ? saccoSavings : netPay;
                            payRoll.SACCO_SAVINGS = payRoll.SACCO_SAVINGS > 0 ? payRoll.SACCO_SAVINGS : 0;
                            netPay -= saccoSavings;
                        }
                    }

                    payRoll.Totaldeductions = grossPay - netPay;
                        if (StrValues.Slopes == sacco)
                            payRoll.Totaldeductions = payRoll.Totaldeductions > grossPay ? grossPay : payRoll.Totaldeductions;
                    //netPay -= debits;
                    payRoll.NetPay = netPay;
                    dTransportersPayRolls.Add(payRoll);
                }
            });

            if (dTransportersPayRolls.Any())
                await _context.DTransportersPayRolls.AddRangeAsync(dTransportersPayRolls);
            if(!_context.SaccoLoans.Any(l => l.Saccocode == sacco && l.TransDate == monthsLastDate) && listSaccoLoans.Any())
                await _context.SaccoLoans.AddRangeAsync(listSaccoLoans);
            if(!_context.SaccoShares.Any(s => s.Saccocode == sacco && s.TransDate == monthsLastDate) && listSaccoShares.Any())
                await _context.SaccoShares.AddRangeAsync(listSaccoShares);

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

        private async Task CalcDefaultdeductions(DateTime startDate, DateTime endDate, List<ProductIntake> productIntakeslist)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var dcodes = await _context.DDcodes.Where(d => d.Dcode == sacco).ToListAsync();
            IQueryable<ProductIntake> productIntakes1 = _context.ProductIntake;
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
                /// FOR ELBURGON PROGRESIVE DAIRY ONLY
                //var getsuppliers = _context.DSuppliers.Where(n => n.Scode == sacco  && n.Branch == saccoBranch).ToList().GroupBy(b => b.Sno.ToUpper()).Distinct().ToList();
                var getsuppliers = _context.d_PreSets.Where(l => !l.Stopped && l.saccocode == sacco && l.BranchCode == saccoBranch).ToList().GroupBy(b => b.Sno.ToUpper()).Distinct().ToList();
                getsuppliers.ForEach(n =>
                {
                    var supplierDetails = n.FirstOrDefault();

                    var Selectdetails = productIntakes1.Where(l => (l.TransDate >= startDate && l.TransDate <= endDate) && l.SaccoCode == sacco
                    && l.Branch == saccoBranch && l.Sno.ToUpper().Equals(supplierDetails.Sno.ToUpper())
                    && (l.TransactionType == TransactionType.Intake || l.TransactionType == TransactionType.Correction)).ToList();

                    var kilos = Selectdetails.Sum(w => w.Qsupplied);
                    if (kilos > 0)
                    {
                        var totalshare = _context.DShares.Where(f => f.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && f.Branch == saccoBranch && f.Sno.ToUpper().Equals(supplierDetails.Sno.ToUpper())).Sum(n => n.Amount);
                        if (totalshare < 20000)
                        {
                            var checkifhasnegative = Selectdetails.Sum(f => f.CR) - Selectdetails.Sum(g => g.DR);
                            if (checkifhasnegative > 0)
                            {
                                _context.DShares.Add(new DShare
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

                                var getpricegls = _context.DPrices.FirstOrDefault(j => j.SaccoCode.ToUpper().Equals(sacco.ToUpper()));
                                _context.ProductIntake.Add(new ProductIntake
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
            IQueryable<DPayroll> dPayrolls = _context.DPayrolls;
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            IQueryable<DTransportersPayRoll> dTransportersPayRolls = _context.DTransportersPayRolls;
            IQueryable<DSupplier> dSuppliers = _context.DSuppliers;
            IQueryable<DTransporter> dTransporters = _context.DTransporters;

            ViewBag.isElburgon = StrValues.Elburgon == sacco;
            if (StrValues.Elburgon == sacco)
            {
                productty = "Others";
                thismonthdescription = endDate.Month.ToString() + endDate.Year.ToString() + "Arrears";
                forlastmonthremarks = endDate.Month.ToString() + endDate.Year.ToString() + "Arrears CF";
            }

            var suppliers = dPayrolls.Where(s => s.SaccoCode == sacco && s.EndofPeriod == endDate && s.Npay<0).ToList();
            suppliers.ForEach(s =>
            {
                var payrolls = dPayrolls.Where(p => p.SaccoCode == sacco
                && p.EndofPeriod == endDate && p.Sno.ToUpper().Equals(s.Sno.ToUpper()) && p.Branch.ToUpper().Equals(s.Branch.ToUpper())).ToList();

                var netPay = payrolls.Sum(p => p.Npay);
                var debited = productIntakes.Any(i => i.SaccoCode == sacco && i.Sno.ToUpper().Equals(s.Sno.ToUpper())
                && i.TransDate >= nextMonth && i.Branch.ToUpper().Equals(s.Branch.ToUpper()) && i.Description == "Carry Forward");
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
                        CR = netPay * -1,
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
                        DR = netPay*-1,
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

            var Transporters = dTransporters.Where(s => s.ParentT == sacco).ToList();
            Transporters.ForEach(s =>
            {
                var payrolls = dTransportersPayRolls.Where(p => p.SaccoCode == sacco
                && p.EndPeriod == endDate && p.Code.ToUpper().Equals(s.TransCode.ToUpper()) && p.Branch.ToUpper().Equals(s.Tbranch.ToUpper())).ToList();

                var netPay = payrolls.Sum(p => p.NetPay);
                var debited = productIntakes.Any(i => i.SaccoCode == sacco && i.Sno.ToUpper().Equals(s.TransCode.ToUpper())
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
                        CR = netPay*-1,
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
                        CR = 0,
                        DR = netPay * -1,
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
