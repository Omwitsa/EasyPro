using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutletVehicle
    {
        public long Id { get; set; }
        public string Vehicle { get; set; }
        public DateTime? Date { get; set; }
        public double? Kgs { get; set; }
        public string Customer { get; set; }
        public long? Run { get; set; }
    }
}
