using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Contrib
    {
        public long Id { get; set; }
        public string MemberNo { get; set; }
        public DateTime ContrDate { get; set; }
        public long? RefNo { get; set; }
        public decimal? Amount { get; set; }
        public decimal Commission { get; set; }
        public decimal Interest { get; set; }
        public double IntRate { get; set; }
        public decimal? ShareBal { get; set; }
        public string TransBy { get; set; }
        public string ChequeNo { get; set; }
        public string ReceiptNo { get; set; }
        public string Locked { get; set; }
        public string Posted { get; set; }
        public string Remarks { get; set; }
        public string AuditId { get; set; }
        public DateTime AuditTime { get; set; }
        public string Sharescode { get; set; }
        public string TransactionNo { get; set; }
        public bool Used { get; set; }
        public short Fperiod { get; set; }
        public DateTime? Maturitydate { get; set; }
    }
}
