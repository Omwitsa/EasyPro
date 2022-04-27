using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TransportersPayQuery
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? IdNumber { get; set; }
        public string BankName { get; set; }
        public string AccNumber { get; set; }
        public string Branch { get; set; }
        public decimal? Amnt { get; set; }
        public decimal? Subsidy { get; set; }
        public decimal? GrossPay { get; set; }
        public decimal? Agrovet { get; set; }
        public decimal? TMShares { get; set; }
        public decimal? HShares { get; set; }
        public decimal? Advances { get; set; }
        public decimal? Ai { get; set; }
        public decimal? Others { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetPay { get; set; }
        public string Period { get; set; }
    }
}
