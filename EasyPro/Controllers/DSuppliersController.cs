using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;

namespace EasyPro.Controllers
{
    public class DSuppliersController : Controller
    {
        private readonly MORINGAContext _context;

        public DSuppliersController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: DSuppliers
        public async Task<IActionResult> Index()
        {
            return View(await _context.DSuppliers.ToListAsync());
        }

        // GET: DSuppliers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dSupplier = await _context.DSuppliers
                .FirstOrDefaultAsync(m => m.Sno == id);
            if (dSupplier == null)
            {
                return NotFound();
            }

            return View(dSupplier);
        }

        // GET: DSuppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DSuppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LocalId,Sno,Regdate,IdNo,Names,AccNo,Bcode,Bbranch,Type,Village,Location,Division,District,Trader,Active,Approval,Branch,PhoneNo,Address,Town,Email,TransCode,Sign,Photo,AuditId,Auditdatetime,Scode,Loan,Compare,Isfrate,Frate,Rate,Hast,Br,Mno,Branchcode,HasNursery,Notrees,Aarno,Tmd,Landsize,Thcpactive,Thcppremium,Status,Status2,Status3,Status4,Status5,Status6,Types,Dob,Freezed,Mass,Status1,Run")] DSupplier dSupplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dSupplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dSupplier);
        }

        // GET: DSuppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dSupplier = await _context.DSuppliers.FindAsync(id);
            if (dSupplier == null)
            {
                return NotFound();
            }
            return View(dSupplier);
        }

        // POST: DSuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,LocalId,Sno,Regdate,IdNo,Names,AccNo,Bcode,Bbranch,Type,Village,Location,Division,District,Trader,Active,Approval,Branch,PhoneNo,Address,Town,Email,TransCode,Sign,Photo,AuditId,Auditdatetime,Scode,Loan,Compare,Isfrate,Frate,Rate,Hast,Br,Mno,Branchcode,HasNursery,Notrees,Aarno,Tmd,Landsize,Thcpactive,Thcppremium,Status,Status2,Status3,Status4,Status5,Status6,Types,Dob,Freezed,Mass,Status1,Run")] DSupplier dSupplier)
        {
            if (id != dSupplier.Sno)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dSupplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DSupplierExists(dSupplier.Sno))
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
            return View(dSupplier);
        }

        // GET: DSuppliers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dSupplier = await _context.DSuppliers
                .FirstOrDefaultAsync(m => m.Sno == id);
            if (dSupplier == null)
            {
                return NotFound();
            }

            return View(dSupplier);
        }

        // POST: DSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var dSupplier = await _context.DSuppliers.FindAsync(id);
            _context.DSuppliers.Remove(dSupplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DSupplierExists(long id)
        {
            return _context.DSuppliers.Any(e => e.Sno == id);
        }
    }
}
