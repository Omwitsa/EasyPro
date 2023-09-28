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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var products = await _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderBy(p => p.PName).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                products = products.Where(p => p.Branch == saccobranch).ToList();
            return View(products);
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

            var agsuppliers = _context.AgSupplier1s.Where(L => L.saccocode == sacco).ToList();
            ViewBag.agsuppliers = agsuppliers;


        }

        // GET: AgProducts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var product = _context.AgProducts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => Convert.ToInt32(u.PCode)).FirstOrDefault();
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var checkproductExist = _context.AgProducts.Any(a => a.saccocode == sacco && a.PName.ToLower().Equals(agProduct.PName.ToLower()) && a.Branch == saccobranch);
            if (checkproductExist)
            {
                GetInitialValues();
                _notyf.Error("Product Name Already exist");
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
                    Approved = false,
                    saccocode = sacco
                });

                var getGLS = _context.AgSupplier1s.FirstOrDefault(g => g.CompanyName.ToUpper().Equals(agProduct.SupplierId.ToUpper())
                && g.saccocode== sacco);
                _context.Gltransactions.Add(new Gltransaction
                {
                    AuditId = loggedInUser,
                    TransDate = (DateTime)agProduct.DateEntered,
                    Amount = (decimal)(agProduct.Pprice* (decimal)agProduct.Qin),
                    AuditTime = DateTime.Now,
                    DocumentNo = "Stock Receive",
                    Source = getGLS.CompanyName,
                    TransDescript = "Stock Receive",
                    Transactionno = $"{loggedInUser}{DateTime.Now}",
                    SaccoCode = sacco,
                    DrAccNo = getGLS.AccDr,
                    CrAccNo = getGLS.AccCr,
                });

                await _context.SaveChangesAsync();
                _notyf.Success("Stock Added Successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(agProduct);
        }

        public IActionResult UnApprovedProducts()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var products4s = _context.AgProducts4s.Where(p => p.saccocode == sacco && !p.Approved).ToList();
            return View(products4s);
        }

        public IActionResult ApproveProduct(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                _notyf.Error("Sorry, Product not found");
                return NotFound();
            }
            var agProduct = _context.AgProducts4s.FirstOrDefault(p => p.Id == id);
            if (agProduct == null)
            {
                _notyf.Error("Sorry, Product not found");
                return NotFound();
            }
            var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var product = _context.AgProducts.FirstOrDefault(a => a.saccocode == agProduct.saccocode && a.PCode == agProduct.PCode && a.Branch == agProduct.Branch);
            if (product == null)
            {
                _context.AgProducts.Add(new AgProduct {
                    PCode = agProduct.PCode,
                    PName = agProduct.PName,
                    Qin = agProduct.Qin,
                    Qout = agProduct.Qout,
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
                    saccocode = agProduct.saccocode
                });
            }
            else
            {
                product.Qin = agProduct.Qin;
                product.OBal = agProduct.Qin + product.OBal;
                product.Pprice = agProduct.Pprice;
                product.Sprice = agProduct.Sprice;
            }

            agProduct.Approved = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: AgProducts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id != agProduct.Id)
            {
                _notyf.Error("Sorry, Product not found");
                return View(agProduct);
            }

            if (agProduct.Qin < 1)
            {
                _notyf.Error("Sorry, Product should be more than Zero");
                return View(agProduct);
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var getGLS = _context.AgSupplier1s.FirstOrDefault(g => g.CompanyName.ToUpper().Equals(agProduct.SupplierId.ToUpper())
                    && g.saccocode == sacco);
                    var user = HttpContext.Session.GetString(StrValues.LoggedInUser);
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

                    _context.Gltransactions.Add(new Gltransaction
                    {
                        AuditId = loggedInUser,
                        TransDate = (DateTime)agProduct.DateEntered,
                        Amount = (decimal)(agProduct.Pprice * (decimal)agProduct.Qin),
                        AuditTime = DateTime.Now,
                        DocumentNo = "Stock Receive",
                        Source = getGLS.CompanyName,
                        TransDescript = "Stock Receive",
                        Transactionno = $"{loggedInUser}{DateTime.Now}",
                        SaccoCode = sacco,
                        DrAccNo = getGLS.AccDr,
                        CrAccNo = getGLS.AccCr,
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var agProduct = await _context.AgProducts.FindAsync(id);
            _context.AgProducts.Remove(agProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult SelectedDateIntake(string pname)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var todaysIntake = _context.AgSupplier1s.Where(L => L.CompanyName.ToUpper().Equals(pname.ToUpper()) && L.saccocode == sacco).ToList();

            return Json(todaysIntake);
        }

        private bool AgProductExists(long id)
        {
            utilities.SetUpPrivileges(this);
            return _context.AgProducts.Any(e => e.Id == id);
        }
    }
}
