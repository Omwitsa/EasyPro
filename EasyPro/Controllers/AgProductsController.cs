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
            GetInitialValues();
            return View(await _context.AgProducts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var suppliers = _context.Suppliers.Where(i => i.saccocode == sacco).Select(b => b.Names).ToList();
            ViewBag.suppliers = new SelectList(suppliers);

            var products = _context.DBranchProducts.Where(a => a.saccocode == sacco).Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
            var glAccounts = _context.Glsetups.ToList();
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
            var count = _context.AgProducts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u=>u.PCode)
                .Select(b => b.PCode);
            var selectedno = count.FirstOrDefault();
            double num =Convert.ToInt32(selectedno);
            return View(new AgProduct {
                PCode = ""+ (num+1) ,
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
            var checkproductExist = _context.AgProducts.Any(a => a.saccocode == sacco && a.PName== agProduct.PName);
            if (checkproductExist)
            {
                GetInitialValues();
                _notyf.Error("Product Already exist");
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
                // S_No, , , , , user_id, audit_date, o_bal, SupplierID,
                ////Serialized, unserialized, seria, pprice, sprice, Branch, DRACCNO, CRACCNO, AI, saccocode
                
                var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
                agProduct.UserId = user;
                agProduct.AuditDate = DateTime.Now;
                agProduct.saccocode = sacco;
                _context.AgProducts.Add(agProduct);
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
            return View(agProduct);
        }

        // POST: AgProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,PCode,PName,SNo,Qin,Qout,DateEntered,LastDUpdated,UserId,AuditDate,OBal,SupplierId,Serialized,Unserialized,Seria,Pprice,Sprice,Branch,Draccno,Craccno,Ai,Expirydate,Run,Process1,Process2,Remarks,saccocode")] AgProduct agProduct)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != agProduct.PCode)
            {
                GetInitialValues();
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var Quantity = _context.AgProducts.Where(u => u.saccocode == sacco && u.PCode == agProduct.PCode).Select(p => p.Qin);
                    agProduct.Qin = (Convert.ToInt32(Quantity) + agProduct.Qin);
                    _context.Update(agProduct);
                    var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
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
                        OBal = agProduct.Qin,
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
                    if (!AgProductExists(agProduct.PCode))
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
        public async Task<IActionResult> Delete(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                GetInitialValues();
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

        // POST: AgProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);
            var agProduct = await _context.AgProducts.FindAsync(id);
            _context.AgProducts.Remove(agProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgProductExists(string id)
        {
            utilities.SetUpPrivileges(this);
            return _context.AgProducts.Any(e => e.PCode == id);
        }
    }
}
