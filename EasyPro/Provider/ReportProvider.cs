using DinkToPdf;
using DinkToPdf.Contracts;
using EasyPro.IProvider;
using EasyPro.Utils;
using EasyPro.ViewModels;
using System;
using System.Text;

namespace EasyPro.Provider
{
    public class ReportProvider : IReportProvider
    {
        private readonly IConverter _converter;
        private GlobalSettings recieptGlobalSettings;
        private GlobalSettings pdfGlobalSettings;
        public ReportProvider(IConverter converter)
        {
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
                //PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateIntakeReceiptHtml(intake),
                WebSettings = { DefaultEncoding = "utf-8" },
                //HeaderSettings = { FontSize = 10, Right = "Page [page] of [toPage]", Line = true },
                //FooterSettings = { FontSize = 8, Center = "PDF demo from JeminPro", Line = true },
            };

            recieptGlobalSettings.DocumentTitle = $"{intake.ProductType} Intake";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = recieptGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }
    }
}
