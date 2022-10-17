using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;

namespace EasyPro.Controllers
{
    public class DLocationsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DLocationsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DLocations
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.DLocations
                .Where(i => i.Lcode.ToUpper().Equals(sacco.ToUpper()) && i.Branch == saccoBranch).ToListAsync());
        }

        // GET: DLocations/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dLocation = await _context.DLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dLocation == null)
            {
                return NotFound();
            }

            return View(dLocation);
        }

        // GET: DLocations/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Lcode,Lname,AuditId,Auditdatetime")] DLocation dLocation)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccoBranch = HttpContext.Session.GetString(StrValues.Branch);
            var locations = _context.DLocations.Where(i => i.Lname == dLocation.Lname && i.Lcode == sacco && i.Branch == saccoBranch).Count();
            if (locations != 0)
            {
                _notyf.Error("Sorry, The Location Name already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                dLocation.Lcode = sacco;
                dLocation.Branch = saccoBranch;
                _context.Add(dLocation);
                await _context.SaveChangesAsync();
                _notyf.Success("Location saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dLocation);
        }

        // GET: DLocations/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dLocation = await _context.DLocations.FindAsync(id);
            if (dLocation == null)
            {
                return NotFound();
            }
            return View(dLocation);
        }

        // POST: DLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Lcode,Lname,AuditId,Auditdatetime")] DLocation dLocation)
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != dLocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dLocation.Lcode = sacco;
                    _context.Update(dLocation);
                    _notyf.Success("Location Edited successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DLocationExists(dLocation.Id))
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
            return View(dLocation);
        }

        // GET: DLocations/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dLocation = await _context.DLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dLocation == null)
            {
                return NotFound();
            }

            return View(dLocation);
        }

        // POST: DLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dLocation = await _context.DLocations.FindAsync(id);
            _context.DLocations.Remove(dLocation);
            await _context.SaveChangesAsync();
            _notyf.Error("Location Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DLocationExists(long id)
        {
            utilities.SetUpPrivileges(this);
            return _context.DLocations.Any(e => e.Id == id);
        }
    }
}
