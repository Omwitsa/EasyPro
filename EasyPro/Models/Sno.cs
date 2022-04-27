using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Sno
    {
        public long Receipthnoid { get; set; }
        public string Receiptno { get; set; }
        public DateTime Auditdate { get; set; }
        public string AuditId { get; set; }
    }
}
