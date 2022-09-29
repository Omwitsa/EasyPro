using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using EasyPro.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace EasyPro.Controllers
{
    public class DDcodesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DDcodesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DDcodes
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.DDcodes
                .Where(i => i.Dcode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DDcodes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dDcode = await _context.DDcodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dDcode == null)
            {
                return NotFound();
            }

            return View(dDcode);
        }

        // GET: DDcodes/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }

        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = _context.Glsetups.Where(a => a.saccocode == sacco).ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");
        }

        // POST: DDcodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dcode,Description,Dedaccno,Contraacc,Auditid,Auditdatetime")] DDcode dDcode)
        {
            utilities.SetUpPrivileges(this);
            var dCodesExists= _context.DDcodes.Any(i => i.Description == dDcode.Description && i.Dcode== dDcode.Dcode);
            if (dCodesExists)
            {
                _notyf.Error("Sorry, The Branch Name already exist");
                return View();
            }

            if (ModelState.IsValid)
            {
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
                sacco = sacco ?? "";
                dDcode.Dcode = sacco;
                _context.Add(dDcode);
                await _context.SaveChangesAsync();
                _notyf.Success("Deduction saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(dDcode);
        }

        // GET: DDcodes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }
            
            var dDcode = await _context.DDcodes.FindAsync(id);
            if (dDcode == null)
            {
                return NotFound();
            }
            return View(dDcode);
        }

        // POST: DDcodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Dcode,Description,Dedaccno,Contraacc,Auditid,Auditdatetime")] DDcode dDcode)
        {
            utilities.SetUpPrivileges(this);
            if (id != dDcode.Id)
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
                    dDcode.Dcode = sacco;
                    _context.Update(dDcode);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Deduction saved successfully");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DDcodeExists(dDcode.Id))
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
            return View(dDcode);
        }

        // GET: DDcodes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dDcode = await _context.DDcodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dDcode == null)
            {
                return NotFound();
            }

            return View(dDcode);
        }

        // POST: DDcodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            var dDcode = await _context.DDcodes.FindAsync(id);
            _context.DDcodes.Remove(dDcode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DDcodeExists(long id)
        {
            return _context.DDcodes.Any(e => e.Id == id);
        }
    }
}
