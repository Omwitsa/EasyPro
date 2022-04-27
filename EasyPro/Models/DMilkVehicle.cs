using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMilkVehicle
    {
        public long Id { get; set; }
        public string Vehicle { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Actual { get; set; }
        public decimal? Varriance { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string User { get; set; }
    }
}
