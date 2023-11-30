using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Wordprocessing;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class AgrovetCheckOReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AgrovetCheckOReportController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public async Task<IActionResult> Index()
        {
            DateTime Now = DateTime.Today;
            DateTime startDate = new DateTime(Now.Year, Now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            await SetIntakeInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var Salescheckoff = await _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.TDate >= startDate && i.TDate <= enDate && i.SNo.ToUpper() != "CASH" && i.SNo.ToUpper() != "STAFF").ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                Salescheckoff = Salescheckoff.Where(s => s.Branch == saccobranch).ToList();

            Salescheckoff = Salescheckoff.OrderByDescending(s => s.TDate).ToList();
            return View(Salescheckoff);
        }

        private async Task SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var products = await _context.AgProducts
                .Where(s => s.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                products = products.Where(s => s.Branch == saccoBranch).ToList();
            ViewBag.products = new SelectList(products, "PCode", "PName");
        }

        [HttpPost]
        public async Task<JsonResult> CheckOffReport(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
        {
            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno, salestype);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper() != "CASH" && i.SNo.ToUpper() != "STAFF").OrderByDescending(i => i.TDate).ToList();
            return Json(Salescheckoff);
        }

        [HttpPost]
        public async Task<JsonResult> CheckOffSummary(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            
            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno,salestype);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper() != "CASH" && i.SNo.ToUpper() != "STAFF").OrderByDescending(i => i.TDate).ToList();
            var groupedReports = Salescheckoff.GroupBy(i => i.TDate).ToList();
            var productName = "";
            if (!string.IsNullOrEmpty(product))
            {
                var products = await _context.AgProducts.Where(p => p.saccocode == sacco && p.PCode == product).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    products = products.Where(i => i.Branch == saccobranch).ToList();
                productName = products.FirstOrDefault()?.PName ?? "";
                sno = products.FirstOrDefault()?.SNo ?? "";
            }

            var summary = new List<dynamic>();
            groupedReports.ForEach(i =>
            {
                var correctquantity = i.Where(b => b.Amount > 0).Sum(r => r.Qua) - i.Where(b => b.Amount < 0).Sum(r => r.Qua);
                summary.Add(new
                {
                    date = i.Key,
                    product,
                    productName,
                    sno,
                    quantity = correctquantity,
                    amount = i.Sum(r => r.Amount)
                });
            });
          
            return Json(summary);
        }

        [HttpPost]
        public async Task<JsonResult> checkOffSummarydDeductions(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var supplier = await _context.DSuppliers.Where(n => n.Scode == sacco).ToListAsync();

            var Salescheckoff = await GetAgReceipts(startDate, endDate, product, sno, salestype);
            Salescheckoff = Salescheckoff.Where(i => i.SNo.ToUpper() != "CASH" && i.SNo.ToUpper() != "STAFF").OrderByDescending(i => i.TDate).ToList();
            var groupedReports = Salescheckoff.GroupBy(i => i.SNo).ToList();

            IQueryable<DTransporter> dTransporters = _context.DTransporters;
            var summary = new List<dynamic>();
            groupedReports.ForEach(i =>
            {
                var getfirst = i.FirstOrDefault().SNo;
                SuppliersVM suppliernames = new SuppliersVM();
                var suppliername = supplier.FirstOrDefault(c=>  c.Sno.ToUpper().Trim() == getfirst.ToUpper().Trim());
                if(suppliername != null)
                {
                    suppliernames.Name = suppliername.Names;
                    suppliernames.SNo = suppliername.Sno;
                }
                var transportersname = dTransporters.FirstOrDefault(v => v.ParentT == sacco &&  v.TransCode.ToUpper().Trim() == getfirst.ToUpper().Trim());
                if(transportersname != null)
                {
                    suppliernames.SNo = transportersname.TransCode;
                    suppliernames.Name = transportersname.TransName;
                }

                if(suppliernames.SNo == null)
                {

                }
                var salesamount = i.Where(b => b.PCode != "0").Sum(r => r.Amount);
                var partialpaymentincase = (i.Where(b => b.PCode == "0").Sum(r => r.Amount))*-1;
                summary.Add(new
                {
                    sno = suppliernames.SNo,
                    name = suppliernames.Name,
                    sales = salesamount,
                    partialpayment =  partialpaymentincase,
                    balance =  salesamount - partialpaymentincase,
                });
            });

            return Json(summary);
        }

        private async Task<List<AgReceipt>> GetAgReceipts(DateTime startDate, DateTime endDate, string product, string sno, int salestype)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var receipts = await _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
               && i.TDate >= startDate && i.TDate <= endDate).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
            receipts = receipts.Where(i => i.Branch == saccobranch).ToList();

            if(!string.IsNullOrEmpty(product))
                receipts = receipts.Where(r => r.PCode == product).ToList();
            if (!string.IsNullOrEmpty(sno))
                receipts = receipts.Where(r => r.SNo == sno).ToList();
            if (salestype == 1)
                receipts = receipts.Where(m => m.Amount > 0).ToList();
            if (salestype == 2)
                receipts = receipts.Where(m => m.Amount < 0).ToList();

            return receipts;
        }
    }
}
