﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using EasyPro.Utils;

namespace EasyPro.Controllers
{
    public class WardsController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;

        public WardsController(MORINGAContext context)
        {
            _context = context;
            utilities = new Utilities(context);
        }

        // GET: Wards
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.Ward.ToListAsync());
        }

        // GET: Wards/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Ward
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // GET: Wards/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        // POST: Wards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SubCounty,Contact,Closed,CreatedOn,CreatedBy")] Ward ward)
        {
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                ward.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                ward.CreatedOn = DateTime.Today;
                _context.Add(ward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ward);
        }

        // GET: Wards/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Ward.FindAsync(id);
            if (ward == null)
            {
                return NotFound();
            }
            return View(ward);
        }

        private void SetInitialValues()
        {
            var subCounties = _context.SubCounty.Where(c => !c.Closed).ToList();
            ViewBag.subCounties = new SelectList(subCounties, "Name", "Name");
        }

        // POST: Wards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,SubCounty,Contact,Closed,CreatedOn,CreatedBy")] Ward ward)
        {
            utilities.SetUpPrivileges(this);
            if (id != ward.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ward.CreatedBy = HttpContext.Session.GetString(StrValues.LoggedInUser);
                    ward.CreatedOn = DateTime.Today;
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardExists(ward.Id))
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
            return View(ward);
        }

        // GET: Wards/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Ward
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // POST: Wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var ward = await _context.Ward.FindAsync(id);
            _context.Ward.Remove(ward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardExists(long id)
        {
            return _context.Ward.Any(e => e.Id == id);
        }
    }
}
