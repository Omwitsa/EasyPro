using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DIncomestate
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public double? Sales { get; set; }
        public double? Purchases { get; set; }
        public double? Diff { get; set; }
    }
}
