using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels.TranssupplyVM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace EasyPro.Controllers
{
    public class DTransportsController : Controller
    {
        private readonly MORINGAContext _context;
        private IEnumerable<DSupplier> DSuppliers;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public DTransportsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }
        public TransSuppliers TransSuppliersobj { get; private set; }
        public IEnumerable<DTransporter> DTransporter { get; private set; }
        public DTransporter dtransporterobj { get; private set; }
        // GET: DTransports
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var intakes = GetAssignedTransporters();

            return View(intakes);
        }
        private List<TransSuppliersVM> GetAssignedTransporters()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var today = DateTime.Now;
            var month = new DateTime(today.Year, today.Month, 1);
            var startdate = month;
            var enddate = startdate.AddMonths(1).AddDays(-1);
            var transdeduction = _context.DTransports.Where(i => i.Active && i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
            var intakes = new List<TransSuppliersVM>();
            foreach (var intake in transdeduction)
            {
                var trans = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.TransCode && i.ParentT.ToUpper().Equals(sacco.ToUpper()));
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == intake.Sno && i.Scode.ToUpper().Equals(sacco.ToUpper()));
                if(supplier != null)
                {
                    intakes.Add(new TransSuppliersVM
                    {
                        Id = intake.Id,
                        TransCode = trans.TransCode,
                        TransName = trans.TransName,
                        Sno = supplier.Sno,
                        Names = supplier.Names,
                        Rate = intake.Rate,
                        Startdate = intake.Startdate,
                    });
                }
            }

            return intakes;
        }
        // GET: DTransports/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransport == null)
            {
                return NotFound();
            }
            return View(dTransport);
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            var producttypes = _context.DPrices.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.Products).ToList();
            ViewBag.producttypes = new SelectList(producttypes, "");
        }
        // GET: DTransports/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            TransSuppliersobj = new TransSuppliers
            {
                DTransport = new DTransport(),
                DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
                DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
            };
            GetInitialValues();
            //return Json(new { data = Farmersobj });
            return View(TransSuppliersobj);
        }

        // POST: DTransports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,Sno,Rate,Startdate,Active,DateInactivate,Auditid,Auditdatetime,Isfrate,producttype")] DTransport dTransport)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var checkNonExistingTransporter = _context.DTransporters.Any(i => i.TransCode == dTransport.TransCode && i.Active && i.ParentT.ToUpper().Equals(sacco.ToUpper()));
            if (!checkNonExistingTransporter)
            {
                _notyf.Error("TransCode not Exist or InActive");
                TransSuppliersobj = new TransSuppliers
                {
                    DTransport = new DTransport(),
                    //DTransport = _context.DTransports,
                    DTransporter = _context.DTransporters.Where(i=>i.ParentT.ToUpper().Equals(sacco.ToUpper())),
                    DSuppliers = _context.DSuppliers.Where(i=>i.Scode.ToUpper().Equals(sacco.ToUpper())),
                };
                GetInitialValues();
                return View(TransSuppliersobj);
            }
            var dTransporterExists = _context.DTransports
                .Any(i => i.Sno == dTransport.Sno && i.Active == true && i.producttype == dTransport.producttype && i.saccocode.ToUpper().Equals(sacco.ToUpper()));
            if (dTransporterExists)
            {
                _notyf.Error("Supplier has an active Assignment");
                TransSuppliersobj = new TransSuppliers
                {
                    DTransport = new DTransport(),
                    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
                    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
                };
                GetInitialValues();
                return View(TransSuppliersobj);
            }
            if (ModelState.IsValid)
            {
                dTransport.saccocode = sacco;
                _context.Add(dTransport);
                UpdateLastKgs(dTransport);
                if (dTransport.DateInactivate != null)
                    UpdateIntakeKgs(dTransport);
                await _context.SaveChangesAsync();
                _notyf.Success("Assignment saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dTransport);
        }
        private void UpdateLastKgs(DTransport dTransport)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var transExisting = _context.DTransports
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Active && i.producttype == dTransport.producttype && i.Sno == dTransport.Sno);
            if (transExisting.Any())
            {
                var selectintaketobeupdated = _context.ProductIntake
                    .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.ProductType == dTransport.producttype && i.TransDate >= dTransport.Startdate && i.Sno == dTransport.Sno.ToString() && i.Qsupplied != 0)
                    .Sum(a => a.Qsupplied);
                if (selectintaketobeupdated > 0)
                {
                    var intakekgs = _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.Qsupplied != 0);
                    if (intakekgs.Any())
                    {
                        foreach (var details in intakekgs)
                        {
                            details.Sno = dTransport.Sno.ToString();
                            details.TransDate = DateTime.Today;
                            details.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                            details.ProductType = dTransport.producttype;
                            details.Qsupplied = selectintaketobeupdated;
                            var price = _context.DPrices
                                .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                                && p.Products.ToUpper().Equals(details.ProductType.ToUpper()));
                            details.Ppu = price.Price;
                            details.CR = 0;
                            details.DR = (selectintaketobeupdated * price.Price);
                            details.Balance = GetBalance(dTransport);
                            details.Description = "Transport";
                            details.Paid = false;
                            details.TransactionType = TransactionType.Intake;
                            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                            details.Auditdatetime = DateTime.Now;
                            var branch = _context.DBranch.FirstOrDefault(b => b.Bcode.ToUpper().Equals(sacco.ToUpper()));
                            details.Branch = branch.Bname;
                            details.SaccoCode = sacco;
                            details.DrAccNo = price.DrAccNo;
                            details.CrAccNo = price.CrAccNo;
                        }

                    }
                    var transport = _context.ProductIntake.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.Qsupplied != 0);
                    if (transport.Any())
                    {
                        foreach (var transdetails in transport)
                        {
                            transdetails.Sno = dTransport.TransCode.ToString();
                            transdetails.TransDate = DateTime.Today;
                            transdetails.TransTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                            transdetails.ProductType = dTransport.producttype;
                            transdetails.Qsupplied = selectintaketobeupdated;
                            var price = _context.DPrices
                                .FirstOrDefault(p => p.SaccoCode.ToUpper().Equals(sacco.ToUpper())
                                && p.Products.ToUpper().Equals(transdetails.ProductType.ToUpper()));
                            transdetails.Ppu = price.Price;
                            transdetails.DR = 0;
                            transdetails.CR = (selectintaketobeupdated * price.Price);
                            transdetails.Balance = GetBalance(dTransport);
                            transdetails.Description = "Transport";
                            transdetails.Paid = false;
                            transdetails.TransactionType = TransactionType.Intake;
                            var auditId = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                            transdetails.Auditdatetime = DateTime.Now;
                            var branch = _context.DBranch.FirstOrDefault(b => b.Bcode.ToUpper().Equals(sacco.ToUpper()));
                            transdetails.Branch = branch.Bname;
                            transdetails.SaccoCode = sacco;
                            transdetails.DrAccNo = price.TransportDrAccNo;
                            transdetails.CrAccNo = price.TransportCrAccNo;
                        }

                    }
                }

                _context.SaveChanges();
            }

        }
        private decimal? GetBalance(DTransport dTransport)
        {
            var latestIntake = _context.ProductIntake.Where(i => i.Sno == dTransport.Sno.ToString())
                    .OrderByDescending(i => i.Id).FirstOrDefault();
            if (latestIntake == null)
                latestIntake = new ProductIntake();
            latestIntake.Balance = latestIntake?.Balance ?? 0;
            latestIntake.DR = latestIntake?.DR ?? 0;
            latestIntake.CR = latestIntake?.CR ?? 0;
            var balance = latestIntake.Balance + latestIntake.CR - latestIntake.DR;
            return balance;
        }

        private void UpdateIntakeKgs(DTransport dTransport)
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var transExisting = _context.DTransports
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Active && i.producttype == dTransport.producttype && i.Startdate <= dTransport.DateInactivate && i.Sno == dTransport.Sno);
            if (transExisting.Any())
            {
                var selectassigntrans = _context.ProductIntake
                    .Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper()) && i.ProductType == dTransport.producttype && i.TransDate >= dTransport.DateInactivate && i.Sno == dTransport.Sno.ToString() && i.Qsupplied != 0);
                if (selectassigntrans.Any())
                {
                    _context.ProductIntake.RemoveRange(selectassigntrans);
                }
                foreach (var details in transExisting)
                {
                    details.Active = false;
                    details.DateInactivate = dTransport.DateInactivate;
                }
                _context.SaveChanges();
            }

        }
        // GET: DTransports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            //return Json(new { data = Farmersobj });

            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports.FindAsync(id);
            if (dTransport == null)
            {
                return NotFound();
            }
            TransSuppliersobj = new TransSuppliers
            {
                DTransport = await _context.DTransports.FindAsync(id),
                DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
                DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
            };
            GetInitialValues();
            TransSuppliersobj.DTransporter = TransSuppliersobj.DTransporter.Where(i => i.TransCode == dTransport.TransCode && i.ParentT.ToUpper().Equals(sacco.ToUpper()));
            TransSuppliersobj.DSuppliers = TransSuppliersobj.DSuppliers.Where(i => i.Sno == dTransport.Sno && i.Scode.ToUpper().Equals(sacco.ToUpper()));
            return View(TransSuppliersobj);
        }

        // POST: DTransports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,Sno,Rate,Startdate,Active,DateInactivate,Auditid,Auditdatetime,Isfrate,producttype")] DTransport dTransport)
        {
            utilities.SetUpPrivileges(this);
            if (id != dTransport.Id)
            {
                GetInitialValues();
                return NotFound();
            }
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            if(dTransport.Startdate > dTransport.DateInactivate)
            {
                _notyf.Error("Data InActivated should not be less than Date Assigned, Try Again");
                TransSuppliersobj = new TransSuppliers
                {
                    DTransport = new DTransport(),
                    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
                    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
                };
                GetInitialValues();
                return View(TransSuppliersobj);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dTransport.saccocode = sacco;
                    //dTransport.producttype = dTransport.producttype;
                    _context.Update(dTransport);
                    _notyf.Success("Edit saved successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransportExists(dTransport.Id))
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
            return View(dTransport);
        }

        // GET: DTransports/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransport = await _context.DTransports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransport == null)
            {
                return NotFound();
            }

            return View(dTransport);
        }

        // POST: DTransports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dTransport = await _context.DTransports.FindAsync(id);
            _context.DTransports.Remove(dTransport);
            await _context.SaveChangesAsync();
            _notyf.Error("Data Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DTransportExists(long id)
        {
            return _context.DTransports.Any(e => e.Id == id);
        }
    }
}
