using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDebtorsparchases2
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Remarks { get; set; }
        public double? Kgs { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Expenses { get; set; }
        public string Vehicle { get; set; }
    }
}
