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
            var products = await _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderBy(p => p.PName).ToListAsync();
            return View(products);
        }
        // GET: AgProducts
        public async Task<IActionResult> ChangeIndex()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            //GetInitialValues();
            var products = await _context.ag_Products45.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderBy(p => p.p_name).ToListAsync();
            //.("dd/MM/yyy")
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
        public IActionResult Create()//Changeproduct
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

            return View(new AgProduct
            {
                PCode = "" + (num + 1),
                OBal = 0,
                Qin = 0,
                Qout = 0,
                Sprice = 0,
                Pprice = 0,
                DateEntered = DateTime.Today,
                Expirydate = DateTime.Today
            });
        }
        // GET: AgProducts/Create
        public IActionResult Changeproduct()//
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);

            IQueryable<AgProduct> agProducts = _context.AgProducts;
            var products = agProducts.Where(s => s.saccocode.ToUpper().Equals(sacco.ToUpper()) && s.Branch == saccobranch).ToList();
            ViewBag.productslist = new SelectList(products, "PCode", "PName");

            var agsuppliers = agProducts.Where(L => L.saccocode == sacco && L.Branch == saccobranch).ToList();
            ViewBag.agsuppliers = agsuppliers;

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Savepartial([FromBody] ag_Products45 productIntake, bool individual)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";

            if (string.IsNullOrEmpty(productIntake.p_name))
            {
                _notyf.Error("Sorry, Kindly provide Product Name.");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.Initsprice == 0)
            {
                _notyf.Error("Sorry, Kindly provide Product Name.");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.Newpprice == 0)
            {
                _notyf.Error("Sorry, Kindly provide Buying Price.");
                return Json(new
                {
                    success = false
                });
            }
            if (productIntake.Newpprice > productIntake.Newsprice)
            {
                _notyf.Error("Sorry, Buying Price cannot be morethan Selling Price.");
                return Json(new
                {
                    success = false
                });
            }
            IQueryable<AgProduct> agProducts = _context.AgProducts;
            IQueryable<Gltransaction> gltransactions = _context.Gltransactions;
            IQueryable<AgReceipt> agReceipts = _context.AgReceipts;

            var checktheproducttoupdate = agProducts.FirstOrDefault(K => K.saccocode == sacco && K.Branch == saccoBranch && K.PCode == productIntake.p_code);
            if (string.IsNullOrEmpty(checktheproducttoupdate.PName))
            {
                _notyf.Error("Sorry, Product does not Exist.");
                return Json(new
                {
                    success = false
                });
            }
            if (checktheproducttoupdate != null)
            {

                checktheproducttoupdate.PCode = productIntake.p_code;
                checktheproducttoupdate.PName = checktheproducttoupdate.PName;
                checktheproducttoupdate.Qin = checktheproducttoupdate.Qin;
                checktheproducttoupdate.Qout = checktheproducttoupdate.Qout;
                checktheproducttoupdate.DateEntered = productIntake.Date_Entered;
                checktheproducttoupdate.LastDUpdated = productIntake.Date_Entered;
                checktheproducttoupdate.UserId = loggedInUser;
                checktheproducttoupdate.AuditDate = DateTime.Now;
                checktheproducttoupdate.OBal = checktheproducttoupdate.OBal;
                checktheproducttoupdate.SupplierId = checktheproducttoupdate.SupplierId;
                checktheproducttoupdate.Pprice = productIntake.Newpprice;
                checktheproducttoupdate.Sprice = productIntake.Newsprice;
                checktheproducttoupdate.Branch = saccoBranch;
                checktheproducttoupdate.Draccno = checktheproducttoupdate.Draccno;
                checktheproducttoupdate.Craccno = checktheproducttoupdate.Craccno;
                checktheproducttoupdate.saccocode = sacco;
                _context.AgProducts.Update(checktheproducttoupdate);

            }

            var collection = new ag_Products45
            {
                p_code = productIntake.p_code.Trim().ToUpper(),
                Date_Entered = productIntake?.Date_Entered ?? DateTime.Today,
                p_name = checktheproducttoupdate.PName,
                Initpprice = productIntake.Initpprice,
                Initsprice = productIntake.Initsprice,
                Initbal = productIntake.Initbal,
                Newpprice = productIntake.Newpprice,
                Newsprice = productIntake.Newsprice,
                Newbal = 0,
                saccocode = sacco,
                Branch = saccoBranch,
                User = loggedInUser,
                audit_date = DateTime.Now,
            };
            _context.ag_Products45.Add(collection);

            if (individual)
            {
                var getgltransactions = gltransactions.Where(m => m.SaccoCode == sacco && m.TransDate >= productIntake.Date_Entered
                && m.TransDescript == "Stock Receive" && m.Source == checktheproducttoupdate.SupplierId).ToList();
                var agProducts4 = _context.AgProducts4s.Where(n => n.saccocode == sacco && n.PCode == productIntake.p_code
                && n.DateEntered >= productIntake.Date_Entered).ToList();
                var groupbydate = agProducts4.GroupBy(m => m.DateEntered).ToList();
                groupbydate.ForEach(n =>
                {
                    var getindividualDateproduct = agProducts4.Where(m => m.DateEntered == n.Key).ToList();
                    getindividualDateproduct.ForEach(p =>
                    {
                        p.PCode = p.PCode;
                        p.PName = p.PName;
                        p.SNo = p.SNo;
                        p.Qin = p.Qin;
                        p.Qout = p.Qout;
                        p.DateEntered = p.DateEntered;
                        p.LastDUpdated = p.LastDUpdated;
                        p.UserId = loggedInUser;
                        p.AuditDate = DateTime.Now;
                        p.OBal = p.OBal;
                        p.SupplierId = p.SupplierId;
                        p.Serialized = p.Serialized;
                        p.Unserialized = p.Unserialized;
                        p.Seria = p.Seria;
                        p.Pprice = productIntake.Newpprice;
                        p.Sprice = p.Sprice;
                        p.Branch = saccoBranch;
                        p.Draccno = p.Draccno;
                        p.Craccno = p.Craccno;
                        p.Ai = p.Ai;
                        p.saccocode = p.saccocode;
                        p.Approved = p.Approved;
                        _context.AgProducts4s.Update(p);

                        decimal getamount = (decimal)((decimal)p.Qin * productIntake.Initpprice);
                        var selectfromgl = getgltransactions.FirstOrDefault(k => k.Amount == getamount && k.TransDate == p.DateEntered
                        && k.DrAccNo == p.Draccno && k.CrAccNo == p.Craccno);
                        if (selectfromgl != null)
                        {
                            selectfromgl.AuditId = loggedInUser;
                            selectfromgl.TransDate = (DateTime)selectfromgl.TransDate;
                            selectfromgl.Amount = (decimal)(productIntake.Newpprice * (decimal)p.Qin);
                            selectfromgl.AuditTime = DateTime.Now;
                            selectfromgl.DocumentNo = "Stock Receive" +" "+ p.PCode;
                            selectfromgl.Source = selectfromgl.Source;
                            selectfromgl.TransDescript = "Stock Receive";
                            selectfromgl.Transactionno = $"{loggedInUser}{DateTime.Now}";
                            selectfromgl.SaccoCode = sacco;
                            selectfromgl.DrAccNo = selectfromgl.DrAccNo;
                            selectfromgl.CrAccNo = selectfromgl.CrAccNo;
                            _context.Gltransactions.Update(selectfromgl);
                        }


                    });
                });

                var getallsalestochangebuying = agReceipts.Where(m => m.saccocode == sacco && m.TDate >= productIntake.Date_Entered
                && m.PCode == productIntake.p_code).ToList();
                var groupedsales = getallsalestochangebuying.GroupBy(n => n.TDate).ToList();
                groupedsales.ForEach(b => {
                    var getindividualSalesproduct = getallsalestochangebuying.Where(m => m.TDate == b.Key).ToList();
                    getindividualSalesproduct.ForEach(v => {
                        v.RNo = v.RNo;
                        v.PCode = v.PCode;
                        v.TDate = v.TDate;
                        v.Amount = v.Amount;
                        v.SNo = v.SNo;
                        v.Qua = v.Qua;
                        v.SBal = v.SBal;
                        v.UserId = loggedInUser;
                        v.AuditDate = v.AuditDate;
                        v.Cash = v.Cash;
                        v.Sno1 = v.Sno1;
                        v.Transby = v.Transby;
                        v.Idno = v.Idno;
                        v.Mobile = v.Mobile;
                        v.Remarks = v.Remarks;
                        v.Branch = v.Branch;
                        v.Sprice = v.Sprice;
                        v.Bprice = (decimal)(v.Qua) * (productIntake.Newpprice);
                        v.Ai = 0;
                        v.Run = 0;
                        v.Paid = 0;
                        v.Completed = 0;
                        v.Salesrep = "";
                        v.saccocode = sacco;
                        v.Zone = v.Zone;
                        _context.AgReceipts.Update(v);
                        });
                });
            }

            _context.SaveChanges();
            _notyf.Success("Intake saved successfully");


            return Json(new
            {
                success = true
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
            if (agProduct.Qin == 0)
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
                    Approved = false,
                    saccocode = sacco
                });

                var getGLS = _context.AgSupplier1s.FirstOrDefault(g => g.CompanyName.ToUpper().Equals(agProduct.SupplierId.ToUpper())
                && g.saccocode == sacco);
                _context.Gltransactions.Add(new Gltransaction
                {
                    AuditId = loggedInUser,
                    TransDate = (DateTime)agProduct.DateEntered,
                    Amount = (decimal)(agProduct.Pprice * (decimal)agProduct.Qin),
                    AuditTime = DateTime.Now,
                    DocumentNo = "Stock Receive" + agProduct.PCode,
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
                _context.AgProducts.Add(new AgProduct
                {
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
