using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyPro.Models;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;

namespace EasyPro.Controllers
{
    public class DBranchProductsController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;
        private readonly INotyfService _notyf;

        public DBranchProductsController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            utilities = new Utilities(context);
            _notyf = notyf;
        }

        // GET: DBranchProducts
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.DBranchProducts
                .Where(i => i.saccocode.ToUpper().Equals(sacco.ToUpper())).ToListAsync());
        }

        // GET: DBranchProducts/Details/5
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

            var dBranchProduct = await _context.DBranchProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBranchProduct == null)
            {
                return NotFound();
            }

            return View(dBranchProduct);
        }

        // GET: DBranchProducts/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: DBranchProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,saccocode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBranchProduct dBranchProduct)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            var dbranchproduct = _context.DBranchProducts.Where(i => i.saccocode == sacco && i.Bname == dBranchProduct.Bname).Count();
            if (dbranchproduct != 0)
            {
                _notyf.Error("Sorry, The product already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                dBranchProduct.saccocode = sacco;
                _context.Add(dBranchProduct);
                _notyf.Success("The product saved successfully");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }           
            return View(dBranchProduct);
        }

        // GET: DBranchProducts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dBranchProduct = await _context.DBranchProducts.FindAsync(id);
            if (dBranchProduct == null)
            {
                return NotFound();
            }
            return View(dBranchProduct);
        }

        // POST: DBranchProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,saccocode,Bname,Auditid,Auditdatetime,LocalId,Run")] DBranchProduct dBranchProduct)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            if (id != dBranchProduct.Id)
            {
                return NotFound();
            }
            var dbranchproduct = _context.DBranchProducts.Where(i => i.saccocode == sacco && i.Bname == dBranchProduct.Bname).Count();
            if (dbranchproduct != 0)
            {
                _notyf.Error("Sorry, The product already exist");
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    dBranchProduct.saccocode = sacco;
                    _context.Update(dBranchProduct);
                    _notyf.Success("The product updated successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DBranchProductExists(dBranchProduct.Id))
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
            return View(dBranchProduct);
        }

        // GET: DBranchProducts/Delete/5
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

            var dBranchProduct = await _context.DBranchProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dBranchProduct == null)
            {
                return NotFound();
            }

            return View(dBranchProduct);
        }

        // POST: DBranchProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var dBranchProduct = await _context.DBranchProducts.FindAsync(id);
            _context.DBranchProducts.Remove(dBranchProduct);
            await _context.SaveChangesAsync();
            _notyf.Error("The product Deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool DBranchProductExists(long id)
        {
            return _context.DBranchProducts.Any(e => e.Id == id);
        }
    }
}
