using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgReceiptsalesrep
    {
        public long Id { get; set; }
        public long RId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Paid { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
