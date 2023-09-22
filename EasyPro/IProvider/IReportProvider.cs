using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.IProvider
{
    public interface IReportProvider
    {
        public byte[] GetIntakeReport(ProductIntakeVm intake);
        public byte[] GetSuppliersReport(IEnumerable<DSupplier> suppliersobj, DCompany company, string supplierStatus);
        public Task<byte[]> GetIntakesPdf(IEnumerable<ProductIntake> productIntakeobj, DCompany company, string title, TransactionType intake, string loggedInUser, string saccoBranch);
        public byte[] GetBranchIntakeReport(IEnumerable<DBranch> branchobj, DCompany company, string title, FilterVm filter);
        public byte[] GetSuppliersPayroll(IEnumerable<DPayroll> dpayrollobj, DCompany company, string title);
        public byte[] GetTransportersPayroll(IEnumerable<DTransportersPayRoll> transporterpayrollobj, DCompany company, string title);
        public byte[] GetTransporterReport(IEnumerable<DTransporter> transporterobj, DCompany company, string title);
        public byte[] GetTIntakePdf(IEnumerable<DTransporter> transporterobj, DCompany company, string title, FilterVm filter);
        public byte[] GetAgSalesReport(List<AgReceipt> receipts, DSupplier supplier);
        public byte[] GetDailySummary(List<IGrouping<DateTime, ProductIntake>> intakes, DCompany company, string title);
        public byte[] GetBankPayroll(IEnumerable<DPayroll> dpayrollobj, DCompany company, string title, string loggedInUser, string saccoBranch);
        public byte[] GetZonesIntakePdf(List<ProductIntake> productIntakes, DCompany company, string title);
    }
}
