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
using EasyPro.ViewModels.BonusVM;

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
        public async Task<IActionResult> Index(string Search)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            ViewData["Getsuppliers"] = Search;
            var suppliers = _context.d_PreSets
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && !i.Stopped)
                .OrderByDescending(m=>m.Auditdatetime);
            return View(suppliers);
        }
        //
        public async Task<IActionResult> UnaprovedIndex(string Search)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            ViewData["Getsuppliers"] = Search;
            var suppliers = _context.d_PreSets
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Stopped);
            if (!string.IsNullOrEmpty(Search))
                suppliers = suppliers.Where(x => x.Sno.ToUpper().Contains(Search.ToUpper()) || x.Deduction.ToUpper().Contains(Search.ToUpper()) 
                || x.Remark.ToUpper().Contains(Search.ToUpper()) || x.BranchCode.ToUpper().Contains(Search.ToUpper()));

            return View(suppliers);
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
            ViewBag.Description = new SelectList(dDcodes); 
        }

        [HttpPost]
        public JsonResult Save(DateTime date,string? Sno,string Deduction, string Remark,decimal amtRate, bool isspecific, bool isallsup, bool isRate,bool isFixed)
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

                if (amtRate <= 0)
                {
                    _notyf.Error("Sorry, Kindly Provide Rate Amount");
                    return Json("");
                }

                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccobranch = HttpContext.Session.GetString(StrValues.Branch);

                Boolean Rated = true;
                if (isFixed)
                    Rated = false;
                
                if (isallsup)
                {
                    var supplierslist = _context.d_PreSets
                .Where(p => p.Deduction.ToUpper().Equals(Deduction.ToUpper())
                && p.saccocode.ToUpper().Equals(sacco.ToUpper()));
                    if (supplierslist.Any())
                    {
                        _context.d_PreSets.RemoveRange(supplierslist);
                        _context.SaveChanges();
                    }

                    var suppliers = _context.DSuppliers.Where(h => h.Scode == sacco).ToList();
                    suppliers.ForEach(s => {
                            _context.Add(new DPreSet
                            {
                                Sno = s.Sno,
                                Deduction = Deduction,
                                Remark = Remark,
                                StartDate = date,
                                Rate = amtRate,
                                Stopped = false,
                                Auditdatetime = DateTime.Now,
                                AuditId = loggedInUser,
                                Rated = Rated,
                                Status = 0,
                                Status2 = 0,
                                Status3 = 0,
                                Status4 = 0,
                                Status5 = 0,
                                Status6 = 0,
                                saccocode = sacco,
                                BranchCode = s.Branch,
                            });
                        });
                }
                if (isspecific)
                {
                    var supplierslist = _context.d_PreSets
                .Where(p => p.Deduction.ToUpper().Equals(Deduction.ToUpper())
                && p.saccocode.ToUpper().Equals(sacco.ToUpper()) && p.Sno.ToUpper().Equals(Sno.ToUpper()));
                    if (supplierslist.Any())
                    {
                        _context.d_PreSets.RemoveRange(supplierslist);
                        _context.SaveChanges();
                    }

                    var intakess = _context.DSuppliers.FirstOrDefault(s =>s.Scode == sacco && s.Sno.ToUpper().Equals(Sno.ToUpper()));
                        _context.Add(new DPreSet
                        {
                            Sno = intakess.Sno,
                            Deduction = Deduction,
                            Remark = Remark,
                            StartDate = date,
                            Rate = amtRate,
                            Stopped = false,
                            Auditdatetime = DateTime.Now,
                            AuditId = loggedInUser,
                            Rated = Rated,
                            Status = 0,
                            Status2 = 0,
                            Status3 = 0,
                            Status4 = 0,
                            Status5 = 0,
                            Status6 = 0,
                            saccocode = sacco,
                            BranchCode = intakess.Branch,
                        });
                }
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

        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dSupplier = await _context.d_PreSets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dSupplier == null)
            {
                return NotFound();
            }

            return View(dSupplier);
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
