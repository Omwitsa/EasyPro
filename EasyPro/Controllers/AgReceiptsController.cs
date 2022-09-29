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
using EasyPro.ViewModels.TranssupplyVM;

namespace EasyPro.Controllers
{
    public class AgReceiptsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public AgReceiptsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
       
        // GET: AgReceipts
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            return View(await _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.TDate == DateTime.Today && i.Branch == saccobranch)
                .OrderByDescending(s => s.AuditDate).ToListAsync());
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var agproducts = _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch).Select(b => b.PName).ToList();
            ViewBag.agproductsall = new SelectList(agproducts, "");

            var branches = _context.DBranch.Where(i => i.Bcode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Bname).ToList();
            ViewBag.branches = new SelectList(branches, "");

        }
        //PRODUCT SALES
        [HttpPost]
        public JsonResult Save([FromBody] List<ProductIntake> intakes, string RNo)
        {
            try
            {
                if (!intakes.Any())
                {
                    _notyf.Error("Sorry, Kindly provide records");
                    return Json("");
                }
                var da = intakes;
                var cash = "";
                da.ForEach(d =>
                {
                    cash = d.Sno;

                });
                
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
                intakes.ForEach(t =>
                {
                    t.Description = t?.Description ?? "";
                    t.SaccoCode = sacco;
                    t.Branch = saccobranch;
                    t.TransactionType = TransactionType.Deduction;
                    t.AuditId = loggedInUser;
                    if (t.Sno == "")
                    {
                        t.Sno = "cash";
                    }
                    var product = _context.AgProducts.FirstOrDefault(p => p.PName.ToUpper().Equals(t.Description.ToUpper())
                    && p.saccocode == sacco && p.Branch== saccobranch);
                    if(product != null)
                    {
                        var bal = product.OBal - (double?)t.Qsupplied;
                        _context.AgReceipts.Add(new AgReceipt
                        {
                            RNo = RNo,
                            PCode = product.PCode,
                            TDate = t.TransDate,
                            Amount = t.DR,
                            SNo = t.Sno,
                            Qua = (double?)t.Qsupplied,
                            SBal = bal,
                            UserId = loggedInUser,
                            AuditDate = DateTime.Now,
                            Cash = false,
                            Sno1 = t.Sno,
                            Transby = "",
                            Idno = "",
                            Mobile = "",
                            Remarks = t.Description,
                            Branch  = t.Branch,
                            Sprice = product.Sprice,
                            Bprice = product.Pprice,
                            Ai = 0,
                            Run = 0,
                            Paid = 0,
                            Completed = 0,
                            Salesrep = "",
                            saccocode = sacco
                        });

                        _context.Gltransactions.Add(new Gltransaction
                        {
                            AuditId = loggedInUser,
                            TransDate = t.TransDate,
                            Amount = (decimal)t.DR,
                            AuditTime = DateTime.Now,
                            DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                            Source = t.Sno,
                            TransDescript = t.Remarks,
                            Transactionno = $"{loggedInUser}{DateTime.Now}",
                            SaccoCode = sacco,
                            DrAccNo = product.Draccno,
                            CrAccNo = product.Craccno,
                        });

                        product.Qin = bal;
                        product.Qout = bal;
                        product.OBal = bal;
                    }
                    
                });
                
                if (cash != "")
                {
                    _context.ProductIntake.AddRange(intakes);
                }


                _context.SaveChanges();
                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        //PRODUCT REVERSAL
        [HttpPost]
        public JsonResult SaveReturns([FromBody] List<ProductIntake> intakes, string RNo)
        {
            try
            {
                
                if (!intakes.Any())
                {
                    _notyf.Error("Sorry, Kindly provide records");
                    return Json("");
                }

                var da = intakes;
                var cash = "";
                da.ForEach(d =>
                {
                    cash = d.Sno;

                });
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                intakes.ForEach(t =>
                {
                    t.Description = t?.Description ?? "";
                    t.SaccoCode = sacco;
                    t.Branch = saccobranch;
                    t.TransactionType = TransactionType.Deduction;
                    t.AuditId = loggedInUser;
                    t.Qsupplied = t.Qsupplied * -1;
                    t.CR = t.DR;
                    t.DR = 0;
                    if (t.Sno == "")
                    {
                        t.Sno = "cash";
                    }

                    var product = _context.AgProducts.FirstOrDefault(p => p.PName.ToUpper().Equals(t.Description.ToUpper())
                    && p.saccocode == sacco && p.Branch== saccobranch);
                    if (product != null)
                    {
                        var bal = product.OBal + (double?)t.Qsupplied;
                        _context.AgReceipts.Add(new AgReceipt
                        {
                            RNo = RNo,
                            PCode = product.PCode,
                            TDate = t.TransDate,
                            Amount = t.CR * -1,
                            SNo = t.Sno,
                            Qua = (double?)t.Qsupplied,
                            SBal = bal,
                            UserId = loggedInUser,
                            AuditDate = DateTime.Now,
                            Cash = false,
                            Sno1 = t.Sno,
                            Transby = "",
                            Idno = "",
                            Mobile = "",
                            Remarks = t.Description,
                            Branch = t.Branch,
                            Sprice = product.Sprice * -1,
                            Bprice = product.Pprice * -1,
                            Ai = 0,
                            Run = 0,
                            Paid = 0,
                            Completed = 0,
                            Salesrep = "",
                            saccocode = sacco
                        });

                        _context.Gltransactions.Add(new Gltransaction
                        {
                            AuditId = loggedInUser,
                            TransDate = t.TransDate,
                            Amount = (decimal)t.CR,
                            AuditTime = DateTime.Now,
                            DocumentNo = DateTime.Now.ToString().Replace("/", "").Replace("-", ""),
                            Source = t.Sno,
                            TransDescript = t.Remarks,
                            Transactionno = $"{loggedInUser}{DateTime.Now}",
                            SaccoCode = sacco,
                            CrAccNo = product.Draccno,
                            DrAccNo = product.Craccno,
                        });

                        product.Qin = bal;
                        product.Qout = bal;
                        product.OBal = bal;
                    }
                });
                if (cash != "")
                {
                    _context.ProductIntake.AddRange(intakes);
                }
                   

                _context.SaveChanges();
                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        // GET: AgReceipts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                GetInitialValues();
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts
                .FirstOrDefaultAsync(m => m.RId == id);
            if (agReceipt == null)
            {
                return NotFound();
            }

            return View(agReceipt);
        }

        // GET: AgReceipts/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch== saccobranch)
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();
            //AgProductobj = _context.AgProducts
            //.Where(p => p.saccocode == sacco)
            //.OrderBy(s => s.PCode);

            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };

            var transporters = _context.DTransporters.Where(s => s.ParentT == sacco && s.Tbranch == saccobranch).ToList();
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco && s.Branch== saccobranch).ToList();
            var products = _context.AgProducts.Where(p => p.saccocode == sacco && p.Branch== saccobranch).ToList();
            var intakes = _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && u.Branch== saccobranch);
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                DTransporter = transporters,
                DSuppliers = suppliers,
                AgProductobj = products,
                ProductIntake= intakes
            };
            return View(agrovetsales);
            //Agrovetsalesobj = new Agrovetsales
            //{
            //    AgReceipt = new AgReceipt(),
            //    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
            //    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
            //};
        }

        // POST: AgReceipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (ModelState.IsValid)
            {
                _context.Add(agReceipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agReceipt);
        }

        public IActionResult CreateReversal()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccobranch)
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();

            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };

            var transporters = _context.DTransporters.Where(s => s.ParentT == sacco && s.Tbranch== saccobranch).ToList();
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco && s.Branch== saccobranch).ToList();
            var products = _context.AgProducts.Where(p => p.saccocode == sacco && p.Branch== saccobranch).ToList();
            var intakes = _context.ProductIntake.Where(u => u.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && u.Branch== saccobranch);
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                DTransporter = transporters,
                DSuppliers = suppliers,
                AgProductobj = products,
                ProductIntake = intakes
            };
            return View(agrovetsales);
            //Agrovetsalesobj = new Agrovetsales
            //{
            //    AgReceipt = new AgReceipt(),
            //    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
            //    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
            //};
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReversal([Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (ModelState.IsValid)
            {
                _context.Add(agReceipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agReceipt);
        }

        // GET: AgReceipts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Branch== saccobranch)
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();

            if (id == null)
            {
                return NotFound();
            }
            
            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };

            var agReceipt = await _context.AgReceipts.FindAsync(id);
            if (agReceipt == null)
            {
                return NotFound();
            }
            return View(agReceipt);
        }

        // POST: AgReceipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            utilities.SetUpPrivileges(this);
            if (id != agReceipt.RId)
            {
                GetInitialValues();
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agReceipt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgReceiptExists(agReceipt.RId))
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
            return View(agReceipt);
        }

        // GET: AgReceipts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts
                .FirstOrDefaultAsync(m => m.RId == id);
            if (agReceipt == null)
            {
                return NotFound();
            }

            return View(agReceipt);
        }

        // POST: AgReceipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            var agReceipt = await _context.AgReceipts.FindAsync(id);
            _context.AgReceipts.Remove(agReceipt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgReceiptExists(long id)
        {
            GetInitialValues();
            return _context.AgReceipts.Any(e => e.RId == id);
        }
    }
}
