using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EasyPro.Models;
using EasyPro.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.ViewModels;
using EasyPro.ViewModels.EnquiryVM;
using System.Collections.Generic;
using static EasyPro.ViewModels.AccountingVm;
using System.Globalization;

namespace EasyPro.Controllers
{
    public class SupplierEnquiryController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private List<DSupplier> dsuppliers;

        public SupplierEnquiryController(MORINGAContext context)
        {
            _context = context;
            utilities = new Utilities(context);
        }
        public TransportersEnquiryVM TransportersEnquiryVMobj { get; set; }
        public DTmpTransEnquery DTmpTransEnqueryobj { get; set; }
        public IActionResult Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            GetInitialValues();
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            var suppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(i => i.Branch == saccobranch);

            ViewBag.suppliers = suppliers.Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
                IdNo = s.IdNo,
                PhoneNo = s.PhoneNo,
                AccNo = s.AccNo,
                Bbranch = s.Bbranch,
                Zone = s.Zone
            }).ToList();

            return View();
        }

        [HttpGet]
        public JsonResult SelectedDateIntake(string sno)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            dsuppliers = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco).ToList();

            ViewBag.suppliers = dsuppliers.Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
                IdNo = s.IdNo,
                PhoneNo = s.PhoneNo,
                AccNo = s.AccNo,
                Bbranch = s.Bbranch
            }).ToList();
            return Json(ViewBag.suppliers);
        }

        [HttpGet]
        public JsonResult SelectedName2(string sno)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var todaysIntake = _context.DSuppliers.Where(L => L.Sno == sno && L.Scode == sacco && L.Branch== saccoBranch);

            return Json(todaysIntake);
        }

        [HttpGet]
        public JsonResult Selectemaxloan(string sno)
        {
               utilities.SetUpPrivileges(this);
               var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
               var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            decimal Total = 0;

            //    var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //    var startDate = month.AddMonths(0);
            //    var endDate = month.AddMonths(1).AddDays(-1);

            //    var productIntakes = _context.ProductIntake.Where(M => M.TransDate >= startDate && M.Sno.ToUpper().Equals(sno.ToUpper()) && M.TransDate <= endDate && M.SaccoCode == sacco).ToList();
            //    decimal milk = 0, assets = 0, land = 0, crops = 0, eductaion = 0, animals = 0, Total = 0;
            //    var flmdanimalandeducation = _context.FLMD.FirstOrDefault(k => k.Sno.ToUpper().Equals(sno.ToUpper()) && k.SaccoCode == sacco);
            //    var flmdcrops = _context.FLMDCrops.FirstOrDefault(k => k.Sno.ToUpper().Equals(sno.ToUpper()) && k.SaccoCode == sacco);
            //    var flmdland = _context.FLMDLand.FirstOrDefault(k => k.Sno.ToUpper().Equals(sno.ToUpper()) && k.SaccoCode == sacco);

            //    if (productIntakes.Count > 0)
            //    {
            //        milk = (productIntakes.Sum(d => d.CR) ?? 0) - (productIntakes.Sum(d => d.DR) ?? 0);
            //    }

            //    //animals = (decimal)(((double)flmdanimalandeducation.Sum(g => g.ExoticCattle)*40000)+((double)flmdanimalandeducation.Sum(g => g.IndigenousCattle)*20000)+((double)flmdanimalandeducation.Sum(g => g.Sheep)*6000)+((double)flmdanimalandeducation.Sum(g => g.Goats)*5000)+((double)flmdanimalandeducation.Sum(g => g.Donkeys)*4000)+((double)flmdanimalandeducation.Sum(g => g.Pigs)*8000));
            //    if (flmdanimalandeducation != null)
            //    {
            //        animals = (decimal)(((double)(flmdanimalandeducation.ExoticCattle ?? 0) * 40000) + ((double)(flmdanimalandeducation.IndigenousCattle ?? 0) * 20000) + ((double)(flmdanimalandeducation.Sheep ?? 0) * 6000) + ((double)(flmdanimalandeducation.Goats ?? 0) * 5000) + ((double)(flmdanimalandeducation.Donkeys ?? 0) * 4000) + ((double)(flmdanimalandeducation.Pigs ?? 0) * 8000));
            //        eductaion = (decimal)(((double)(flmdanimalandeducation.Graduates ?? 0) * 120000) + ((double)(flmdanimalandeducation.UnderGraduates ?? 0) * 80000) + ((double)(flmdanimalandeducation.Secondary ?? 0) * 53000) + ((double)(flmdanimalandeducation.Primary ?? 0) * 26000));
            //    }

            //    //eductaion = (decimal)(((double)flmdanimalandeducation.Sum(g => g.Graduates) * 120000) + ((double)flmdanimalandeducation.Sum(g => g.UnderGraduates) * 80000) + ((double)flmdanimalandeducation.Sum(g => g.Secondary) * 53000) + ((double)flmdanimalandeducation.Sum(g => g.Primary) * 26000));
            //    if (flmdland != null)
            //    {
            //        crops = 120000;
            //    }

            //    //land = (decimal)((double)flmdland.Sum(g => g.TotalAcres) * 1200000);
            //    if (flmdland != null)
            //    {
            //        land = (decimal)((double)flmdland.TotalAcres * 1200000);
            //    }

            //    assets = (animals - eductaion + crops + land) * (decimal)(0.01);
               Total = 0;
            return Json(Total);
        }

        public async Task<IActionResult> Transporters()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            DateTime now = DateTime.Now;
            DateTime startDate = new DateTime(now.Year, now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);
            IQueryable<DTransporter> dTransporters = _context.DTransporters;
            var transporters = await dTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                transporters = transporters.Where(t => t.Tbranch == saccobranch).ToList();

            var codes = transporters.Select(t => t.TransCode).ToList();
            if (StrValues.Slopes == sacco)
                codes = transporters.OrderBy(t => t.CertNo).Select(t => t.CertNo).ToList();

            ViewBag.slopes = StrValues.Slopes == sacco;
            ViewBag.codes = new SelectList(codes);
            ViewBag.transporters = transporters.Select(s => new DTransporter
            {
                TransCode = s.TransCode,
                TransName = s.TransName,
                CertNo = s.CertNo,
                Phoneno = s.Phoneno,
                Bcode = s.Bcode,
                Accno = s.Accno,
                Bbranch = s.Bbranch
            }).ToList();
            return View();
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var products = _context.DBranchProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);

            var zones = _context.Zones.Where(a => a.Code == sacco).Select(b => b.Name).ToList();
            ViewBag.zones = new SelectList(zones);

            

            //if (zones.Count != 0)
            //    ViewBag.checkiftoenable = 1;
            //else
            //    ViewBag.checkiftoenable = 0;
        }

        private async Task Getshares(string sno)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var shares = await _context.DShares.Where(m => m.SaccoCode == sacco && m.Type.Contains("shares") && m.Sno.ToUpper().Equals(sno.ToUpper()))
                .ToListAsync();
            var sharesAmount = shares.Sum(x=>x.Amount);
            ViewBag.shares = sharesAmount;

            var transport = await _context.DTransports.FirstOrDefaultAsync(t => t.Sno.ToUpper().Equals(sno.ToUpper()) && t.saccocode == sacco && t.Branch == saccobranch);
            var trancode = transport?.TransCode ?? "";
            var transporter = await _context.DTransporters.FirstOrDefaultAsync(t => t.TransCode.ToUpper().Equals(trancode.ToUpper()) && t.ParentT == sacco && t.Tbranch == saccobranch);
            if (transporter == null)
                transporter = new DTransporter();
           ViewBag.Transportert = transporter.TransName;
        }
        //public async Task<dynamic> GenerateStatement(StatementFilter filter)
        //{
        //}

        [HttpPost]
        public async Task<JsonResult> SuppliedProducts([FromBody] DSupplier supplier, DateTime date1, DateTime date2, string producttype, string sno)
        {
            supplier.Sno = supplier?.Sno ?? "";
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            var val = _context.ProductIntake.Where(n => n.Sno == supplier.Sno).ToList();
            //var statementResp = await GenerateStatement(supplier.Sno, date1, DateTime date2);
            var intakes = productIntakes.Where(i => i.Sno.ToUpper().Equals(supplier.Sno.ToUpper())
            && i.SaccoCode == sacco && ( i.TransDate >= date1 && i.TransDate <= date2)).ToList();

            var getsumkgs = intakes.Where(i => i.TransactionType == TransactionType.Intake 
            || i.TransactionType == TransactionType.Correction && i.Branch == saccobranch).Sum(n => n.Qsupplied);

            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            if (!string.IsNullOrEmpty(producttype))
                intakes = intakes.Where(i => i.ProductType.ToUpper().Equals(producttype.ToUpper())).ToList();
            decimal? bal = 0;
            var intake = intakes.OrderBy(i => i.TransDate).GroupBy(d => d.TransDate).ToList();
            var MilkEnquryVM = new List<MilkEnqury>();
            intake.ForEach(l =>
            {
                intakes = l.OrderBy(d => d.Id).ToList();
                intakes.ForEach(i => {

                    i.CR = i?.CR ?? 0;
                    i.DR = i?.DR ?? 0;
                    bal += i.CR - i.DR;
                    i.Balance = bal;
                    if (i.Remarks == null)
                        i.Remarks = i.ProductType;
                    MilkEnquryVM.Add(new MilkEnqury
                    {
                        TransDate = i.TransDate,
                        ProductType = i.ProductType,
                        Qsupplied = i?.Qsupplied ?? 0,
                        Ppu = i?.Ppu ?? 0,
                        CR = i?.CR ?? 0,
                        DR = i?.DR ?? 0,
                        Balance = bal,
                        Description = i.Description,
                        Remarks = i.Remarks,
                        Auditdatetime = DateTime.Now,
                        //shares= ViewBag.shares,
                    });
                    _context.SaveChanges();
                });

            });

            await Getshares(supplier.Sno);

            var Transportert = ViewBag.Transportert;
            //if (Transportert != null)
            //{ 
                MilkEnquryVM.Add(new MilkEnqury
                {
                    Transporter = ViewBag.Transportert,
                });
            //}
            var shar = ViewBag.shares;
            if (shar > 0)
                MilkEnquryVM.Add(new MilkEnqury
                {
                    shares = ViewBag.shares,
                });

            var entries = MilkEnquryVM.OrderByDescending(n => n.Auditdatetime).ToList();
            return Json(new
            {
                entries,
                getsumkgs
            });
        }

        [HttpPost]
        public JsonResult SuppliedProductsTransporter(string sno, DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            IQueryable<DTransporter> dTransporters = _context.DTransporters;
            IQueryable<ProductIntake> productIntakes = _context.ProductIntake;
            if (StrValues.Slopes == sacco)
                sno = dTransporters.FirstOrDefault(t => t.CertNo == sno)?.TransCode ?? "";

            var intakes = productIntakes.Where(i => i.Sno.ToUpper().Equals(sno.ToUpper()) && i.SaccoCode.ToUpper()
           .Equals(sacco.ToUpper()) && (i.TransDate >= date1 && i.TransDate <= date2)).ToList().OrderBy(m=>m.TransDate).ToList();
            if (StrValues.Slopes == sacco)
            { 
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }
            else
            {
                intakes = intakes.Where(i => i.Branch == saccobranch).ToList();
            }

            decimal? bal = 0;
            intakes.ForEach(i =>
            {
                i.CR = i?.CR ?? 0;
                i.DR = i?.DR ?? 0;
                bal += i.CR - i.DR;
                i.Balance = bal;
                if (i.Remarks == null)
                    i.Remarks = i.ProductType;
            });

            //intakes = intakes.OrderByDescending(i => i.TransDate).ToList();
            return Json(intakes);
        }

        [HttpPost]
        public JsonResult SuppliedProductsTransporter1(string sno, DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            var resultsget = _context.ProductIntake.OrderByDescending(i => i.TransDate)
                        .Where(i => i.SaccoCode == sacco && i.Branch == saccobranch
                    && i.TransDate >= date1 && i.TransDate <= date2).ToList();
            var transExist = _context.DTransporters.Any(u => u.TransCode == sno && u.Active == true && u.ParentT == sacco);
            if (transExist)
            {
                var transassign = _context.DTransports
                    .Where(u => u.TransCode == sno && u.Branch == saccobranch && u.Active == true && u.saccocode.ToUpper().Equals(sacco.ToUpper()));
                foreach (var snoo in transassign)
                {
                    var sumkgspersupplier = _context.ProductIntake
                        .Where(u => u.SaccoCode == sacco && u.Branch == saccobranch && u.TransDate >= date1 && u.TransDate <= date2 && u.Sno == snoo.Sno.ToString())
                        .Sum(i => i.CR);
                    var all = _context.DTmpTransEnqueries;
                    DTmpTransEnqueryobj.sacco = sacco;
                    DTmpTransEnqueryobj.Sno = snoo.Sno.ToString();
                    DTmpTransEnqueryobj.TransDate = date2;
                    DTmpTransEnqueryobj.Cr = sumkgspersupplier;
                    DTmpTransEnqueryobj.Branch = saccobranch;
                }
                var intakes = _context.DTmpTransEnqueries.OrderByDescending(i => i.TransDate)
                       .Where(i => i.Sno == sno && i.sacco.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch
                   && i.TransDate >= date1 && i.TransDate <= date2).ToList();
                //resultsget = intakes;
            }
            return Json(resultsget);
        }

        public IActionResult SharesInquiry()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            ViewBag.isAinabkoi = sacco == StrValues.Ainabkoi;
            GetInitialValues();
            return View();
        }

        [HttpGet]
        public JsonResult ShareInquiries(string sno)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var shares = _context.DShares.Where(s => s.Sno.ToUpper().Equals(sno.ToUpper()) && s.SaccoCode == sacco && s.Branch == saccobranch).ToList();

            shares = shares.OrderBy(s => s.TransDate).ToList();

            return Json(shares);
        }
    }
}
