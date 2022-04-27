using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DCtype
    {
        public long Id { get; set; }
        public string ContCode { get; set; }
        public string ContName { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
