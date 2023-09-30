using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class AgrovetStockBalReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AgrovetStockBalReportController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
        public IActionResult Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            DateTime Now = DateTime.Today;
            DateTime startDate = new DateTime(Now.Year, Now.Month, 1);
            DateTime enDate = startDate.AddMonths(1).AddDays(-1);

            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);

            ViewBag.Sacco = sacco;
            ViewBag.User = loggedInUser;
            var SalesAnalysis = _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.TDate >= startDate && i.TDate <= enDate)
            .OrderByDescending(s => s.RId).ToList();

            return View(SalesAnalysis);
        }

        [HttpPost]
        public JsonResult SuppliedProducts(DateTime date1, DateTime date2)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var startDate = new DateTime(date1.Year, date1.Month, 1);
            var endDate = startDate.AddDays(-1);

            var products = new List<AgProductVM>();

            //var agProductsReceive = _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate >= date1 && i.TDate <= date2).ToList();
            var agProductsReceive = _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                agProductsReceive = agProductsReceive.Where(i => i.Branch == saccobranch).ToList();

            agProductsReceive = agProductsReceive.OrderByDescending(i => i.PCode).ToList();

            var Branchlist = agProductsReceive.GroupBy(m => m.Branch).ToList();
            IQueryable<AgReceipt> agReceipts = _context.AgReceipts;
            Branchlist.ForEach(b =>
            {
                var listPerBranch = agProductsReceive.Where(z => z.Branch == b.Key).ToList();
                var listofproducts = listPerBranch.GroupBy(m => m.PCode).ToList();
                listofproducts.ForEach(e =>
            {
                var productNow = e.FirstOrDefault();
                var pro_buyy = _context.AgProducts4s.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch
                && i.DateEntered <= endDate && i.PCode.ToUpper().Equals(productNow.PCode.ToUpper())).Sum(g => g.Qin);

                var positive_pro_sell = agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch &&
                 i.TDate <= endDate && i.Amount >= 0 && i.PCode.ToUpper().Equals(productNow.PCode.ToUpper())).Sum(d => d.Qua);

                var negatives_pro_sell = agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch &&
                 i.TDate <= endDate && i.Amount < 0 && i.PCode.ToUpper().Equals(productNow.PCode.ToUpper())).Sum(d => d.Qua);

                decimal open = (decimal)((pro_buyy) - (positive_pro_sell - negatives_pro_sell));

                var receiptthatmonth = _context.AgProducts4s.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch && i.DateEntered >= date1
               && i.DateEntered <= date2 && i.PCode.ToUpper().Equals(productNow.PCode.ToUpper())).Sum(g => g.Qin);

                var positive_agProductsales = agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate >= date1
                && i.TDate <= date2 && i.Amount >= 0 && i.Branch == saccobranch && i.PCode.ToUpper().Equals(productNow.PCode.ToUpper())).Sum(n => n.Qua);

                var negative_agProductsales = agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate >= date1
                && i.TDate <= date2 && i.Amount < 0 && i.Branch == saccobranch && i.PCode.ToUpper().Equals(productNow.PCode.ToUpper())).Sum(n => n.Qua);

                decimal correctbal = (decimal)receiptthatmonth + open;
                decimal saleskgs = (decimal)(positive_agProductsales - negative_agProductsales);
                decimal bal = (correctbal - saleskgs);
                decimal BPrice = (decimal)productNow.Pprice;
                decimal SPrice = (decimal)productNow.Sprice;

                if ((decimal)productNow.Pprice < 0)
                    BPrice = (decimal)productNow.Pprice * -1;

                if ((decimal)productNow.Sprice < 0)
                    SPrice = (decimal)productNow.Sprice * -1;

                products.Add(new AgProductVM
                {
                    Code = productNow.PCode,
                    Name = productNow.PName,
                    Openning = open,
                    AddedStock = (decimal)receiptthatmonth,
                    StoreBal = correctbal,
                    Sales = (decimal)saleskgs,
                    Bal = bal,
                    BPrice = BPrice,
                    SPrice = SPrice,
                    Branch = productNow.Branch,
                    Date = DateTime.Today
                });
            });
            });
            return Json(products);
        }
    }
}
