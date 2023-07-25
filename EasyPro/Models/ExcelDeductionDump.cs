using System;

namespace EasyPro.Models
{
    public class ExcelDeductionDump
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string ProductType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransDate { get; set; }
        public string LoggedInUser { get; set; }
        public string Branch { get; set; }
        public string SaccoCode { get; set; }
    }
}