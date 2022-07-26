using System;

namespace EasyPro.Models
{
    public class ExcelDump
    {
        public long Id { get; set; }
        public string LoggedInUser { get; set; }
        public string SaccoCode { get; set; }
        public string Sno { get; set; }
        public string ProductType { get; set; }
        public decimal Quantity { get; set; }
        public DateTime TransDate { get; set; }
    }
}