using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransportStandingorder
    {
        public string TransCode { get; set; }
        public DateTime? DateDeduc { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string Period { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Auditid { get; set; }
        public long? Yyear { get; set; }
        public string Remarks { get; set; }
    }
}
