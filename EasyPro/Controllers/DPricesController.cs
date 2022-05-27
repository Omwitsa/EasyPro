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
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }
            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Products == id);
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
        }
        // POST: DPrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Edate,Price,Products")] DPrice dPrice)
        {
            utilities.SetUpPrivileges(this);
            var dpricer = _context.DPrices.Where(i => i.Products == dPrice.Products).Count();
            if (dpricer != 0)
            {
                _notyf.Error("Sorry, The product already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                _context.Add(dPrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dPrice);
        }
        // GET: DPrices/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices.FindAsync(id);
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
        public async Task<IActionResult> Edit(string id, [Bind("Edate,Price,Products")] DPrice dPrice)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id != dPrice.Products)
            {
                return NotFound();
            }
            var dpricer = _context.DPrices.Where(i => i.Products == dPrice.Products).Count();
            if (dpricer != 0)
            {
                _notyf.Error("Sorry, The product already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
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
        public async Task<IActionResult> Delete(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dPrice = await _context.DPrices
                .FirstOrDefaultAsync(m => m.Products == id);
            if (dPrice == null)
            {
                return NotFound();
            }

            return View(dPrice);
        }

        // POST: DPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string product)
        {
            utilities.SetUpPrivileges(this);
            var dPrice = await _context.DPrices.FindAsync(product);
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
