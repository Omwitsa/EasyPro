using System;
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
        private Utilities utilities;

        public UserAccountsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: UserAccounts
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            return View(await _context.UserAccounts
                .Where(u => u.Branchcode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: UserAccounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var userGroups = _context.Usergroups.Where(g => g.SaccoCode == sacco).ToList();
            var branches = _context.DBranch.ToList();
            var accounts = _context.Glsetups.ToList();
            var saccos = _context.DCompanies.ToList();
            if (!loggedInUser.ToLower().Equals("psigei"))
            {
                
                branches = _context.DBranch.Where(p=>p.Bcode.ToUpper().Equals(sacco.ToUpper())).ToList();
                accounts = _context.Glsetups.Where(p => p.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
                saccos = _context.DCompanies.Where(p => p.Name.ToUpper().Equals(sacco.ToUpper())).ToList();
            }

            ViewBag.userGroups = new SelectList(userGroups, "GroupName", "GroupName");
            ViewBag.branches = new SelectList(branches, "Bname", "Bname");
            ViewBag.dBranches = branches;
            ViewBag.accounts = new SelectList(accounts, "AccNo", "GlAccName"); 
            ViewBag.saccos = new SelectList(saccos, "Name", "Name");
        }

        // POST: UserAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,UserName,UserLoginIds,Password,UserGroup,PassExpire,DateCreated,Superuser,AssignGl,Branchcode,Levels,Authorize,Status,Branch,Sign,Phone")] UserAccount userAccount)
        {
            utilities.SetUpPrivileges(this);
            try
            {
                if (userAccount.Branch == "")
                {
                     SetInitialValues();
                    _notyf.Error("Select the branch");
                    return View();
                }

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
            catch (Exception ex)
            {
                _notyf.Error("Sorry, An error occurred");
                return View(userAccount);
            }
        }

        // GET: UserAccounts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
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
            utilities.SetUpPrivileges(this);
            var userAccount = await _context.UserAccounts.FindAsync(id);
            _context.UserAccounts.Remove(userAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(long id)
        {
            return _context.UserAccounts.Any(e => e.Userid == id);
        }

        public async Task<IActionResult> ResetPasswordList()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.UserAccounts
                .Where(u => (bool)u.Reset).ToListAsync());
        }

        public async Task<IActionResult> ResetPassword(long? id)
        {
            utilities.SetUpPrivileges(this);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(long id, [Bind("Userid,UserName,UserLoginIds,Password,UserGroup,PassExpire,DateCreated,Superuser,AssignGl,Branchcode,Levels,Authorize,Status,Branch,Sign,Phone")] UserAccount userAccount)
        {
            utilities.SetUpPrivileges(this);
            if (id != userAccount.Userid)
            {
                _notyf.Error("Sorry, User not found");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _notyf.Success("Password reset successfully");
                    var savedUser = _context.UserAccounts.FirstOrDefault(u => u.Userid == id);
                    savedUser.Reset = false;
                    savedUser.Password = Decryptor.Decript_String(userAccount.Password);
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
                return RedirectToAction(nameof(ResetPasswordList));
            }
            _notyf.Error("Sorry, An error occurred");
            return View(userAccount);
        }
    }
}
