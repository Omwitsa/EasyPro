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
            var today = DateTime.Now;
            var month = new DateTime(today.Year, today.Month, 1);
            var startdate = month;
            var enddate = startdate.AddMonths(1).AddDays(-1);
            //var startdate = month.AddMonths(-1).ToString("dd/MM/yyy");
            //var enddate = month.AddDays(-1).ToString("dd/MM/yyy");

            var transdeduction = await _context.DTransports.Where(i=>i.Active== true).ToListAsync();
            var intakes = new List<TransSuppliersVM>();
            foreach (var intake in transdeduction)
            {
                var trans = _context.DTransporters.FirstOrDefault(i => i.TransCode == intake.TransCode);
                var supplier = _context.DSuppliers.FirstOrDefault(i => i.Sno == intake.Sno);
                intakes.Add(new TransSuppliersVM
                {
                    Id = intake.Id,
                    TransCode = trans.TransCode,
                    TransName = trans.TransName,
                    Sno = supplier.Sno,
                    Names = supplier.Names,
                    Rate = intake.Rate,
                    Startdate = intake.Startdate,
                    DateInactivate = intake.DateInactivate
                });
            }
            dtransporterobj.TregDate = DateTime.Now;
            return View(intakes);
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
            var producttypes = _context.DPrices.Where(i => i.SaccoCode == sacco).Select(b => b.Products).ToList();
            ViewBag.producttypes = new SelectList(producttypes,"");
        }
        // GET: DTransports/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            TransSuppliersobj = new TransSuppliers
            {
                DTransport = new DTransport(),
                //DTransport = _context.DTransports,
                DTransporter = _context.DTransporters,
                DSuppliers = _context.DSuppliers,
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
            sacco = sacco ?? "";
            var dTransporterExists = _context.DTransports.Any(i => i.Sno == dTransport.Sno && i.Active==true && i.producttype== dTransport.producttype && i.saccocode == sacco);
            if (dTransporterExists)
            {
                GetInitialValues();
                _notyf.Error("Supplier has an active Assignment");
                return View();
            }
            if (ModelState.IsValid)
            {
                dTransport.saccocode = sacco;
                _context.Add(dTransport);
                await _context.SaveChangesAsync();
                _notyf.Success("Assignment saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dTransport);
        }

        // GET: DTransports/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
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
                //DTransport = _context.DTransports,
                DTransporter = _context.DTransporters,
                DSuppliers = _context.DSuppliers,
            };
            GetInitialValues();
            TransSuppliersobj.DTransporter = TransSuppliersobj.DTransporter.Where(i => i.TransCode == dTransport.TransCode);
            TransSuppliersobj.DSuppliers = TransSuppliersobj.DSuppliers.Where(i => i.Sno == dTransport.Sno);
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
