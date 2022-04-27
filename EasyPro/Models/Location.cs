using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Location
    {
        public string Location1 { get; set; }
        public double? Quantity { get; set; }
        public short? Active { get; set; }
        public short? Dormant { get; set; }
        public string Period { get; set; }
        public short? Days { get; set; }
    }
}
