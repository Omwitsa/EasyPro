using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DCashShare
    {
        public long Id { get; set; }
        public DateTime Period { get; set; }
        public long Sno { get; set; }
        public decimal? Amount { get; set; }
        public string Ref { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdate { get; set; }
    }
}
