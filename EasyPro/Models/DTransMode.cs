using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransMode
    {
        public long Id { get; set; }
        public string TransCode { get; set; }
        public string Transport { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public double? Rate { get; set; }
    }
}
