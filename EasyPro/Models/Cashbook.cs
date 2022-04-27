using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Cashbook
    {
        public long CashId { get; set; }
        public string TransId { get; set; }
        public string MemberNo { get; set; }
        public decimal? Amount { get; set; }
        public string TransDescript { get; set; }
        public string ReceiptNo { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? TransDate { get; set; }
        public int? Posted { get; set; }
        public int? IsMember { get; set; }
        public string AccNo { get; set; }
        public string AccNoCr { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditTime { get; set; }
    }
}
