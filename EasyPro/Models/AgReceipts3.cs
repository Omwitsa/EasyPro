using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgReceipts3
    {
        public long RId { get; set; }
        public string RNo { get; set; }
        public string PCode { get; set; }
        public DateTime? TDate { get; set; }
        public decimal? Amount { get; set; }
        public string SNo { get; set; }
        public double? Qua { get; set; }
        public double? SBal { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
        public bool? Cash { get; set; }
        public string Sno1 { get; set; }
        public string Transby { get; set; }
        public string Idno { get; set; }
        public string Mobile { get; set; }
        public string Remarks { get; set; }
        public string Branch { get; set; }
        public int? Ai { get; set; }
        public string Salesrep { get; set; }
    }
}
