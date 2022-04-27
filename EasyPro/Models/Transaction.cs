using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Transaction
    {
        public string TransactionNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransDate { get; set; }
        public string AuditId { get; set; }
        public DateTime AuditTime { get; set; }
        public string TransDescription { get; set; }
        public string Status { get; set; }
    }
}
