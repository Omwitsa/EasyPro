using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransDetailed
    {
        public long Id { get; set; }
        public long? Sno { get; set; }
        public double? Qnty { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Subsidy { get; set; }
        public string TransCode { get; set; }
        public DateTime? EndPeriod { get; set; }
        public int? Mmonth { get; set; }
        public int? Yyear { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
