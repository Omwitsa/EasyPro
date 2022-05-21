using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;

namespace EasyPro.Controllers
{
    public class GlsetupsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public GlsetupsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: Glsetups
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            return View(await _context.Glsetups.ToListAsync());
        }

        // GET: Glsetups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var glsetup = await _context.Glsetups
                .FirstOrDefaultAsync(m => m.AccNo == id);
            if (glsetup == null)
            {
                return NotFound();
            }

            return View(glsetup);
        }

        // GET: Glsetups/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: Glsetups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Glid,GlCode,GlAccName,AccNo,GlAccType,GlAccGroup,GlAccMainGroup,NormalBal,GlAccStatus,OpeningBal,CurrentBal,Bal,CurrCode,AuditOrg,AuditId,AuditDate,Curr,Actuals,Budgetted,TransDate,IsSubLedger,AccCategory,NewGlopeningBal,NewGlopeningBalDate,Branch,Hcode,Mcode,Hname,Header,Mheader,Iorder,Border,Type,Subtype,IsRearning,Issuspense,Run")] Glsetup glsetup)
        {
            utilities.SetUpPrivileges(this);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(glsetup.AccNo))
                {
                    _notyf.Error("Sorry, Kindly provide account No.");
                    return View(glsetup);
                }
                if (string.IsNullOrEmpty(glsetup.GlAccName))
                {
                    _notyf.Error("Sorry, Kindly provide account Name");
                    return View(glsetup);
                }
                if (_context.Glsetups.Any(u => u.AccNo.ToUpper().Equals(glsetup.AccNo.ToUpper())))
                {
                    _notyf.Error("Sorry, Account No. already exist");
                    return View(glsetup);
                }
                if (_context.Glsetups.Any(u => u.GlAccName.ToUpper().Equals(glsetup.GlAccName.ToUpper())))
                {
                    _notyf.Error("Sorry, Account Name already exist");
                    return View(glsetup);
                }
                _context.Add(glsetup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(glsetup);
        }

        // GET: Glsetups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var glsetup = await _context.Glsetups.FindAsync(id);
            if (glsetup == null)
            {
                return NotFound();
            }
            return View(glsetup);
        }

        // POST: Glsetups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Glid,GlCode,GlAccName,AccNo,GlAccType,GlAccGroup,GlAccMainGroup,NormalBal,GlAccStatus,OpeningBal,CurrentBal,Bal,CurrCode,AuditOrg,AuditId,AuditDate,Curr,Actuals,Budgetted,TransDate,IsSubLedger,AccCategory,NewGlopeningBal,NewGlopeningBalDate,Branch,Hcode,Mcode,Hname,Header,Mheader,Iorder,Border,Type,Subtype,IsRearning,Issuspense,Run")] Glsetup glsetup)
        {
            utilities.SetUpPrivileges(this);
            if (id != glsetup.AccNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(glsetup.AccNo))
                    {
                        _notyf.Error("Sorry, Kindly provide account No.");
                        return View(glsetup);
                    }
                    if (string.IsNullOrEmpty(glsetup.GlAccName))
                    {
                        _notyf.Error("Sorry, Kindly provide account Name");
                        return View(glsetup);
                    }
                    if (_context.Glsetups.Any(u => u.AccNo.ToUpper().Equals(glsetup.AccNo.ToUpper()) && u.Glid != glsetup.Glid))
                    {
                        _notyf.Error("Sorry, Account No. already exist");
                        return View(glsetup);
                    }
                    if (_context.Glsetups.Any(u => u.GlAccName.ToUpper().Equals(glsetup.GlAccName.ToUpper()) && u.Glid == glsetup.Glid))
                    {
                        _notyf.Error("Sorry, Account Name already exist");
                        return View(glsetup);
                    }
                    _context.Update(glsetup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlsetupExists(glsetup.AccNo))
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
            return View(glsetup);
        }

        // GET: Glsetups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var glsetup = await _context.Glsetups
                .FirstOrDefaultAsync(m => m.AccNo == id);
            if (glsetup == null)
            {
                return NotFound();
            }

            return View(glsetup);
        }

        // POST: Glsetups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            utilities.SetUpPrivileges(this);
            var glsetup = await _context.Glsetups.FindAsync(id);
            _context.Glsetups.Remove(glsetup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GlsetupExists(string id)
        {
            return _context.Glsetups.Any(e => e.AccNo == id);
        }
    }
}
