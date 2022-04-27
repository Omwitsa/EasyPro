using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPayrollCopy
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Names { get; set; }
        public string PhoneNo { get; set; }
        public decimal? NetPay { get; set; }
        public DateTime? DatePosted { get; set; }
        public string Location { get; set; }
        public int? Yyear { get; set; }
        public int? Mmonth { get; set; }
        public string Phoneno2 { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string User3 { get; set; }
        public DateTime? AuditDateTime { get; set; }
        public long? Status1 { get; set; }
        public long? Status2 { get; set; }
        public long? Status3 { get; set; }
        public long? Run2 { get; set; }
    }
}
