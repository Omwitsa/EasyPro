using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;

namespace EasyPro.Controllers
{
    public class DPrice2Controller : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;
        public DPrice2Controller(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }


        // GET: DPrice2
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.d_Price2
                .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DPrice2/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            if (id == null)
            {
                return NotFound();
            }

            var dPrice2 = await _context.d_Price2
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPrice2 == null)
            {
                return NotFound();
            }

            return View(dPrice2);
        }

        // GET: DPrice2/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var products = _context.DBranchProducts.Where(a => a.saccocode == sacco).Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
            var glAccounts = _context.Glsetups.Where(g => g.saccocode == sacco).ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");
        }
        // POST: DPrice2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,Name,Price,Date,UserId,Branch,SaccoCode,Product,Active")] DPrice2 dPrice2)
        {
            
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var checkname = _context.DSuppliers.FirstOrDefault(h => h.Scode.ToUpper().Equals(sacco.ToUpper()) && h.Branch == saccoBranch && h.Sno.ToUpper().Equals(dPrice2.Sno.ToUpper()));
            if (checkname == null)
            {
                _notyf.Error("Sorry, Supplier code Does not Exist");
                return View();
            }
            var checksno = _context.d_Price2.Where(n => n.Sno.ToUpper().Equals(dPrice2.Sno.ToUpper()) 
            && n.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && n.Branch== saccoBranch && n.Product.ToUpper().Equals(dPrice2.Product.ToUpper())).ToList();
            if (checksno.Count >0)
            {
                _notyf.Error("Sorry, Supplier code Already Exist");
                return View();
            }
            if (ModelState.IsValid)
            {
               
                dPrice2.SaccoCode = sacco;
                dPrice2.Branch = saccoBranch;
                dPrice2.UserId = loggedInUser;
                dPrice2.Active = true;
                dPrice2.Name = checkname.Names;
                _context.Add(dPrice2);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dPrice2);
        }

        // GET: DPrice2/Edit/5
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

            var dPrice2 = await _context.d_Price2.FindAsync(id);
            if (dPrice2 == null)
            {
                return NotFound();
            }
            
            GetInitialValues();
            return View(dPrice2);
        }

        // POST: DPrice2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,Name,Price,Date,UserId,Branch,SaccoCode,Product,Active")] DPrice2 dPrice2)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != dPrice2.Id)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(dPrice2.Product))
            {
                _notyf.Error("Sorry, Kindly provide product");
                return View();
            }
            if (dPrice2.Price < 0)
            {
                _notyf.Error("Sorry, Kindly provide price");
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                    var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
                    var productIntakes = _context.ProductIntake
                       .Where(p => p.TransDate >= dPrice2.Date && p.Sno.ToUpper().Equals(dPrice2.Sno.ToUpper()) && (p.TransactionType == TransactionType.Intake || p.TransactionType == TransactionType.Correction)
                       && p.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToList();

                    productIntakes.ForEach(p =>
                    {
                        p.Ppu = dPrice2.Price;
                        if (p.CR != 0)
                            p.CR = dPrice2.Price * p.Qsupplied;
                        if (p.DR != 0)
                            p.DR = dPrice2.Price * p.Qsupplied * -1;
                    });

                    dPrice2.SaccoCode = sacco;
                    dPrice2.Branch = saccoBranch;
                    dPrice2.UserId = loggedInUser;

                    _context.Update(dPrice2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPrice2Exists(dPrice2.Id))
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
            return View(dPrice2);
        }

        // GET: DPrice2/Delete/5
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

            var dPrice2 = await _context.d_Price2
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPrice2 == null)
            {
                return NotFound();
            }

            return View(dPrice2);
        }

        // POST: DPrice2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dPrice2 = await _context.d_Price2.FindAsync(id);
            _context.d_Price2.Remove(dPrice2);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPrice2Exists(long id)
        {
            return _context.d_Price2.Any(e => e.Id == id);
        }
    }
}
