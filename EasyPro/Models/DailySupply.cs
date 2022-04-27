using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DailySupply
    {
        public DateTime? Date { get; set; }
        public double? Intake { get; set; }
        public double? Dispatch { get; set; }
        public double? OpeningStock { get; set; }
        public double? ClosingStock { get; set; }
        public double? Dipping { get; set; }
        public double? Variance { get; set; }
        public string Branch { get; set; }
    }
}
