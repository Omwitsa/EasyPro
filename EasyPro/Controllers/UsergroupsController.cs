using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class UsergroupsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;

        public UsergroupsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Usergroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usergroups.ToListAsync());
        }

        // GET: Usergroups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usergroup = await _context.Usergroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (usergroup == null)
            {
                return NotFound();
            }

            return View(usergroup);
        }

        // GET: Usergroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usergroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,GroupName,CashBook,Transactions,Activity,Reports,Setup,Files,Accounts,AccountsPay,FixedAssets")] Usergroup usergroup)
        {
            try
            {
                _notyf.Success("Group saved successfuly");
                _context.Add(usergroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(usergroup);
            }
        }

        // GET: Usergroups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usergroup = await _context.Usergroups.FindAsync(id);
            if (usergroup == null)
            {
                return NotFound();
            }
            return View(usergroup);
        }

        // POST: Usergroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("GroupId,GroupName,CashBook,Transactions,Activity,Reports,Setup,Files,Accounts,AccountsPay,FixedAssets")] Usergroup usergroup)
        {
            if (id != usergroup.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _notyf.Success("Group edited successfully");
                    _context.Update(usergroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    _notyf.Error("Sorry, An error occurred");
                    if (!UsergroupExists(usergroup.GroupId))
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
            _notyf.Error("Sorry, An error occurred");
            return View(usergroup);
        }

        // GET: Usergroups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usergroup = await _context.Usergroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (usergroup == null)
            {
                return NotFound();
            }

            return View(usergroup);
        }

        // POST: Usergroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usergroup = await _context.Usergroups.FindAsync(id);
            _context.Usergroups.Remove(usergroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsergroupExists(string id)
        {
            return _context.Usergroups.Any(e => e.GroupId == id);
        }
    }
}
