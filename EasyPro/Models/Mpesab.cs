using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Mpesab
    {
        public long Id { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? CompletionTime { get; set; }
        public DateTime? InitiationTime { get; set; }
        public string Details { get; set; }
        public string TransactionStatus { get; set; }
        public decimal? PaidIn { get; set; }
        public decimal? Withdrawn { get; set; }
        public decimal? Balance { get; set; }
        public string BalanceConfirmed { get; set; }
        public string ReasonType { get; set; }
        public string OtherPartyInfo { get; set; }
        public string Reference { get; set; }
        public string LinkedTransactionId { get; set; }
        public string Phoneno { get; set; }
        public string Narration { get; set; }
        public string AcNo { get; set; }
        public long Run { get; set; }
        public long? Run2 { get; set; }
        public DateTime Auditdatetime { get; set; }
    }
}
