using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class UsergroupsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public UsergroupsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Usergroups
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.Usergroups.Where(g => g.SaccoCode == sacco).ToListAsync());
        }

        // GET: Usergroups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: Usergroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,GroupName,Registration,Activity,Activity,Reports,Setup,Flmd,Files,Accounts,Deductions,Staff,Store,SaccoReports")] Usergroup usergroup)
        {
            utilities.SetUpPrivileges(this);
            try
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (string.IsNullOrEmpty(usergroup.GroupId))
                {
                    _notyf.Error("Sorry, Kindly provide group code");
                    return View(usergroup);
                }
                if (string.IsNullOrEmpty(usergroup.GroupName))
                {
                    _notyf.Error("Sorry, Kindly provide group name");
                    return View(usergroup);
                }
                if (_context.Usergroups.Any(g => g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper()) && g.SaccoCode.ToUpper().Equals(sacco.ToUpper())))
                {
                    _notyf.Error("Sorry, Group code already exist");
                    return View(usergroup);
                }
                if (_context.Usergroups.Any(g => g.GroupName.ToUpper().Equals(usergroup.GroupName.ToUpper()) && g.SaccoCode.ToUpper().Equals(sacco.ToUpper())))
                {
                    _notyf.Error("Sorry, Group name already exist");
                    return View(usergroup);
                }
               
                usergroup.SaccoCode = sacco;
                usergroup.Registration = usergroup?.Registration ?? false;
                usergroup.Activity = usergroup?.Activity ?? false;
                usergroup.Reports = usergroup?.Reports ?? false;
                usergroup.Setup = usergroup?.Setup ?? false;
                usergroup.Files = usergroup?.Files ?? false;
                usergroup.Accounts = usergroup?.Accounts ?? false;
                usergroup.Deductions = usergroup?.Deductions ?? false;
                usergroup.Staff = usergroup?.Staff ?? false;
                usergroup.Store = usergroup?.Store ?? false;
                usergroup.Flmd = usergroup?.Flmd ?? false;
                usergroup.SaccoReports = usergroup?.SaccoReports ?? false;

                _context.Add(usergroup);
                await _context.SaveChangesAsync();
                _notyf.Success("Group saved successfuly");
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
            utilities.SetUpPrivileges(this);
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
        public async Task<IActionResult> Edit(string id, [Bind("GroupId,GroupName,Registration,Activity,Activity,Reports,Setup,Files,Accounts,Flmd,Deductions,Staff,Store,SaccoReports")] Usergroup usergroup)
        {
            utilities.SetUpPrivileges(this);
            if (id != usergroup.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(usergroup.GroupId))
                    {
                        _notyf.Error("Sorry, Kindly provide group code");
                        return View(usergroup);
                    }
                    if (string.IsNullOrEmpty(usergroup.GroupName))
                    {
                        _notyf.Error("Sorry, Kindly provide group name");
                        return View(usergroup);
                    }
                    var codeExist = _context.Usergroups.Any(g => g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper())
                    && !g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper()));
                    if (codeExist)
                    {
                        _notyf.Error("Sorry, Group code already exist");
                        return View(usergroup);
                    }
                    var nameExist = _context.Usergroups.Any(g => g.GroupName.ToUpper().Equals(usergroup.GroupName.ToUpper())
                    && !g.GroupId.ToUpper().Equals(usergroup.GroupId.ToUpper()));
                    if (nameExist)
                    {
                        _notyf.Error("Sorry, Group name already exist");
                        return View(usergroup);
                    }
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                    usergroup.SaccoCode = sacco;
                    usergroup.Registration = usergroup?.Registration ?? false;
                    usergroup.Activity = usergroup?.Activity ?? false;
                    usergroup.Reports = usergroup?.Reports ?? false;
                    usergroup.Setup = usergroup?.Setup ?? false;
                    usergroup.Files = usergroup?.Files ?? false;
                    usergroup.Accounts = usergroup?.Accounts ?? false;
                    usergroup.Deductions = usergroup?.Deductions ?? false;
                    usergroup.Staff = usergroup?.Staff ?? false;
                    usergroup.Store = usergroup?.Store ?? false;
                    usergroup.Flmd = usergroup?.Flmd ?? false;
                    usergroup.SaccoReports = usergroup?.SaccoReports ?? false;
                    _context.Update(usergroup);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Group edited successfully");
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
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
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
