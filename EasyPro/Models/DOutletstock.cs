using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutletstock
    {
        public long Id { get; set; }
        public DateTime? DateEntered { get; set; }
        public string PName { get; set; }
        public double? Quantity { get; set; }
        public string OutletName { get; set; }
        public long? Run { get; set; }
    }
}
