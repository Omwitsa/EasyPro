using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class B2cpaymentDummy
    {
        public long Id { get; set; }
        public string ReceiptNo { get; set; }
        public string Sno { get; set; }
        public string Names { get; set; }
        public string PhoneNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? DatePosted { get; set; }
        public string Location { get; set; }
        public string OtherParyInfo { get; set; }
        public string TransactionStatus { get; set; }
        public string User1 { get; set; }
        public DateTime? AuditDateTime { get; set; }
        public long? Status1 { get; set; }
    }
}
