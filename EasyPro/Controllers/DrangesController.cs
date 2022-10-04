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
    public class DrangesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public DrangesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Dranges
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.Dranges.Where(i => i.SaccoCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var deductionnames = _context.Deductions.Where(a => a.SaccoCode == sacco).ToList();
            ViewBag.deductionname = new SelectList(deductionnames, "Name", "Name");
        }
        // GET: Dranges/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drange = await _context.Dranges
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drange == null)
            {
                return NotFound();
            }

            return View(drange);
        }

        // GET: Dranges/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View();
        }

        // POST: Dranges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Deduction,From,To,Rate,Percentage,Audittime,Auditid,SaccoCode")] Drange drange)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var loginuser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            var dCodesExists = _context.Dranges.Any(i => i.Deduction.ToUpper().Equals(drange.Deduction.ToUpper())  && i.SaccoCode == sacco && i.From== drange.From && i.To== drange.To);
            if (dCodesExists)
            {
                _notyf.Error("Sorry, The Name already exist");
                GetInitialValues();
                return View(drange);
            }


            if (ModelState.IsValid)
            {
                drange.SaccoCode = sacco;
                drange.Auditid = loginuser;
                drange.Audittime = DateTime.Now;
                _context.Add(drange);
                await _context.SaveChangesAsync();
                _notyf.Success("Range saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(drange);
        }

        // GET: Dranges/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id == null)
            {
                return NotFound();
            }

            var drange = await _context.Dranges.FindAsync(id);
            if (drange == null)
            {
                return NotFound();
            }
            return View(drange);
        }

        // POST: Dranges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Deduction,From,To,Rate,Percentage,Audittime,Auditid,SaccoCode")] Drange drange)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != drange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var dCodesExists = _context.Dranges.Any(i => i.Deduction.ToUpper().Equals(drange.Deduction.ToUpper()) 
                    && i.SaccoCode == sacco && i.From == drange.From && i.To == drange.To && i.Id != drange.Id);
                    if (dCodesExists)
                    {
                        _notyf.Error("Sorry, The Name already exist");
                        GetInitialValues();
                        return View(drange);
                    }

                    _context.Update(drange);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Range Edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrangeExists(drange.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //_notyf.Error("Error Occured");
                return RedirectToAction(nameof(Index));
            }
            return View(drange);
        }

        // GET: Dranges/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id == null)
            {
                return NotFound();
            }

            var drange = await _context.Dranges
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drange == null)
            {
                return NotFound();
            }

            return View(drange);
        }

        // POST: Dranges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var drange = await _context.Dranges.FindAsync(id);
            _context.Dranges.Remove(drange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrangeExists(long id)
        {
            return _context.Dranges.Any(e => e.Id == id);
        }
    }
}
