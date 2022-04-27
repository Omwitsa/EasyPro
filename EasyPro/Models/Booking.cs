using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Booking
    {
        public long Id { get; set; }
        public DateTime TransDate { get; set; }
        public decimal Amount { get; set; }
        public string DrAccNo { get; set; }
        public string CrAccNo { get; set; }
        public string DocumentNo { get; set; }
        public string Source { get; set; }
        public string TransDescript { get; set; }
        public DateTime AuditTime { get; set; }
        public string AuditId { get; set; }
        public int Cash { get; set; }
        public int DocPosted { get; set; }
        public string ChequeNo { get; set; }
        public bool? Dregard { get; set; }
        public bool Glpost { get; set; }
    }
}
