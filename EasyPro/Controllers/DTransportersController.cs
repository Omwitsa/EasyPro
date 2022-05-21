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

namespace EasyPro.Controllers
{
    public class DTransportersController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DTransportersController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DTransporters
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.DTransporters.Where(i => i.Active == true || i.Active == false).ToListAsync());
        }

        // GET: DTransporters/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransporter = await _context.DTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransporter == null)
            {
                return NotFound();
            }

            return View(dTransporter);
        }

        // GET: DTransporters/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }
        private void GetInitialValues()
        {
            var banksname = _context.DBanks.Select(b => b.BankName).ToList();
            ViewBag.banksname = new SelectList(banksname);

            var brances = _context.DBranch.Select(b => b.Bname).ToList();
            ViewBag.brances = new SelectList(brances);

            var bankbrances = _context.DBankBranch.Select(b => b.Bname).ToList();
            ViewBag.bankbrances = new SelectList(bankbrances);

            List<SelectListItem> gender = new()
            {
                new SelectListItem { Text = "Male" },
                new SelectListItem {Text = "Female" },
            };
            ViewBag.gender = gender;
            List<SelectListItem> payment = new()
            {
                new SelectListItem {  Text = "Weekly" },
                new SelectListItem { Text = "Monthly" },
            };
            ViewBag.payment = payment;
        }
        // POST: DTransporters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransCode,TransName,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Isfrate,Rate,Canno,Tt,ParentT,Ttrate,Br,Freezed,PaymenMode")] DTransporter dTransporter)
        {
            utilities.SetUpPrivileges(this);
            if (dTransporter == null)
            {
                _notyf.Error("Transporter cannot be empty");
                return NotFound();
            }
            var dTransporter10 = _context.DTransporters.Where(i=>i.TransCode == dTransporter.TransCode && i.CertNo == dTransporter.CertNo).Count();
            if (dTransporter10 != 0)
            {
                GetInitialValues();
                _notyf.Error("Transporter entered already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                _context.Add(dTransporter);
                await _context.SaveChangesAsync();
                _notyf.Success("Transporter saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dTransporter);
        }

        // GET: DTransporters/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }
            GetInitialValues();
            var dTransporter = await _context.DTransporters.FindAsync(id);
            if (dTransporter == null)
            {
                return NotFound();
            }
            return View(dTransporter);
        }

        // POST: DTransporters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TransCode,TransName,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Isfrate,Rate,Canno,Tt,ParentT,Ttrate,Br,Freezed,PaymenMode")] DTransporter dTransporter)
        {
            utilities.SetUpPrivileges(this);
            if (id != dTransporter.Id)
            {
                _notyf.Error("Sorry, error occured while editing, try again");
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    dTransporter.TransCode = dTransporter.TransCode;
                    dTransporter.Br = "A";
                    dTransporter.Freezed = "0";
                    _context.Update(dTransporter);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Transporter Edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DTransporterExists(dTransporter.Id))
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
            return View(dTransporter);
        }

        // GET: DTransporters/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dTransporter = await _context.DTransporters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dTransporter == null)
            {
                return NotFound();
            }

            return View(dTransporter);
        }

        // POST: DTransporters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long itemId)
        {
            utilities.SetUpPrivileges(this);
            var dTransporter = await _context.DTransporters.FindAsync(itemId);
            _context.DTransporters.Remove(dTransporter);
            await _context.SaveChangesAsync();
            _notyf.Success("Transporter Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DTransporterExists(long id)
        {
            return _context.DTransporters.Any(e => e.Id == id);
        }
    }
}
