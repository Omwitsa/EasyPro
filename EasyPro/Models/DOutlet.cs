using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutlet
    {
        public string PCode { get; set; }
        public string PName { get; set; }
        public DateTime? DateEntered { get; set; }
        public double? Qin { get; set; }
        public double? Qout { get; set; }
        public double? OBal { get; set; }
        public string UserId { get; set; }
        public decimal? Wprice { get; set; }
        public decimal? Rprice { get; set; }
        public string Branch { get; set; }
        public int? Type { get; set; }
        public long? Run { get; set; }
    }
}
