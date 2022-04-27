using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class QueryPayroll
    {
        public short? SupplierNumber { get; set; }
        public string IdNumber { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public decimal? Transport { get; set; }
        public decimal? Agrovet { get; set; }
        public decimal? Ai { get; set; }
        public decimal? TMShares { get; set; }
        public decimal? HShares { get; set; }
        public decimal? Advance { get; set; }
        public decimal? Others { get; set; }
        public decimal? Total { get; set; }
        public double? TotalKgs { get; set; }
        public decimal? GrossPay { get; set; }
        public decimal? NetPay { get; set; }
        public string Period { get; set; }
        public string Trader { get; set; }
    }
}
