using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using EasyPro.Utils;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class DPreSetController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public DPreSetController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }

        // GET: DPreSets
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.d_PreSets.Where(c=>c.saccocode== sacco && c.Stopped==false).ToListAsync());
        }
        //
        public async Task<IActionResult> UnaprovedIndex()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.d_PreSets.Where(c => c.saccocode == sacco && c.Stopped == true).ToListAsync());
        }

        public async Task<IActionResult> ResumeStop(long id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var dPreSet = await _context.d_PreSets.FindAsync(id);
            if (id != dPreSet.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    dPreSet.Stopped = false;
                    _context.Update(dPreSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPreSetExists(dPreSet.Id))
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
            return View(dPreSet);

        }


        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var glAccounts = _context.Glsetups.Where(a => a.saccocode == sacco).ToList();
            ViewBag.glAccounts = new SelectList(glAccounts, "AccNo", "GlAccName");

            var dDcodes = _context.DDcodes.Where(a => a.Dcode == sacco).Select(m=>m.Description).ToList();
            ViewBag.Description = dDcodes;
        }

        [HttpPost]
        public JsonResult Save([Bind("Id,Sno,Deduction,Remark,StartDate,Rate,Stopped,Auditdatetime,AuditId,Rated,Status,Status2,Status3,Status4,Status5,Status6,saccocode,BranchCode")] DPreSet dPreSet, bool isspecific, bool isallsup, bool isRate,bool isFixed)
        {
            try
            {
                DateTime Now = DateTime.Today;
                DateTime startD = new DateTime(Now.Year, Now.Month, 1);
                DateTime enDate = startD.AddMonths(1).AddDays(-1);


                if (!isspecific && !isallsup)
                {
                    _notyf.Error("Sorry, Kindly select Any");
                    return Json("");
                }


                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch);
                var Sno = dPreSet.Sno;
                Boolean Rated = true;
                if (isFixed)
                    Rated = false;

                var suppliers = _context.DSuppliers.Where(h=>h.Scode==sacco).ToList();
                if (isspecific)
                    suppliers = (List<DSupplier>)suppliers.Where(s => s.Sno.ToUpper().Equals(Sno.ToUpper()));
                suppliers.ForEach(t =>
                {
                    _context.d_PreSets.Add(new DPreSet
                    {
                        Sno = t.Sno,
                        Deduction = dPreSet.Deduction,
                        Remark = dPreSet.Remark,
                        StartDate = dPreSet.StartDate,
                        Rate = dPreSet.Rate,
                        Stopped = false,
                        Auditdatetime = DateTime.Now,
                        AuditId = loggedInUser,
                        Rated = Rated,
                        saccocode = sacco,
                        BranchCode= saccobranch,
                    });
                });

                _context.SaveChanges();
                _notyf.Success("Saved successfully");

                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        // GET: DPreSets/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var dPreSet = await _context.d_PreSets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dPreSet == null)
            {
                return NotFound();
            }

            return View(dPreSet);
        }

        // GET: DPreSets/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            return View();
        }

        // POST: DPreSets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,Deduction,Remark,StartDate,Rate,Stopped,Auditdatetime,AuditId,Rated,Status,Status2,Status3,Status4,Status5,Status6,saccocode,BranchCode")] DPreSet dPreSet)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (ModelState.IsValid)
            {
                _context.Add(dPreSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dPreSet);
        }

        // GET: DPreSets/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var dPreSet = await _context.d_PreSets.FindAsync(id);
            if (dPreSet == null)
            {
                return NotFound();
            }
            return View(dPreSet);
        }

        // POST: DPreSets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,Deduction,Remark,StartDate,Rate,Stopped,Auditdatetime,AuditId,Rated,Status,Status2,Status3,Status4,Status5,Status6,saccocode,BranchCode")] DPreSet dPreSet)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id != dPreSet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dPreSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPreSetExists(dPreSet.Id))
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
            return View(dPreSet);
        }

        // GET: DPreSets/Delete/5
        public async Task<IActionResult> Stop(long id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var dPreSet = await _context.d_PreSets.FindAsync(id);
            if (id != dPreSet.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    dPreSet.Stopped = true;
                    _context.Update(dPreSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DPreSetExists(dPreSet.Id))
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
            return View(dPreSet);

        }

        // POST: DPreSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var dPreSet = await _context.d_PreSets.FindAsync(id);
            _context.d_PreSets.Remove(dPreSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DPreSetExists(long id)
        {
            return _context.d_PreSets.Any(e => e.Id == id);
        }
    }
}
