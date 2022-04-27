using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DLocation
    {
        public long Id { get; set; }
        public string Lcode { get; set; }
        public string Lname { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
