using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyPro.Models;
using Microsoft.EntityFrameworkCore;
using EasyPro.Constants;

namespace EasyPro.Controllers
{
    public class ProductIntakesController : Controller
    {
        private readonly MORINGAContext _context;

        public ProductIntakesController(MORINGAContext context)
        {
            _context = context;
        }

        // GET: ProductIntakes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductIntake.Where(i => i.TransactionType == TransactionType.Intake).ToListAsync());
        }

        public async Task<IActionResult> DeductionList()
        {
            return View(await _context.ProductIntake.Where(i => i.TransactionType == TransactionType.Deduction).ToListAsync());
        }

        public async Task<IActionResult> CorrectionList()
        {
            return View(await _context.ProductIntake.Where(c => c.TransactionType == TransactionType.Correction).ToListAsync());
        }

        // GET: ProductIntakes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productIntake = await _context.ProductIntake
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productIntake == null)
            {
                return NotFound();
            }

            return View(productIntake);
        }

        // GET: ProductIntakes/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateDeduction()
        {
            return View();
        }

        public IActionResult CreateCorrection()
        {
            return View();
        }

        // POST: ProductIntakes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            if (ModelState.IsValid)
            {
                productIntake.TransactionType = TransactionType.Intake;
                _context.Add(productIntake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productIntake);
        }

        // GET: ProductIntakes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productIntake = await _context.ProductIntake.FindAsync(id);
            if (productIntake == null)
            {
                return NotFound();
            }
            return View(productIntake);
        }

        // POST: ProductIntakes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Sno,TransDate,ProductType,Qsupplied,Ppu,CR,DR,Balance,Description,Remarks,AuditId,Auditdatetime,Branch")] ProductIntake productIntake)
        {
            if (id != productIntake.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productIntake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductIntakeExists(productIntake.Id))
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
            return View(productIntake);
        }

        // GET: ProductIntakes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productIntake = await _context.ProductIntake
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productIntake == null)
            {
                return NotFound();
            }

            return View(productIntake);
        }

        // POST: ProductIntakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var productIntake = await _context.ProductIntake.FindAsync(id);
            _context.ProductIntake.Remove(productIntake);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductIntakeExists(long id)
        {
            return _context.ProductIntake.Any(e => e.Id == id);
        }
    }
}
