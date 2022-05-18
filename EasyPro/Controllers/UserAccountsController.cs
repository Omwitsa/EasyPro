using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;

        public UserAccountsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: UserAccounts
        public async Task<IActionResult> Index()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            return View(await _context.UserAccounts
                .Where(u => u.Branchcode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: UserAccounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // GET: UserAccounts/Create
        public IActionResult Create()
        {
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var userGroups = _context.Usergroups.ToList();
            ViewBag.userGroups = new SelectList(userGroups, "GroupId", "GroupName");
            var branches = _context.DBranch.ToList();
            ViewBag.branches = new SelectList(branches, "Bname", "Bname");
            ViewBag.dBranches = branches;
            var accounts = _context.Glsetups.ToList();
            ViewBag.accounts = new SelectList(accounts, "AccNo", "GlAccName"); 
            var saccos = _context.DCompanies.ToList();
            ViewBag.saccos = new SelectList(saccos, "Name", "Name");
        }

        // POST: UserAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,UserName,UserLoginIds,Password,UserGroup,PassExpire,DateCreated,Superuser,AssignGl,Branchcode,Levels,Authorize,Status,Branch,Sign,Phone")] UserAccount userAccount)
        {
            try
            {
                userAccount.Branchcode = userAccount?.Branchcode ?? "";
                userAccount.UserLoginIds = userAccount?.UserLoginIds ?? "";
                userAccount.DateCreated = DateTime.UtcNow.AddHours(3);
                userAccount.Password = Decryptor.Decript_String(userAccount.Password);
                if (_context.UserAccounts.Any(u => u.UserLoginIds.ToUpper().Equals(userAccount.UserLoginIds.ToUpper())))
                {
                    _notyf.Error("Sorry, UserName already exist");
                    return View(userAccount);
                }

                _notyf.Success("User created successfully");
                _context.Add(userAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(userAccount);
            }
        }

        // GET: UserAccounts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            SetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            return View(userAccount);
        }

        // POST: UserAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Userid,UserName,UserLoginIds,Password,UserGroup,PassExpire,DateCreated,Superuser,AssignGl,Branchcode,Levels,Authorize,Status,Branch,Sign,Phone")] UserAccount userAccount)
        {
            if (id != userAccount.Userid)
            {
                _notyf.Error("Sorry, User not found");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _notyf.Success("User saved successfully");
                    userAccount.Branchcode = userAccount?.Branchcode ?? "";
                    _context.Update(userAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    _notyf.Error("Sorry, An error occurred");
                    if (!UserAccountExists(userAccount.Userid))
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
            return View(userAccount);
        }

        // GET: UserAccounts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        // POST: UserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userAccount = await _context.UserAccounts.FindAsync(id);
            _context.UserAccounts.Remove(userAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(long id)
        {
            return _context.UserAccounts.Any(e => e.Userid == id);
        }
    }
}
