using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Share
    {
        public int? Sno { get; set; }
        public int? ShareNumber { get; set; }
        public string Name { get; set; }
        public int? IdNo { get; set; }
        public string Location { get; set; }
        public decimal? Refunds { get; set; }
        public decimal? Amount { get; set; }
        public double? Rate { get; set; }
        public string Period { get; set; }
    }
}
