using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPaymentReq
    {
        public long Id { get; set; }
        public string Rno { get; set; }
        public string InvId { get; set; }
        public string Vendor { get; set; }
        public DateTime? InvDate { get; set; }
        public decimal? Amnt { get; set; }
        public string Desc { get; set; }
        public string AuditId { get; set; }
        public DateTime Auditdatetime { get; set; }
        public bool Posted { get; set; }
        public string Status { get; set; }
    }
}
