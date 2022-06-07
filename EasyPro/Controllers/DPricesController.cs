using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class DPricesController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public DPricesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
       
        // GET: DPrices
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.DPrices.ToListAsync());
        }
       
        // GET: DPrices/Details/5
        public async Task<IActionResult> Details(long Id)
        {
            utilities.SetUpPrivileges(this);
            if (Id < 1)
            {
                return NotFound();
            }
            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // GET: DPrices/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var products = _context.DBranchProducts.Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
            var glAccounts = _context.Glsetups.ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");
        }
        
        // POST: DPrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Edate,Price,Products,SubsidyQty,SubsidyPrice,DrAccNo,CrAccNo,TransportDrAccNo,TransportCrAccNo")] DPrice dPrice)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (string.IsNullOrEmpty(dPrice.Products))
            {
                _notyf.Error("Sorry, Kindly provide product");
                return View();
            }
            if(dPrice.Price < 1)
            {
                _notyf.Error("Sorry, Kindly provide price");
                return View();
            }
            if (string.IsNullOrEmpty(dPrice.DrAccNo))
            {
                _notyf.Error("Sorry, Kindly provide product Dr Acc");
                return View();
            }
            if (string.IsNullOrEmpty(dPrice.CrAccNo))
            {
                _notyf.Error("Sorry, Kindly provide product Cr Acc");
                return View();
            }
            if (_context.DPrices.Any(i => i.Products == dPrice.Products))
            {
                _notyf.Error("Sorry, The product already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                dPrice.SaccoCode = sacco;
                _context.Add(dPrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dPrice);
        }
        // GET: DPrices/Edit/5
        public async Task<IActionResult> Edit(long Id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (Id < 1)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
              .FirstOrDefaultAsync(m => m.Id == Id);
            if (dPrice == null)
            {
                return NotFound();
            }
            return View(dPrice);
        }

        // POST: DPrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long Id, [Bind("Id,Edate,Price,Products,SubsidyQty,SubsidyPrice,DrAccNo,CrAccNo,TransportDrAccNo,TransportCrAccNo")] DPrice dPrice)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (Id != dPrice.Id)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(dPrice.Products))
            {
                _notyf.Error("Sorry, Kindly provide product");
                return View();
            }
            if (dPrice.Price < 1)
            {
                _notyf.Error("Sorry, Kindly provide price");
                return View();
            }
            if (string.IsNullOrEmpty(dPrice.DrAccNo))
            {
                _notyf.Error("Sorry, Kindly provide product Dr Acc");
                return View();
            }
            if (string.IsNullOrEmpty(dPrice.CrAccNo))
            {
                _notyf.Error("Sorry, Kindly provide product Cr Acc");
                return View();
            }
            if (_context.DPrices.Any(i => i.Products == dPrice.Products && i.Id != dPrice.Id))
            {
                _notyf.Error("Sorry, The product already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    dPrice.SaccoCode = sacco;
                    _context.Update(dPrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPriceExists(dPrice.Products))
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
            return View(dPrice);
        }

        // GET: DPrices/Delete/5
        public async Task<IActionResult> Delete(long Id)
        {
            utilities.SetUpPrivileges(this);
            if (Id < 1)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // POST: DPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dPrice = await _context.DPrices.FindAsync(id);
            _context.DPrices.Remove(dPrice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPriceExists(string product)
        {
            return _context.DPrices.Any(e => e.Products == product);
        }
    }
}
