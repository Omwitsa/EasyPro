using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace EasyPro.Controllers.Reports
{
    public class PdfReportController : Controller
    {
        private readonly MORINGAContext _context;
        private readonly IReportProvider _reportProvider;
        public PdfReportController(IReportProvider reportProvider, MORINGAContext context)
        {
            _reportProvider = reportProvider;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetIntakeReceipt(long? id)
        {
            var productIntake = _context.ProductIntake.FirstOrDefault(i => i.Id == id);
            if(productIntake != null)
            {
                long.TryParse(productIntake.Sno, out long sno);
                var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno);
                var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var cumlativeIntake = _context.ProductIntake.Where(s => s.Sno == productIntake.Sno
                && s.TransDate >= startDate && s.ProductType.ToUpper().Equals(productIntake.ProductType.ToUpper()));
                var intake = new ProductIntakeVm
                {
                    SaccoCode = productIntake.SaccoCode,
                    Sno = productIntake.Sno,
                    SupName = supplier.Names,
                    Qsupplied = productIntake.Qsupplied,
                    ProductType = productIntake.ProductType,
                    TransDate = productIntake.TransDate,
                    AuditId = productIntake.AuditId,
                    Cumlative = cumlativeIntake.Sum(c => c.Qsupplied),
                    PhoneNo = supplier.PhoneNo
                };

                var pdfFile = _reportProvider.GetIntakeReport(intake);
                return File(pdfFile, "application/pdf");
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult GetAgSalesReceipt(string rno)
        {
            var receipts = _context.AgReceipts.Where(i => i.RNo == rno).ToList();
            if (receipts.Any())
            {
                var rSno = receipts.FirstOrDefault().SNo ?? "";
                long.TryParse(rSno, out long sno);
                var supplier = _context.DSuppliers.FirstOrDefault(s => s.Sno == sno);
                var pdfFile = _reportProvider.GetAgSalesReport(receipts, supplier);
                return File(pdfFile, "application/pdf");
            }

            return NotFound();
        }
    }
}
