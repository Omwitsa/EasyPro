using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPayment
    {
        public long Id { get; set; }
        public string Rno { get; set; }
        public string InvId { get; set; }
        public long? Pono { get; set; }
        public decimal? Amnt { get; set; }
        public string PayMode { get; set; }
        public string Vendor { get; set; }
        public DateTime? PayDate { get; set; }
        public string Glacc { get; set; }
        public string Vno { get; set; }
        public string Category { get; set; }
        public decimal? Balance { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
