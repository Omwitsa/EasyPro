using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransportDeduc
    {
        public long Id { get; set; }
        public string TransCode { get; set; }
        public DateTime? TdateDeduc { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string Period { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string Auditid { get; set; }
        public string Remarks { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public long? Yyear { get; set; }
        public double? Rate { get; set; }
        public int? Ai { get; set; }
    }
}
