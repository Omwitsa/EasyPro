using DinkToPdf;
using DinkToPdf.Contracts;
using EasyPro.Constants;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Utils;
using EasyPro.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<byte[]> GetIntakesPdf(IEnumerable<ProductIntake> productIntakeobj, DCompany company, string title, TransactionType type, string loggedInUser, string saccoBranch)
        {
            title = title ?? "";
            var suppliers = await _context.DSuppliers.Where(s => s.Scode == company.Name).ToListAsync();
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(i => i.Branch == saccoBranch).ToList();

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

        public byte[] GetDailySummary(List<IGrouping<DateTime, ProductIntake>> intakes, DCompany company, string title)
        {
            title = title ?? "";
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateDailySummaryHtml(intakes, company, title),
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


        public byte[] GetAgSalesReport(List<AgReceipt> receipts, DSupplier supplier)
        {
            var objectSettings = new ObjectSettings
            {
                HtmlContent = HtmlGenerator.GenerateAgSalesReceiptHtml(receipts, supplier),
                WebSettings = { DefaultEncoding = "utf-8" },
            };

            recieptGlobalSettings.DocumentTitle = "Sales Receipt";
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = recieptGlobalSettings,
                Objects = { objectSettings },
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GetBankPayroll(IEnumerable<DPayroll> dpayrollobj, DCompany company, string title, string loggedInUser, string saccoBranch)
        {
            title = title ?? "";
            var suppliers = _context.DSuppliers.Where(s => s.Scode == company.Name);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            if (user.AccessLevel == AccessLevel.Branch)
                suppliers = suppliers.Where(i => i.Branch == saccoBranch);

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HtmlGenerator.GenerateBankPayrollHtml(dpayrollobj, company, title, suppliers),
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

        public byte[] GetZonesIntakePdf(List<ProductIntake> productIntakes, DCompany company, string title)
        {
            title = title ?? "";
            var zones = _context.Zones.Where(z => z.Code == company.Name).OrderBy(z => z.Name).ToList();
            var content = HtmlGenerator.GenerateZoneIntakesHtml(productIntakes, company, title, zones);
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

        public byte[] PayCombinedSummary(DCompany company, string title, string loggedInUser, string saccoBranch, DateTime? dateTo)
        {
            title = title ?? "";
            var startDate = new DateTime(dateTo.GetValueOrDefault().Year, dateTo.GetValueOrDefault().Month, 1);
            var monthsLastDate = startDate.AddMonths(1).AddDays(-1);
            var user = _context.UserAccounts.FirstOrDefault(u => u.UserLoginIds.ToUpper().Equals(loggedInUser.ToUpper()));
            var payrolls =  _context.DPayrolls.Where(p => p.EndofPeriod >= startDate && p.EndofPeriod <= monthsLastDate
                && p.SaccoCode == company.Name).ToList();
            var EndofPeriod = payrolls.FirstOrDefault();
            var dTransportersPayRolls = _context.DTransportersPayRolls.Where(p => p.SaccoCode == company.Name && p.NetPay > 0 && p.EndPeriod == EndofPeriod.EndofPeriod)
               .OrderBy(p => p.Code).ToList();

            if (user.AccessLevel == AccessLevel.Branch)
            {
                payrolls = payrolls.Where(i => i.Branch == saccoBranch).ToList();
                dTransportersPayRolls = dTransportersPayRolls.Where(i => i.Branch == saccoBranch).ToList();
            }

            var bankGroupedPayroll = payrolls.GroupBy(p => p.Bank).ToList();
            var combinedSummaries = new List<CombinedSummary>();
            bankGroupedPayroll.ForEach(p =>
            {
                combinedSummaries.Add(new CombinedSummary
                {
                    Name = p.Key,
                    Amount = p.Sum(b => b.Npay)
                });
            });

            var bankGroupedTransporterPayroll = dTransportersPayRolls.GroupBy(p => p.BankName).ToList();
            bankGroupedTransporterPayroll.ForEach(p =>
            {
                combinedSummaries.Add(new CombinedSummary
                {
                    Name = p.Key,
                    Amount = p.Sum(b => b.NetPay)
                });
            });

            var content = HtmlGenerator.GeneratePayCombinedSummarysHtml(combinedSummaries, company, title);
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
    }
}
