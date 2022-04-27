using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Audittran
    {
        public long TransId { get; set; }
        public string TransTable { get; set; }
        public string TransDescription { get; set; }
        public DateTime TransDate { get; set; }
        public decimal Amount { get; set; }
        public string AuditId { get; set; }
        public DateTime AuditTime { get; set; }
    }
}
