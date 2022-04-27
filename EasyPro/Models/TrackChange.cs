using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TrackChange
    {
        public int? SupplierNumber { get; set; }
        public double? Qnty { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Timestamp { get; set; }
        public string User { get; set; }
    }
}
