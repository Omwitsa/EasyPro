using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            SetIntakeInitialValues();
            IQueryable<AgReceipt> agReceipts = _context.AgReceipts;
            var SalesAnalysis = agReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())
            && i.TDate >= startDate && i.TDate <= enDate).ToList();
            var SalesAnaly = SalesAnalysis.OrderByDescending(s => s.RId).ToList();
            return View(SalesAnaly);
        }
        private void SetIntakeInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            IQueryable<AgProduct> agProducts = _context.AgProducts;
            var products = agProducts.Where(s => s.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                products = products.Where(s => s.Branch == saccoBranch).ToList();
            ViewBag.products = new SelectList(products, "PCode", "PName");
        }

        [HttpPost]
        public JsonResult SuppliedProducts(DateTime date1, DateTime date2, string product)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);

            var startingdate = date2;
            var startDate = new DateTime(date1.Year, date1.Month, 1);
            var endDate = startDate.AddDays(-1);

            var products = new List<AgProductVM>();
            //IQueryable<AgReceipt> agReceipts = _context.AgReceipts;
            //IQueryable<AgProducts4> agProducts4s = _context.AgProducts4s;
            //IQueryable<Drawnstock> drawnstocks = _context.Drawnstocks;

            var agReceipts = _context.AgReceipts.Where(n => n.saccocode == sacco).ToList();
            var agProducts4s = _context.AgProducts4s.Where(n => n.saccocode == sacco).ToList();
            var drawnstocks = _context.Drawnstocks.Where(n => n.saccocode == sacco).ToList();

            //var agProductsReceive = _context.AgReceipts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate >= date1 && i.TDate <= date2).ToList();
            var agProductsReceive = _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                agProductsReceive = agProductsReceive.Where(i => i.Branch == saccobranch).ToList();

            agProductsReceive = agProductsReceive.OrderBy(i => i.PName).ToList();

            if (!string.IsNullOrEmpty(product))
            {
                agProductsReceive = agProductsReceive.Where(k => k.Branch == saccobranch).ToList();
            }

            var Branchlist = agProductsReceive.GroupBy(m => m.Branch).ToList();


            Branchlist.ForEach(b =>
            {
               var getIndividualagReceipts = agReceipts.Where(b=>b.Branch == b.Branch).ToList();
               var getIndividualagProducts4s = agProducts4s.Where(b => b.Branch == b.Branch).ToList();
               var getIndividualdrawnstocks = drawnstocks.ToList();

                var listPerBranch = agProductsReceive.Where(z => z.Branch == b.Key).ToList();
                if (!string.IsNullOrEmpty(product))
                {
                    listPerBranch = listPerBranch.Where(k => k.PCode.ToUpper().Equals(product.ToUpper())).ToList();
                    startingdate = date1;
                }
                var listofproducts = listPerBranch.GroupBy(m => m.PCode).ToList();
                listofproducts.ForEach(e =>
                {
                    var productNow = e.FirstOrDefault();

                    while (startingdate <= date2)
                    {
                        if(productNow.PCode== "274")
                        {

                        }
                        var detailstore = GetReceipts(productNow.PCode, sacco, b.Key, date1, startingdate, endDate, getIndividualagReceipts, getIndividualagProducts4s, getIndividualdrawnstocks);

                        decimal open = (decimal)((detailstore.pro_buyy) - (detailstore.positive_pro_sell - detailstore.negatives_pro_sell));
                        open = open + (decimal)((detailstore.dispatchlasttoBranch - detailstore.dispatchlastfromBranch));
                        decimal dispatch = (decimal)(detailstore.dispatchthismonthtoBranch - detailstore.dispatchthismonthfromBranch);
                        decimal correctbal = (decimal)detailstore.receiptthatmonth + open + dispatch;
                        decimal saleskgs = (decimal)(detailstore.positive_agProductsales - detailstore.negative_agProductsales);
                        decimal bal = (correctbal - saleskgs);
                        decimal? BPrice = productNow.Pprice ?? 0;
                        decimal? SPrice = productNow.Sprice ?? 0;

                        if (productNow.Pprice < 0)
                            BPrice = productNow.Pprice * -1;

                        if (productNow.Sprice < 0)
                            SPrice = productNow.Sprice * -1;

                        products.Add(new AgProductVM
                        {
                            Code = productNow.PCode,
                            Name = productNow.PName,
                            Openning = open,
                            AddedStock = (decimal)detailstore.receiptthatmonth,
                            Dispatch = dispatch,
                            StoreBal = correctbal,
                            Sales = (decimal)saleskgs,
                            Bal = bal,
                            BPrice = (decimal)BPrice,
                            SPrice = (decimal)SPrice,
                            Branch = productNow.Branch,
                            Date = DateTime.Today
                        });

                        startingdate = startingdate.AddDays(1);
                    }
                    startingdate = date2;
                    if (!string.IsNullOrEmpty(product))
                    {
                        startingdate = date1;
                    }
                });
            });
            return Json(products);
        }


        private dynamic GetReceipts(string pCode, string sacco, string key, DateTime date1, DateTime date2, DateTime endDate, List<AgReceipt> getIndividualagReceipts, List<AgProducts4> getIndividualagProducts4s, List<Drawnstock> getIndividualdrawnstocks)
        {
            
            var saccobranch = key;


            var pro_buyy = getIndividualagProducts4s.Where(i => i.DateEntered <= endDate && i.Branch == key
            && i.PCode.ToUpper().Equals(pCode.ToUpper())).Sum(g => g.Qin);

            var positive_pro_sell = getIndividualagReceipts.Where(i => i.TDate <= endDate && i.Amount >= 0 
            && i.PCode.ToUpper().Equals(pCode.ToUpper()) && i.Branch == key).Sum(d => d.Qua);

            var dispatchlastfromBranch = getIndividualdrawnstocks.Where(i => i.BranchF == key &&
             i.Date <= endDate && i.Productid.ToUpper().Equals(pCode.ToUpper())).Sum(d => d.Quantity);

            var dispatchlasttoBranch = getIndividualdrawnstocks.Where(i =>i.Date <= endDate && i.Branch == key
            && i.Productid.ToUpper().Equals(pCode.ToUpper())).Sum(d => d.Quantity);

            var negatives_pro_sell = getIndividualagReceipts.Where(i => i.TDate <= endDate && i.Amount < 0 
            && i.PCode.ToUpper().Equals(pCode.ToUpper()) && i.Branch == key).Sum(d => d.Qua);

            var receiptthatmonth = getIndividualagProducts4s.Where(i => i.DateEntered >= date1  
            && i.DateEntered <= date2 && i.Branch == key
            && i.PCode.ToUpper().Equals(pCode.ToUpper())).Sum(g => g.Qin);

            var positive_agProductsales = getIndividualagReceipts.Where(i => i.TDate >= date1
            && i.TDate <= date2 && i.Amount >= 0 && i.Branch == key && i.PCode.ToUpper().Equals(pCode.ToUpper())).Sum(n => n.Qua);

            var negative_agProductsales = getIndividualagReceipts.Where(i => i.TDate >= date1
            && i.TDate <= date2 && i.Amount < 0 && i.Branch == key && i.PCode.ToUpper().Equals(pCode.ToUpper())).Sum(n => n.Qua);

            var dispatchthismonthfromBranch = getIndividualdrawnstocks.Where(i => i.BranchF == key
             && i.Date >= date1 && i.Date <= date2 && i.Productid.ToUpper().Equals(pCode.ToUpper()))
                .Sum(d => d.Quantity);

            var dispatchthismonthtoBranch = getIndividualdrawnstocks.Where(i => i.Branch == key
             && i.Date >= date1 && i.Date <= date2 && i.Productid.ToUpper().Equals(pCode.ToUpper()))
                .Sum(d => d.Quantity);

            return new
            {
                pro_buyy,
                positive_pro_sell,
                negatives_pro_sell,
                receiptthatmonth,
                positive_agProductsales,
                negative_agProductsales,
                dispatchlastfromBranch,
                dispatchlasttoBranch,
                dispatchthismonthfromBranch,
                dispatchthismonthtoBranch
            };
        }
    }
}
