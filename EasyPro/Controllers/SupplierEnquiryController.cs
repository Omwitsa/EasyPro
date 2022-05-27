using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EasyPro.Models;
using EasyPro.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EasyPro.Constants;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace EasyPro.Controllers
{
    public class SupplierEnquiryController : Controller
    {
        private readonly MORINGAContext _context;
        private Utilities utilities;

        public SupplierEnquiryController(MORINGAContext context)
        {
            _context = context;
            utilities = new Utilities(context);
        }
        public IActionResult Index()
        {
            utilities.SetUpPrivileges(this);
            GetInitialValues();
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            
            ViewBag.suppliers = _context.DSuppliers.Select(s => new DSupplier
            {
                Sno = s.Sno,
                Names = s.Names,
                IdNo = s.IdNo,
                PhoneNo = s.PhoneNo,
                AccNo = s.AccNo,
                Bbranch = s.Bbranch
            }).ToList();
            //ViewBag.intakess = _context.ProductIntake.Select(s => new ProductIntake
            //{
            //    Sno = s.Sno,
            //    ProductType = s.ProductType,
            //}).ToList();

            return View();
        }
        private void GetInitialValues()
        {
            var products = _context.DBranchProducts.Select(b => b.Bname).ToList();
            ViewBag.products = new SelectList(products);
        }
        [HttpPost]
        public JsonResult SuppliedProducts([FromBody] DSupplier supplier)
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            //var intakes = _context.ProductIntake.Where(i => i.Sno == supplier.Sno).ToList();
            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate).Where(i => i.Sno == supplier.Sno.ToString()).ToList();
            return Json(intakes);
        }
        [HttpPost]
        public JsonResult SuppliedProducts2([FromBody] ProductIntake intakess)
        {
            utilities.SetUpPrivileges(this);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var enDate = startDate.AddMonths(1).AddDays(-1);
            var intakes = _context.ProductIntake.OrderByDescending(i => i.TransDate).Where(i => i.Sno ==intakess.ProductType).ToList();
            return Json(intakes);
        }

    }
}
