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
using EasyPro.ViewModels.UserViewModel;

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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            return View(await _context.UserAccounts
                .Where(u => u.Branchcode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: UserAccounts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            SetInitialValues();
            return View();
        }

        private void SetInitialValues()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
            var userGroups = _context.Usergroups.Where(g => g.SaccoCode == sacco).OrderBy(K => K.GroupName).ToList();
            var branches = _context.DBranch.OrderBy(K => K.Bname).ToList();
            var accounts = _context.Glsetups.OrderBy(K => K.GlAccName).ToList();
            var users = _context.UserAccounts.OrderBy(K => K.UserName).ToList();
            var saccos = _context.DCompanies.OrderBy(K=>K.Name).ToList();
            if (!loggedInUser.ToLower().Equals("psigei"))
            {
                
                branches = _context.DBranch.Where(p=>p.Bcode.ToUpper().Equals(sacco.ToUpper())).ToList();
                accounts = _context.Glsetups.Where(p => p.saccocode.ToUpper().Equals(sacco.ToUpper())).ToList();
                saccos = _context.DCompanies.Where(p => p.Name.ToUpper().Equals(sacco.ToUpper())).ToList();
                users = _context.UserAccounts.Where(p => p.Branchcode.ToUpper().Equals(sacco.ToUpper())).ToList();
            }

            ViewBag.userGroups = new SelectList(userGroups, "GroupName", "GroupName");
            ViewBag.branches = new SelectList(branches, "Bname", "Bname");
            ViewBag.dBranches = branches;
            ViewBag.accounts = new SelectList(accounts, "AccNo", "GlAccName"); 
            ViewBag.saccos = new SelectList(saccos, "Name", "Name");
            ViewBag.users = new SelectList(saccos, "UserName", "UserName");
        }

        // POST: UserAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,UserName,UserLoginIds,Password,UserGroup,PassExpire,DateCreated,Superuser,AssignGl,Branchcode,Levels,Authorize,Status,Branch,Sign,Phone,AccessLevel")] UserAccount userAccount)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
                
                _context.Add(userAccount);
                await _context.SaveChangesAsync();

               
                _notyf.Success("User created successfully");
               
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            ViewBag.password = Decryptor.Decript_String(userAccount.Password);
            return View(userAccount);
        }

        // GET: UserAccounts/change branch/
        public async Task<IActionResult> ChangeBranch(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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

            var viewuser = new ChangeBranchVM
            {
                Id = userAccount.Userid,
                user = userAccount.UserName,
            };

            return View(viewuser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeBranch(long id, [Bind("Id,user,Branch")] ChangeBranchVM changeBranchVM)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != changeBranchVM.Id)
            {
                _notyf.Error("Sorry, User not found");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // UserName, UserLoginIDs, Password, UserGroup, PassExpire, DateCreated, SUPERUSER, AssignGl,
                    // branchcode, levels, Authorize, Status, Branch, sign, Phone, Reset, AccessLevel
                    var olddetails = _context.UserAccounts.FirstOrDefault(i=>i.Userid==id);
                    olddetails.Branch = changeBranchVM.Branch;
                    //_context.UserAccounts.Update(
                    //{
                    //    UserLoginIds= olddetails.UserLoginIds,
                    //    UserName=olddetails.UserName,
                    //    Password= olddetails.Password,
                    //    UserGroup=olddetails.UserGroup,
                    //    PassExpire = olddetails.PassExpire,
                    //    DateCreated= olddetails.DateCreated,
                    //    Superuser = olddetails.Superuser,
                    //    AssignGl= olddetails.AssignGl,
                    //    Branchcode = olddetails.Branchcode,
                    //    Levels = olddetails.Levels,
                    //    Authorize = olddetails.Authorize,
                    //    Status = olddetails.Status,
                    //    Branch = changeBranchVM.Branch,
                    //    Sign = olddetails.Sign,
                    //    Phone = olddetails.Phone,
                    //    Reset = olddetails.Reset,
                    //    AccessLevel = olddetails.AccessLevel,
                    //});
                    _context.Update(olddetails);
                    await _context.SaveChangesAsync();
                    _notyf.Success("User Transfered successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _notyf.Error("Sorry, An error occurred");
                    if (!UserAccountExists(changeBranchVM.Id))
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
            return View(changeBranchVM);
        }

        // POST: UserAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Userid,UserName,UserLoginIds,Password,UserGroup,PassExpire,DateCreated,Superuser,AssignGl,Branchcode,Levels,Authorize,Status,Branch,Sign,Phone,AccessLevel")] UserAccount userAccount)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
                    userAccount.Password = Decryptor.Decript_String(userAccount.Password);
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            return View(await _context.UserAccounts
                .Where(u => (bool)u.Reset && u.Branchcode == sacco).ToListAsync());
        }

        public async Task<IActionResult> ResetPassword(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
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
