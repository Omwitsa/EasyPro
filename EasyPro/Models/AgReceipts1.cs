using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgReceipts1
    {
        public int RId { get; set; }
        public double? RNo { get; set; }
        public string PCode { get; set; }
        public DateTime? TDate { get; set; }
        public decimal? Amount { get; set; }
        public string SNo { get; set; }
        public double? Qua { get; set; }
        public double? SBal { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
