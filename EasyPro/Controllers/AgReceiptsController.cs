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
using EasyPro.ViewModels.TranssupplyVM;

namespace EasyPro.Controllers
{
    public class AgReceiptsController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;
        public AgReceiptsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }
       
        // GET: AgReceipts
        public async Task<IActionResult> Index()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }
        private void GetInitialValues()
        {
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var agproducts = _context.AgProducts.Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).Select(b => b.PName).ToList();
            ViewBag.agproductsall = new SelectList(agproducts, "");
        }
        // GET: AgReceipts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                GetInitialValues();
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts
                .FirstOrDefaultAsync(m => m.RId == id);
            if (agReceipt == null)
            {
                return NotFound();
            }

            return View(agReceipt);
        }

        // GET: AgReceipts/Create
        public IActionResult Create()
        {
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var count = _context.AgReceipts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper()))
                .OrderByDescending(u => u.RNo)
                .Select(b => b.RNo);
            var selectedno = count.FirstOrDefault();
            double num = Convert.ToInt32(selectedno);
            GetInitialValues();
            //AgProductobj = _context.AgProducts
            //.Where(p => p.saccocode == sacco)
            //.OrderBy(s => s.PCode);

            var receipt = new AgReceipt
            {
                RNo = "" + (num + 1),
                Qua = 0,
                Amount = 0,
                Sprice = 0,
                Bprice = 0,
                SBal = 0,
                TDate = DateTime.Today,
            };

            var transporters = _context.DTransporters.Where(s => s.ParentT == sacco).ToList();
            var suppliers = _context.DSuppliers.Where(s => s.Scode == sacco).ToList();
            var products = _context.AgProducts.Where(p => p.saccocode == sacco).ToList();
            var agrovetsales = new Agrovetsales
            {
                AgReceipt = receipt,
                DTransporter = transporters,
                DSuppliers = suppliers,
                AgProductobj = products
            };
            return View(agrovetsales);
            //Agrovetsalesobj = new Agrovetsales
            //{
            //    AgReceipt = new AgReceipt(),
            //    DTransporter = _context.DTransporters.Where(i => i.ParentT.ToUpper().Equals(sacco.ToUpper())),
            //    DSuppliers = _context.DSuppliers.Where(i => i.Scode.ToUpper().Equals(sacco.ToUpper())),
            //};
        }

        // POST: AgReceipts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (ModelState.IsValid)
            {
                _context.Add(agReceipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agReceipt);
        }

        // GET: AgReceipts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts.FindAsync(id);
            if (agReceipt == null)
            {
                return NotFound();
            }
            return View(agReceipt);
        }

        // POST: AgReceipts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("RId,RNo,PCode,TDate,Amount,SNo,Qua,SBal,UserId,AuditDate,Cash,Sno1,Transby,Idno,Mobile,Remarks,Branch,Sprice,Bprice,Ai,Run,Paid,Completed,Salesrep,saccocode")] AgReceipt agReceipt)
        {
            utilities.SetUpPrivileges(this);
            if (id != agReceipt.RId)
            {
                GetInitialValues();
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agReceipt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgReceiptExists(agReceipt.RId))
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
            return View(agReceipt);
        }

        // GET: AgReceipts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            if (id == null)
            {
                return NotFound();
            }

            var agReceipt = await _context.AgReceipts
                .FirstOrDefaultAsync(m => m.RId == id);
            if (agReceipt == null)
            {
                return NotFound();
            }

            return View(agReceipt);
        }

        // POST: AgReceipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            GetInitialValues();
            utilities.SetUpPrivileges(this);
            var agReceipt = await _context.AgReceipts.FindAsync(id);
            _context.AgReceipts.Remove(agReceipt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgReceiptExists(long id)
        {
            GetInitialValues();
            return _context.AgReceipts.Any(e => e.RId == id);
        }
    }
}
