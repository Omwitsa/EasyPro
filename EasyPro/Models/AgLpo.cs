using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgLpo
    {
        public long Id { get; set; }
        public string Pono { get; set; }
        public string Vendor { get; set; }
        public DateTime? Lpodate { get; set; }
        public DateTime? Duedate { get; set; }
        public string Lposerialno { get; set; }
        public string Remarks { get; set; }
        public string Itemno { get; set; }
        public string Itemname { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Cost { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
