using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using EasyPro.Utils;

namespace EasyPro.Controllers
{
    public class DBankBranchesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DBankBranchesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DBankBranches
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            return View(await _context.DBankBranch
                .Where(i => i.BankCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DBankBranches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBankBranch = await _context.DBankBranch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBankBranch == null)
            {
                return NotFound();
            }

            return View(dBankBranch);
        }

        // GET: DBankBranches/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DBankBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BankCode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBankBranch dBankBranch)
        {
            utilities.SetUpPrivileges(this);
            var dSupplier1 = _context.DBankBranch.Where(i => i.Bname == dBankBranch.Bname && i.BankCode == dBankBranch.BankCode).Count();
            if (dSupplier1 != 0)
            {
                _notyf.Error("Sorry, The Bank Branch Name already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                sacco = sacco ?? "";
                dBankBranch.BankCode = sacco;
                _context.Add(dBankBranch);
                await _context.SaveChangesAsync();
                _notyf.Success("Bank Branch saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dBankBranch);
        }

        // GET: DBankBranches/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBankBranch = await _context.DBankBranch.FindAsync(id);
            if (dBankBranch == null)
            {
                return NotFound();
            }
            return View(dBankBranch);
        }

        // POST: DBankBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,BankCode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBankBranch dBankBranch)
        {
            utilities.SetUpPrivileges(this);
            if (id != dBankBranch.Id)
            {
                _notyf.Error("Sorry, an error occured while eidting");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                    sacco = sacco ?? "";
                    dBankBranch.BankCode = sacco;
                    _context.Update(dBankBranch);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Bank Branch Edited successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBankBranchExists(dBankBranch.Id))
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
            return View(dBankBranch);
        }

        // GET: DBankBranches/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBankBranch = await _context.DBankBranch
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBankBranch == null)
            {
                return NotFound();
            }

            return View(dBankBranch);
        }

        // POST: DBankBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dBankBranch = await _context.DBankBranch.FindAsync(id);
            _context.DBankBranch.Remove(dBankBranch);
            await _context.SaveChangesAsync();
            _notyf.Success("Bank Branch Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DBankBranchExists(long id)
        {
            return _context.DBankBranch.Any(e => e.Id == id);
        }
    }
}
