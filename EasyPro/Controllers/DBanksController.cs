using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using EasyPro.Utils;

namespace EasyPro.Controllers
{
    public class DBanksController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DBanksController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DBanks
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sacco = sacco ?? "";
            return View(await _context.DBanks
                .Where(i => i.BankCode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DBanks/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBank = await _context.DBanks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBank == null)
            {
                return NotFound();
            }

            return View(dBank);
        }

        // GET: DBanks/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DBanks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BankCode,BankName,BranchName,Address,Telephone,AuditId,AuditTime,BankAccNo,Accno,AccType")] DBank dBank)
        {
            utilities.SetUpPrivileges(this);
            var dSupplier1 = _context.DBanks.Where(i => i.BankName == dBank.BankName).Count();
            if (dSupplier1 != 0)
            {
                _notyf.Error("Sorry, The Bank Name already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                sacco = sacco ?? "";
                dBank.BankCode = sacco;
                _context.Add(dBank);
                await _context.SaveChangesAsync();
                _notyf.Success("Bank saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dBank);
        }

        // GET: DBanks/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBank = await _context.DBanks.FindAsync(id);
            if (dBank == null)
            {
                return NotFound();
            }
            return View(dBank);
        }

        // POST: DBanks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,BankCode,BankName,BranchName,Address,Telephone,AuditId,AuditTime,BankAccNo,Accno,AccType")] DBank dBank)
        {
            utilities.SetUpPrivileges(this);
            if (id != dBank.Id)
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
                    dBank.BankCode = sacco;
                    _context.Update(dBank);
                    _notyf.Success("Bank Edited successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBankExists(dBank.Id))
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
            return View(dBank);
        }

        // GET: DBanks/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBank = await _context.DBanks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBank == null)
            {
                return NotFound();
            }

            return View(dBank);
        }

        // POST: DBanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dBank = await _context.DBanks.FindAsync(id);
            _context.DBanks.Remove(dBank);
            await _context.SaveChangesAsync();
            _notyf.Success("Bank Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DBankExists(long id)
        {
            return _context.DBanks.Any(e => e.Id == id);
        }
    }
}
