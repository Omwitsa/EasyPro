using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMilkBranch
    {
        public string Branch { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Actual { get; set; }
        public decimal? Variance { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Vehicle { get; set; }
    }
}
