using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace EasyPro.ViewModels
{
    public class SupplierSummeryVm
    {
        public List<CountySupplierSummery> countySuppliers { get; set; }
        public int TotalSupplies { get; set; }
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
    }

    public class CountySupplierSummery
    {
        public string County { get; set; }
        public List<SaccoSupplierSummery> suppliers { get; set; }
        public int TotalSupplies { get; set; }
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
    }

    public class SaccoSupplierSummery
    {
        public string Sacco { get; set; }
        public int Suppliers { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Transporters { get; set; }
    }
}
