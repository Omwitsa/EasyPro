using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Smssubscription
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Subscription { get; set; }
        public string Freq { get; set; }
        public bool Active { get; set; }
        public string AuditId { get; set; }
        public DateTime AuditDateTime { get; set; }
        public string LastSent { get; set; }
    }
}
