using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Models.Coffee;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using EasyPro.ViewModels.CoffeVM;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace EasyPro.Controllers.Coffee
{
    public class MillerProductsDetailsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public MillerProductsDetailsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: MillerProductsDetails
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var weighing = _context.MillerProductsDetails.Where(n => n.Saccocode == sacco);
            //MillingProductsVm newItem = new  MillingProductsVm();
            //newItem.MillerProd = weighing;
            //newItem.Weights.Add(new MillerProductsWeight() { Id = 0 });
            //newItem.MilledProducts.Add(new MillerProducts() { Id = 0 });

            return View(weighing.OrderByDescending(v => v.Date));
        }
        private void getdefaults()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            var partchments = _context.Parchment.OrderBy(n => n.PName).ToList();
            var factories = _context.DBranch.Where(b => b.Bcode == sacco).OrderBy(n => n.Bname).ToList();
            var Millers = _context.Millers.Where(b => b.saccocode == sacco).OrderBy(n => n.Name).ToList();

            
            ViewBag.Category = new SelectList(partchments.OrderBy(n => n.PName).Select(k => k.PName).ToList());
            ViewBag.Factory = new SelectList(factories.OrderBy(n => n.Bname).Select(k => k.Bname).ToList());
            ViewBag.Miller = new SelectList(Millers.OrderBy(n => n.Name).Select(k => k.Name).ToList());

        }

        // GET: MillerProductsDetails/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var millerProductsDetails = await _context.MillerProductsDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (millerProductsDetails == null)
            {
                return NotFound();
            }

            return View(millerProductsDetails);
        }

        // GET: MillerProductsDetails/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            getdefaults();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            MillerProductsDetails weights = new MillerProductsDetails();
            weights.DeliveryDate = DateTime.Now;
            weights.MillingDate = DateTime.Now;
            weights.Weights.Add(new MillerProductsWeight() { Id = 0 });
            weights.MilledProducts.Add(new MillerProducts() { Id = 0 });
            //weights.MillerProd = new MillerProductsDetails();
            //weights.MillerProd.DeliveryDate = DateTime.Now;
            //weights.MillerProd.MillingDate = DateTime.Now ;
            //weights.Weights = new List<MillerProductsWeight>();
            //weights.MilledProducts = new List<MillerProducts>();
            //weights.MillerProd.DeliveryDate = DateTime.Now;
            //weights.MillerProd.MillingDate = DateTime.Now;
            return View(weights);
        }

        // POST: MillerProductsDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( MillerProductsDetails millerProductsDetails, List<MillerProductsWeight> Weights, List<MillerProducts> MilledProducts)
        {
            try {
                utilities.SetUpPrivileges(this);
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                
                    millerProductsDetails.Saccocode = sacco;
                    millerProductsDetails.AuditDateTime = DateTime.Now;
                    millerProductsDetails.Date = DateTime.Now;
                    _context.Add(millerProductsDetails);

                Weights.ForEach(t =>
                    {
                        t.WeightNoteNo = t.WeightNoteNo;
                        t.Bags = t.Bags;
                        t.GrossWeight = t.GrossWeight;

                        //var product = _context.MillerProductsWeight.FirstOrDefault(p => p.WeightNoteNo.Equals(t.WeightNoteNo)
                        //&& p.Bags == t.Bags && p.GrossWeight == t.GrossWeight);
                        //if (product == null)
                        //{

                            _context.MillerProductsWeight.Add(new MillerProductsWeight
                            {
                                WeightNoteNo = t.WeightNoteNo,
                                Bags = t.Bags,
                                GrossWeight = t.GrossWeight,

                            });

                        //}

                    });
                    MilledProducts.ForEach(t =>
                    {
                        t.Grade = t.Grade;
                        t.Bags = t.Bags;
                        t.Pkts = t.Pkts;
                        t.PercentageTotal = t.PercentageTotal;
                        t.MillClass = t.MillClass;
                        t.BulkNo = t.BulkNo;

                        //var product = _context.MillerProducts.FirstOrDefault(p => p.Grade.Equals(t.Grade)
                        //&& p.Bags == t.Bags && p.MillClass == t.MillClass);
                        //if (product == null)
                        //{

                            _context.MillerProducts.Add(new MillerProducts
                            {
                                Grade = t.Grade,
                                Bags = t.Bags,
                                Pkts = t.Pkts,
                                PercentageTotal = t.PercentageTotal,
                                MillClass = t.MillClass,
                                BulkNo = t.BulkNo,

                            });

                        //}

                    });

                   
                    await _context.SaveChangesAsync();
                    
                    //_context.Add(millerProductsDetails);
                    //await _context.SaveChangesAsync();
                    //var newItem = new MillerProducts()
                    //{
                    //    Name = scan.Name,
                    //    Upc = scan.UPC,
                    //    DateCreated = DateTime.Now,
                    //};
                    //_context.Add(millerProductsDetails);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
            } catch (Exception ex) 
            {
                return NotFound();
            }
           

           
            }

        // GET: MillerProductsDetails/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var millerProductsDetails = await _context.MillerProductsDetails.FindAsync(id);
            if (millerProductsDetails == null)
            {
                return NotFound();
            }
            return View(millerProductsDetails);
        }

        // POST: MillerProductsDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Factory,Saccocode,CbkCode,OutNumber,Category,Season,MillerID,Certification,DeliveryDate,MillingDate,MillingLoss,MoistureParch,MostureClean,MillingCHarges,ExportsCost,VatCost,Date,AuditDateTime")] MillerProductsDetails millerProductsDetails)
        {
            if (id != millerProductsDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(millerProductsDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MillerProductsDetailsExists(millerProductsDetails.Id))
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
            return View(millerProductsDetails);
        }

        // GET: MillerProductsDetails/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var millerProductsDetails = await _context.MillerProductsDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (millerProductsDetails == null)
            {
                return NotFound();
            }

            return View(millerProductsDetails);
        }

        // POST: MillerProductsDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var millerProductsDetails = await _context.MillerProductsDetails.FindAsync(id);
            _context.MillerProductsDetails.Remove(millerProductsDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MillerProductsDetailsExists(long id)
        {
            return _context.MillerProductsDetails.Any(e => e.Id == id);
        }
    }
}
