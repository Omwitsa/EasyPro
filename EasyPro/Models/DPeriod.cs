using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPeriod
    {
        public long Id { get; set; }
        public DateTime? EndPeriod { get; set; }
        public bool? Closed { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditdateTime { get; set; }
    }
}
