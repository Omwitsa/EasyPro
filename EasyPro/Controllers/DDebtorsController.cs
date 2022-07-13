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

namespace EasyPro.Controllers
{
    public class DDebtorsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DDebtorsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DDebtors
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.DDebtors
                .Where(i => i.Dcode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DDebtors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dDebtor = await _context.DDebtors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dDebtor == null)
            {
                return NotFound();
            }

            return View(dDebtor);
        }

        // GET: DDebtors/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DDebtors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dcode,Dname,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Price,AccDr,AccCr,Crate,Drcess,Crcess,Capp")] DDebtor dDebtor)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var locations = _context.DDebtors.Any(i => i.Dname == dDebtor.Dname && i.Dcode == sacco);
            if (locations)
            {
                _notyf.Error("Sorry, The Debtor Name already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                dDebtor.Dcode = sacco;
                dDebtor.Active = true;
                dDebtor.Auditdatetime = DateTime.Now;
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
                dDebtor.Auditid = loggedInUser;
                _context.Add(dDebtor);
                _notyf.Success("Debtor saved successfully");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dDebtor);
        }

        // GET: DDebtors/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dDebtor = await _context.DDebtors.FindAsync(id);
            if (dDebtor == null)
            {
                return NotFound();
            }
            return View(dDebtor);
        }

        // POST: DDebtors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Dcode,Dname,CertNo,Locations,TregDate,Email,Phoneno,Town,Address,Subsidy,Accno,Bcode,Bbranch,Active,Tbranch,Auditid,Auditdatetime,Price,AccDr,AccCr,Crate,Drcess,Crcess,Capp")] DDebtor dDebtor)
        {
            utilities.SetUpPrivileges(this);
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != dDebtor.Id)
            {
                return NotFound();
                
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dDebtor.Dcode = sacco;
                    dDebtor.Active = true;
                    dDebtor.Auditdatetime = DateTime.Now;
                    var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    dDebtor.Auditid = loggedInUser;
                    _context.Add(dDebtor);
                    _context.Update(dDebtor);
                    _notyf.Success("Debtor Edited successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDebtorExists(dDebtor.Id))
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
            return View(dDebtor);
        }

        // GET: DDebtors/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dDebtor = await _context.DDebtors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dDebtor == null)
            {
                return NotFound();
            }

            return View(dDebtor);
        }

        // POST: DDebtors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dDebtor = await _context.DDebtors.FindAsync(id);
            _context.DDebtors.Remove(dDebtor);
            await _context.SaveChangesAsync();
            _notyf.Error("Debtor Delete successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DDebtorExists(long id)
        {
            utilities.SetUpPrivileges(this);
            return _context.DDebtors.Any(e => e.Id == id);
        }
    }
}
