using EasyPro.Constants;
using EasyPro.Models;
using EasyPro.ViewModels;
using System.Collections.Generic;

namespace EasyPro.IProvider
{
    public interface IReportProvider
    {
        public byte[] GetIntakeReport(ProductIntakeVm intake);
        public byte[] GetSuppliersReport(IEnumerable<DSupplier> suppliersobj, DCompany company, string supplierStatus);
        public byte[] GetIntakesPdf(IEnumerable<ProductIntake> productIntakeobj, DCompany company, string title, TransactionType intake);
        public byte[] GetBranchIntakeReport(IEnumerable<DBranch> branchobj, DCompany company, string title, FilterVm filter);
        public byte[] GetSuppliersPayroll(IEnumerable<DPayroll> dpayrollobj, DCompany company, string title);
        public byte[] GetTransportersPayroll(IEnumerable<DTransportersPayRoll> transporterpayrollobj, DCompany company, string title);
        public byte[] GetTransporterReport(IEnumerable<DTransporter> transporterobj, DCompany company, string title);
        public byte[] GetTIntakePdf(IEnumerable<DTransporter> transporterobj, DCompany company, string title, FilterVm filter);
    }
}
