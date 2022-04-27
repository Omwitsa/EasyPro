using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutletSale
    {
        public long Id { get; set; }
        public string Pcode { get; set; }
        public string Pname { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? AuditDate { get; set; }
        public double? Quant { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Paid { get; set; }
        public string AuditId { get; set; }
        public string Description { get; set; }
        public string OutletName { get; set; }
        public string Mpesa { get; set; }
        public long? Run { get; set; }
    }
}
