using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Controllers
{
    public class DispatchBalancingController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly INotyfService _notyf;
        private IWebHostEnvironment _hostingEnvironment;
        private Utilities utilities;

        public DispatchBalancingController(MORINGAContext context, INotyfService notyf, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _notyf = notyf;
            _hostingEnvironment = hostingEnvironment;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var date = DateTime.Now;
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
            var dispatches = _context.DispatchBalancing
                .Where(i => i.Saccocode.ToUpper().Equals(sacco.ToUpper()) && i.Date >= startDate && i.Date <= endDate).OrderByDescending(s => s.Date).ToList();
            return View(dispatches);
        }

        public IActionResult Create()
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            return View();
        }
        // GET: Glsetups/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            if (id == null)
            {
                return NotFound();
            }

            var dipatchbal = await _context.DispatchBalancing
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dipatchbal == null)
            {
                return NotFound();
            }

            return View(dipatchbal);
        }

        // POST: ProductIntakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
            if (string.IsNullOrEmpty(loggedInUser))
                return Redirect("~/");
            utilities.SetUpPrivileges(this);
            var productIntaketodelete = await _context.DispatchBalancing.FindAsync(id);
            _context.DispatchBalancing.Remove(productIntaketodelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductIntakeExists(long id)
        {
            return _context.ProductIntake.Any(e => e.Id == id);
        }


        [HttpGet]
        public async Task<JsonResult> GetSuppliedItems(DateTime? date)
        {
            try
            {
                date = date ?? DateTime.Now;
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                var saccoBranch = HttpContext.Session.GetString(StrValues.Branch) ?? "";
                var suppliers = await _context.DSuppliers.Where(s => s.Scode == sacco).ToListAsync();
                var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
                if (user.AccessLevel == AccessLevel.Branch)
                    suppliers = suppliers.Where(s => s.Branch == saccoBranch).ToList();

                var supplierNos = suppliers.Select(s => s.Sno).Distinct().ToList();
                var intakes = await _context.ProductIntake.Where(s => s.TransDate== date && s.SaccoCode == sacco && supplierNos.Contains(s.Sno) 
                && (s.TransactionType == TransactionType.Intake || s.TransactionType == TransactionType.Correction)).ToListAsync();
                if (user.AccessLevel == AccessLevel.Branch)
                    intakes = intakes.Where(s => s.Branch == saccoBranch).ToList();

                var alredydispatch = await _context.Dispatch.Where(s => s.Transdate== date && s.Dcode == sacco).ToListAsync();
                var dispatch = await _context.DispatchBalancing.FirstOrDefaultAsync(d => d.Saccocode == sacco && d.Date == date);
                double dispatched = 0;
                if (dispatch == null)
                {
                    dispatched = (double)alredydispatch.Sum(c => c.Dispatchkgs);
                }
                else
                {
                    dispatched = (double)dispatch.Dispatch;
                }
                dispatch = dispatch == null ? new DispatchBalancing() : dispatch;
                var balancing = new DispatchBalancing
                {
                    Intake = intakes.Sum(i => i.Qsupplied),
                    Dispatch = (decimal?)dispatched,
                    CF = dispatch.CF,
                    BF = dispatch.BF,
                    Actuals = dispatch.Actuals,
                    Spillage = dispatch.Spillage,
                    Rejects = dispatch.Rejects,
                    Varriance = dispatch.Varriance,
                    Saccocode = sacco,
                };
                return Json(balancing);
            }
            catch (Exception e)
            {
                return Json("");
            }
        }

        [HttpPost]//editVariance
        public JsonResult SaveVariance([FromBody] DispatchBalancing balancing)
        {
            try
            {
                utilities.SetUpPrivileges(this);
                var loggedInUser = HttpContext.Session.GetString(StrValues.LoggedInUser) ?? "";
                var sacco = HttpContext.Session.GetString(StrValues.UserSacco) ?? "";
                if (balancing.Actuals == null)
                {
                    _notyf.Error("Sorry, Kindly provide actual Kgs");
                    return Json("");
                }
               
                var checkexist = _context.DispatchBalancing.Any(s => s.Date == balancing.Date && s.Saccocode == sacco);
                if (checkexist)
                {
                    _notyf.Error("Sorry, Variance already saved");
                    return Json("");
                }
                balancing.BF = balancing?.BF ?? 0;
                balancing.Actuals = balancing?.Actuals ?? 0;
                var available = balancing.BF + balancing.Actuals;
                if (balancing.Dispatch > available)
                {
                    _notyf.Error("Sorry, Dispatch cannot exceed available quantity");
                    return Json("");
                }

                balancing.Saccocode = sacco;
                _context.DispatchBalancing.Add(balancing);
                _context.SaveChanges();
                _notyf.Success("Saved successfully");
                return Json("");
            }
            catch (Exception e)
            {
                return Json("");
            }
        }
    }
}
