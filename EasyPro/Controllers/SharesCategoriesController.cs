using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyPro.Models;
using EasyPro.Constants;
using Microsoft.AspNetCore.Http;
using EasyPro.Utils;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EasyPro.Controllers
{
    public class SharesCategoriesController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private Utilities utilities;

        public SharesCategoriesController(MORINGAContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            utilities = new Utilities(context);
        }


        // GET: SharesCategories
        public async Task<IActionResult> Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            return View(await _context.SharesCategories.Where(c => c.SaccoCode == sacco).ToListAsync());
        }

        // GET: SharesCategories/Details/5
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

            var sharesCategory = await _context.SharesCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sharesCategory == null)
            {
                return NotFound();
            }

            return View(sharesCategory);
        }

        // GET: SharesCategories/Create
        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }

        // POST: SharesCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,MinAmount,MaxAmount,SaccoCode")] SharesCategory sharesCategory)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            sharesCategory.MinAmount = sharesCategory?.MinAmount ?? 0;
            sharesCategory.MaxAmount = sharesCategory?.MaxAmount ?? 0;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sharesCategory.SaccoCode = sacco;
            if (string.IsNullOrEmpty(sharesCategory.Name))
            {
                _notyf.Error("Sorry, Kindly provide shares category");
                return View(sharesCategory);
            }   
            if(sharesCategory.MinAmount < 1)
            {
                _notyf.Error("Sorry, Kindly provide minimum amount for the category");
                return View(sharesCategory);
            }
            if (sharesCategory.MaxAmount < 1)
            {
                _notyf.Error("Sorry, Kindly provide maximum amount for the category");
                return View(sharesCategory);
            }
            if (sharesCategory.MinAmount > sharesCategory.MaxAmount)
            {
                _notyf.Error("Sorry, Minimum amount cannot be greater than maximum amount");
                return View(sharesCategory);
            }
            var exist = _context.SharesCategories.Any(s => s.SaccoCode == sacco && s.Name.ToUpper().Equals(sharesCategory.Name.ToUpper()));
            if (exist)
            {
                _notyf.Error("Sorry, Category already exist");
                return View(sharesCategory);
            }
            if (ModelState.IsValid)
            {
                _context.Add(sharesCategory);
                await _context.SaveChangesAsync();
                _notyf.Success("Category saved successfully");
                return RedirectToAction(nameof(Index));
            }
            return View(sharesCategory);
        }

        // GET: SharesCategories/Edit/5
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

            var sharesCategory = await _context.SharesCategories.FindAsync(id);
            if (sharesCategory == null)
            {
                return NotFound();
            }
            return View(sharesCategory);
        }

        // POST: SharesCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,MinAmount,MaxAmount,SaccoCode")] SharesCategory sharesCategory)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id != sharesCategory.Id)
            {
                return NotFound();
            }
            sharesCategory.MinAmount = sharesCategory?.MinAmount ?? 0;
            sharesCategory.MaxAmount = sharesCategory?.MaxAmount ?? 0;
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco);
            sharesCategory.SaccoCode = sacco;
            if (string.IsNullOrEmpty(sharesCategory.Name))
            {
                _notyf.Error("Sorry, Kindly provide shares category");
                return View(sharesCategory);
            }
            if (sharesCategory.MinAmount < 1)
            {
                _notyf.Error("Sorry, Kindly provide minimum amount for the category");
                return View(sharesCategory);
            }
            if (sharesCategory.MaxAmount < 1)
            {
                _notyf.Error("Sorry, Kindly provide maximum amount for the category");
                return View(sharesCategory);
            }
            if (sharesCategory.MinAmount > sharesCategory.MaxAmount)
            {
                _notyf.Error("Sorry, Minimum amount cannot be greater than maximum amount");
                return View(sharesCategory);
            }
            var exist = _context.SharesCategories.Any(s => s.SaccoCode == sacco 
            && s.Name.ToUpper().Equals(sharesCategory.Name.ToUpper()) && s.Id != sharesCategory.Id);
            if (exist)
            {
                _notyf.Error("Sorry, Category already exist");
                return View(sharesCategory);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sharesCategory);
                    _notyf.Success("Category updated successfully");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SharesCategoryExists(sharesCategory.Id))
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
            return View(sharesCategory);
        }

        // GET: SharesCategories/Delete/5
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

            var sharesCategory = await _context.SharesCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sharesCategory == null)
            {
                return NotFound();
            }

            return View(sharesCategory);
        }

        // POST: SharesCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var sharesCategory = await _context.SharesCategories.FindAsync(id);
            _context.SharesCategories.Remove(sharesCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SharesCategoryExists(long id)
        {
            return _context.SharesCategories.Any(e => e.Id == id);
        }
    }
}
