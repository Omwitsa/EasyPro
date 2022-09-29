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
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class AgProductsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public AgProductsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: AgProducts
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            GetInitialValues();
            return View(await _context.AgProducts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).ToListAsync());
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var suppliers = _context.AgSupplier1s.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.CompanyName).ToList();
            ViewBag.suppliers = new SelectList(suppliers);

            var products = _context.DBranchProducts.Where(a => a.saccocode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
            var glAccounts = _context.Glsetups.Where(a => a.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

           

        }
        
        // GET: AgProducts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var agProduct = await _context.AgProducts
                .FirstOrDefaultAsync(m => m.PCode == id);
            if (agProduct == null)
            {
                return NotFound();
            }

            return View(agProduct);
        }

        // GET: AgProducts/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var product = _context.AgProducts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderByDescending(u => u.Id).FirstOrDefault();
            var num = 0;
            if (product != null)
                num = Convert.ToInt32(product.PCode);

            return View(new AgProduct {
                PCode = ""+ (num + 1) ,
                OBal=0,
                Qin=0,
                Qout=0,
                Sprice=0,
                Pprice=0,
                DateEntered=DateTime.Today,
                Expirydate = DateTime.Today
            });
        }

        // POST: AgProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PCode,PName,SNo,Qin,Qout,DateEntered,LastDUpdated,UserId,AuditDate,OBal,SupplierId,Serialized,Unserialized,Seria,Pprice,Sprice,Branch,Draccno,Craccno,Ai,Expirydate,Run,Process1,Process2,Remarks,saccocode")] AgProduct agProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var checkproductExist = _context.AgProducts.Any(a => a.saccocode == sacco && a.PName== agProduct.PName && a.Branch == saccobranch);
            if (checkproductExist)
            {
                GetInitialValues();
                _notyf.Error("Product Already exist");
                return View();
            }
            var checkproductExistCode = _context.AgProducts.Any(a => a.saccocode == sacco && a.PCode == agProduct.PCode && a.Branch == saccobranch);
            if (checkproductExistCode)
            {
                GetInitialValues();
                _notyf.Error("Product Code Already exist");
                return View();
            }
            if (agProduct.Qin==0)
            {
                GetInitialValues();
                _notyf.Error("Quantity should be Greater Than Zero");
                return View();
            }
            if (agProduct.Sprice == 0)
            {
                GetInitialValues();
                _notyf.Error("Selling Price should be Greater Than Zero");
                return View();
            }
            if (agProduct.Pprice == 0)
            {
                GetInitialValues();
                _notyf.Error("Buying Price should be Greater Than Zero");
                return View();
            }
            if (ModelState.IsValid)
            {
                var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
                agProduct.UserId = user;
                agProduct.AuditDate = DateTime.Now;
                agProduct.saccocode = sacco;
                agProduct.Branch = saccobranch;

                var checkproductifExist = _context.AgProducts.Any(a => a.saccocode == sacco && a.PCode == agProduct.PCode && a.Branch == saccobranch);
                if (!checkproductifExist)
                {
                    agProduct.Qin = agProduct.Qin;
                    agProduct.Qout = agProduct.Qin;
                    agProduct.OBal = agProduct.Qin;

                    _context.AgProducts.Add(agProduct);
                }
                else
                {
                    var checkquantity = _context.AgProducts.Where(a => a.saccocode == sacco && a.PCode == agProduct.PCode && a.Branch == saccobranch).Sum(i => i.Qin);
                    double? qnty = checkquantity;
                    agProduct.Qin = qnty+checkquantity;
                    _context.AgProducts.Update(agProduct);
                }

                _context.AgProducts4s.Add(new AgProducts4 {
                    PName = agProduct.PName,
                    PCode = agProduct.PCode,
                    Qin = agProduct.Qin,
                    Qout = agProduct.Qin,
                    DateEntered = agProduct.DateEntered,
                    LastDUpdated = agProduct.LastDUpdated,
                    UserId = user,
                    AuditDate = DateTime.Now,
                    OBal= agProduct.Qin,
                    SupplierId= agProduct.SupplierId,
                    Pprice= agProduct.Pprice,
                    Sprice= agProduct.Sprice,
                    Branch= agProduct.Branch,
                    Draccno= agProduct.Draccno,
                    Craccno = agProduct.Craccno,
                    saccocode = sacco
                });
                await _context.SaveChangesAsync();
                _notyf.Success("Stock Added Successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(agProduct);
        }

        // GET: AgProducts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();

            if (id == null)
            {
                return NotFound();
            }
            //long.TryParse(agProduct.Id, out long id);

            var agProduct = await _context.AgProducts.FindAsync(id);
            if (agProduct == null)
            {
                return NotFound();
            }
            agProduct.DateEntered = DateTime.Now;
            agProduct.Qin = 0;
            return View(agProduct);
        }

        // POST: AgProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,PCode,PName,SNo,Qin,Qout,DateEntered,LastDUpdated,UserId,AuditDate,OBal,SupplierId,Serialized,Unserialized,Seria,Pprice,Sprice,Branch,Draccno,Craccno,Ai,Expirydate,Run,Process1,Process2,Remarks,saccocode")] AgProduct agProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);

            if (id != agProduct.Id)
            {
                GetInitialValues();
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var Quantity = _context.AgProducts.Where(u => u.saccocode == sacco && u.PCode == agProduct.PCode).Select(p => p.Qin);
                    //agProduct.Qin = Quantity + agProduct.Qin;
                    //_context.Update(agProduct);
            
                    var checkproductifExist = _context.AgProducts.Any(a => a.saccocode == sacco && a.PCode == agProduct.PCode && a.Branch== saccobranch);
                    double? existingkgs = 0;
                    var checkquantity = _context.AgProducts.Where(a => a.saccocode == sacco && a.PCode == agProduct.PCode && a.Branch == saccobranch).Sum(i => i.Qin);
                    existingkgs = checkquantity;
                    if (!checkproductifExist)
                    {

                        _context.AgProducts.Add(agProduct);
                    }
                    else
                    {
                        double? qnty = checkquantity;
                        agProduct.Qin = agProduct.Qin;
                        agProduct.Qout = agProduct.Qin;
                        agProduct.OBal = qnty + agProduct.Qin;
                        _context.AgProducts.Update(agProduct);
                    }
                    var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    //var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    //&& a.Branch== saccobranch
                    agProduct.UserId = user;
                    agProduct.AuditDate = DateTime.Now;
                    agProduct.saccocode = sacco;
                    _context.AgProducts4s.Add(new AgProducts4
                    {
                        PName = agProduct.PName,
                        PCode = agProduct.PCode,
                        Qin = agProduct.Qin,
                        Qout = agProduct.Qin,
                        DateEntered = agProduct.DateEntered,
                        LastDUpdated = agProduct.LastDUpdated,
                        UserId = user,
                        AuditDate = DateTime.Now,
                        OBal = agProduct.OBal,
                        SupplierId = agProduct.SupplierId,
                        Pprice = agProduct.Pprice,
                        Sprice = agProduct.Sprice,
                        Branch = agProduct.Branch,
                        Draccno = agProduct.Draccno,
                        Craccno = agProduct.Craccno,
                        saccocode = sacco
                    });
                    await _context.SaveChangesAsync();
                    _notyf.Success("Stock Edited Successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgProductExists(agProduct.Id))
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
            return View(agProduct);
        }

        // GET: AgProducts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                GetInitialValues();
                return NotFound();
            }

            var agProduct = await _context.AgProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agProduct == null)
            {
                return NotFound();
            }

            return View(agProduct);
        }

        // POST: AgProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var agProduct = await _context.AgProducts.FindAsync(id);
            _context.AgProducts.Remove(agProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgProductExists(long id)
        {
            utilities.SetUpPrivileges(this);
            return _context.AgProducts.Any(e => e.Id == id);
        }
    }
}
