using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using EasyPro.ViewModels;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.Bibliography;

namespace EasyPro.Controllers
{
    public class CProductsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public CProductsController(MORINGAContext context, INotyfService notyf)
        {
            utilities = new Utilities(context);
            _context = context;
            _notyf = notyf;
        }

        // GET: CProducts
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.CProducts.Where(p => p.SaccoCode == sacco).ToListAsync());
        }

        // GET: CProducts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var cProduct = await _context.CProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cProduct == null)
            {
                return NotFound();
            }

            return View(cProduct);
        }

        // GET: CProducts/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var glsetups = _context.Glsetups.Where(c => c.saccocode == sacco).ToList();
            ViewBag.glsetups = new SelectList(glsetups, "AccNo", "GlAccName");
            var types = new string[] { "Consumable", "Service" };
            ViewBag.types = new SelectList(types);
            var taxes = _context.Taxes.Where(c => c.SaccoCode == sacco && c.Type == "Sales").ToList();
            ViewBag.taxes = new SelectList(taxes, "Name", "Name");
        }

        // POST: CProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Category,Ref,BarCode,Price,CustomerTax,Cost,Notes,ARGlAccount,ContraAccount,Closed")] CProduct cProduct)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            cProduct.Price = cProduct?.Price ?? 0;
            if (string.IsNullOrEmpty(cProduct.Name))
            {
                _notyf.Error("Sorry, Kindly provide product name");
                return View(cProduct);
            }
            if (cProduct.Price < 1)
            {
                _notyf.Error("Sorry, Kindly provide Price");
                return View(cProduct);
            }
            if (string.IsNullOrEmpty(cProduct.ARGlAccount))
            {
                _notyf.Error("Sorry, Kindly provide Gl Account");
                return View(cProduct);
            }
            if (string.IsNullOrEmpty(cProduct.ContraAccount))
            {
                _notyf.Error("Sorry, Kindly provide Contra Account");
                return View(cProduct);
            }
            if (_context.CProducts.Any(g => g.SaccoCode == sacco && g.Name.ToUpper().Equals(cProduct.Name.ToUpper())))
            {
                _notyf.Error("Sorry, Product already exist");
                return View(cProduct);
            }
            if (ModelState.IsValid)
            {
                cProduct.Id = Guid.NewGuid();
                cProduct.SaccoCode = sacco;
                _context.Add(cProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cProduct);
        }

        // GET: CProducts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var cProduct = await _context.CProducts.FindAsync(id);
            if (cProduct == null)
            {
                return NotFound();
            }
            return View(cProduct);
        }

        // POST: CProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Type,Category,Ref,BarCode,Price,CustomerTax,Cost,Notes,ARGlAccount,ContraAccount,Closed")] CProduct cProduct)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            SetInitialValues();
            if (id != cProduct.Id)
            {
                return NotFound();
            }
            cProduct.Price = cProduct?.Price ?? 0;
            if (string.IsNullOrEmpty(cProduct.Name))
            {
                _notyf.Error("Sorry, Kindly provide product name");
                return View(cProduct);
            }
            if (cProduct.Price < 1)
            {
                _notyf.Error("Sorry, Kindly provide Price");
                return View(cProduct);
            }
            if (string.IsNullOrEmpty(cProduct.ARGlAccount))
            {
                _notyf.Error("Sorry, Kindly provide Gl Account");
                return View(cProduct);
            }
            if (string.IsNullOrEmpty(cProduct.ContraAccount))
            {
                _notyf.Error("Sorry, Kindly provide Contra Account");
                return View(cProduct);
            }
            if (_context.CProducts.Any(g => g.SaccoCode == sacco && g.Name.ToUpper().Equals(cProduct.Name.ToUpper()) && g.Id != cProduct.Id))
            {
                _notyf.Error("Sorry, Product already exist");
                return View(cProduct);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CProductExists(cProduct.Id))
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
            return View(cProduct);
        }

        // GET: CProducts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var cProduct = await _context.CProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cProduct == null)
            {
                return NotFound();
            }

            return View(cProduct);
        }

        // POST: CProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var cProduct = await _context.CProducts.FindAsync(id);
            _context.CProducts.Remove(cProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CProductExists(Guid id)
        {
            return _context.CProducts.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ReceivedItems()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.ReceivedItems.Where(p => p.Saccocode == sacco).ToListAsync());
        }

        public IActionResult AddItem()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetItemInitialValues();
            return View();
        }

        // POST: CProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("Product,Quantity")] ReceivedItem item)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetItemInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            if (string.IsNullOrEmpty(item.Product))
            {
                _notyf.Error("Sorry, Kindly provide product");
                return View(item);
            }
            if (item.Quantity < 0.01M)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(item);
            }

            var inventory = _context.Inventory.FirstOrDefault(i => i.Product == item.Product && i.Saccocode == sacco && i.Branch == saccoBranch);
            if(inventory == null)
                _context.Inventory.Add(new Inventory
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    Saccocode = sacco,
                    Branch = saccoBranch,
                    AuditId = loggedInUser,
                    TransDate = DateTime.Today,
                    AuditDate = DateTime.Now
                });
            else
            {
                inventory.Quantity += item.Quantity;
                inventory.AuditId = loggedInUser;
                inventory.AuditDate = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                item.Saccocode = sacco;
                item.Branch = saccoBranch;
                item.AuditId = loggedInUser;
                item.TransDate = DateTime.Today;
                item.AuditDate = DateTime.Now;
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ReceivedItems));
            }
            return View(item);
        }

        private void SetItemInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var products = _context.CProducts.Where(c => c.SaccoCode == sacco)
                .Select(C => C.Name).ToList();
            ViewBag.products = new SelectList(products);
            var types = new string[] { "Cash", "Mpesa" };
            ViewBag.types = new SelectList(types);
        }

        public async Task<IActionResult> SoldItems()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.SoldItems.Where(p => p.Saccocode == sacco).ToListAsync());
        }

        public IActionResult SellItem()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetItemInitialValues();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellItem([Bind("Product,Quantity,PaymentMode,TransDate")] SoldItem item)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetItemInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            if (string.IsNullOrEmpty(item.Product))
            {
                _notyf.Error("Sorry, Kindly provide product");
                return View(item);
            }
            if (item.Quantity < 0.01M)
            {
                _notyf.Error("Sorry, Kindly provide quantity");
                return View(item);
            }

            var product = _context.CProducts.FirstOrDefault(p => p.Name == item.Product && p.SaccoCode == sacco);
            if (product == null)
            {
                _notyf.Error("Sorry, Product does not exist");
                return View(item);
            }
            var inventory = _context.Inventory.FirstOrDefault(i => i.Product == item.Product && i.Saccocode == sacco && i.Branch == saccoBranch);
            if (inventory == null)
                inventory = new Inventory();
            inventory.Quantity = inventory?.Quantity ?? 0;
            inventory.Quantity -= item.Quantity;
            if (inventory.Quantity < 0)
            {
                _notyf.Error("Sorry, Kindly check on your stock");
                return View(item);
            }

            decimal? amount = item.Quantity * product.Price;
            _context.Gltransactions.Add(new Gltransaction
            {
                AuditId = loggedInUser,
                TransDate = item.TransDate.GetValueOrDefault(),
                AuditTime = DateTime.Now,
                Source = "",
                TransDescript = "Yoghurt",
                Transactionno = $"{loggedInUser}{DateTime.Now}",
                SaccoCode = sacco,
                Branch = saccoBranch,
                Amount = (decimal)amount,
                DrAccNo = product.ContraAccount,
                CrAccNo = product.ARGlAccount,
            });

            if (ModelState.IsValid)
            {
                item.Saccocode = sacco;
                item.Branch = saccoBranch;
                item.AuditId = loggedInUser;
                item.AuditDate = DateTime.Now;
                item.Amount = amount;
                item.UnitPrice = product.Price;
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SoldItems));
            }
            return View(item);
        }
    }
}
