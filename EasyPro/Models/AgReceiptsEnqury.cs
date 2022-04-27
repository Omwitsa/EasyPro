using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgReceiptsEnqury
    {
        public long Id { get; set; }
        public string Pcode { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public double? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public long RId { get; set; }
        public decimal? Paid { get; set; }
        public string Salesrep { get; set; }
    }
}
