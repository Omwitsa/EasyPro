using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTmpEnquery
    {
        public long Id { get; set; }
        public long? Sno { get; set; }
        public DateTime? TransDate { get; set; }
        public string Description { get; set; }
        public double? Intake { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Dr { get; set; }
        public decimal? Bal { get; set; }
        public string Type2 { get; set; }
    }
}
