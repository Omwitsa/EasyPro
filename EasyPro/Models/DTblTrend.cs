using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTblTrend
    {
        public long Id { get; set; }
        public long Sno { get; set; }
        public decimal? Kgs { get; set; }
        public decimal? Average { get; set; }
        public string Auditid { get; set; }
        public DateTime? Transdate { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
