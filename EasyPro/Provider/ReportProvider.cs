using DinkToPdf;
using DinkToPdf.Contracts;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPro.Provider
{
    public class ReportProvider : IReportProvider
    {
        private readonly IConverter _converter;
        private GlobalSettings recieptGlobalSettings;
        private readonly MORINGAContext _context;
        private GlobalSettings pdfGlobalSettings;
        public ReportProvider(IConverter converter, MORINGAContext context)
        {
            _context = context;
            _converter = converter;
            recieptGlobalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.Prc32K,
                Margins = new MarginSettings { Top = 2 },
            };
            pdfGlobalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
            };
        }
        
        public byte[] GetIntakeReport(ProductIntakeVm intake)
        {
            var objectSettings = new ObjectSettings
            {
                HtmlContent = HtmlGenerator.GenerateIntakeReceiptHtml(intake),
                WebSettings = { DefaultEncoding = "utf-8" },
            };

            recieptGlobalSettings.DocumentTitle = $"{intake.ProductType} Intake";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = recieptGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetIntakesPdf(IEnumerable<ProductIntake> productIntakeobj, DCompany company, string title, TransactionType type)
        {
            title = title ?? "";
            var suppliers = _context.DSuppliers.Where(s => s.Scode == company.Name);
            var content = HtmlGenerator.GenerateIntakesHtml(productIntakeobj, company, title, suppliers);
            if(type == TransactionType.Deduction)
                content = HtmlGenerator.GenerateSuppliersDeductionsHtml(productIntakeobj, company, title, suppliers);

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = content,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetSuppliersReport(IEnumerable<DSupplier> suppliersobj, DCompany company, string title)
        {
            title = title ?? "";
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateSuppliersHtml(suppliersobj, company, title),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetBranchIntakeReport(IEnumerable<DBranch> branchobj, DCompany company, string title, FilterVm filter)
        {
            title = title ?? "";
            var intakes = _context.ProductIntake
                        .Where(i => i.SaccoCode == company.Name && i.TransDate >= filter.DateFrom && i.Qsupplied != 0
                        && i.TransDate <= filter.DateTo);

            var suppliers = _context.DSuppliers.Where(s => s.Scode == company.Name);
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateBranchIntakeHtml(branchobj, company, title, intakes, suppliers),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetSuppliersPayroll(IEnumerable<DPayroll> dpayrollobj, DCompany company, string title)
        {
            title = title ?? "";
            var suppliers = _context.DSuppliers.Where(s => s.Scode == company.Name);
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateSuppliersPayrollHtml(dpayrollobj, company, title, suppliers),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetTransportersPayroll(IEnumerable<DTransportersPayRoll> transporterpayrollobj, DCompany company, string title)
        {
            title = title ?? "";
            var transporters = _context.DTransporters.Where(s => s.ParentT == company.Name);
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateTransportersPayrollHtml(transporterpayrollobj, company, title, transporters),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetTransporterReport(IEnumerable<DTransporter> transporterobj, DCompany company, string title)
        {
            title = title ?? "";
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GeneratetransportersHtml(transporterobj, company, title),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetTIntakePdf(IEnumerable<DTransporter> transporterobj, DCompany company, string title, FilterVm filter)
        {
            title = title ?? "";
            var suppliers = _context.DSuppliers.Where(s => s.Scode == company.Name);
            var intakes = _context.ProductIntake.Where(u => u.TransDate >= filter.DateFrom && u.TransDate <= filter.DateTo && u.Qsupplied != 0 && u.SaccoCode == company.Name);

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
               HtmlContent = HtmlGenerator.GenerateTIntakesHtml(transporterobj, company, title, intakes),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontSize = 8, Center = title, Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{title}";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = pdfGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }
    }
}
