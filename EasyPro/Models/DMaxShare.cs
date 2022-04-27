using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMaxShare
    {
        public long Id { get; set; }
        public string IdNo { get; set; }
        public decimal? MaxAmnt { get; set; }
        public string AuditId { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
