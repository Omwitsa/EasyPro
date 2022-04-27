using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Cheque
    {
        public long ChequeId { get; set; }
        public string LoanNo { get; set; }
        public string ChequeNo { get; set; }
        public decimal? Amount { get; set; }
        public decimal IntAmount { get; set; }
        public decimal Premium { get; set; }
        public decimal OffsetAmount { get; set; }
        public string CollectorId { get; set; }
        public string CollectorName { get; set; }
        public DateTime? DateIssued { get; set; }
        public string ClerkStaffNo { get; set; }
        public string ClerkName { get; set; }
        public string Status { get; set; }
        public string Reasons { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditTime { get; set; }
        public string Remarks { get; set; }
        public string LoanAcc { get; set; }
        public string ContraAcc { get; set; }
        public string PremiumAcc { get; set; }
        public int Dregard { get; set; }
    }
}
