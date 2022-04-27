using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DQuality
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Name { get; set; }
        public string Canno { get; set; }
        public string Rate { get; set; }
        public string Quantity { get; set; }
        public string Quality { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public decimal? Amount { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
