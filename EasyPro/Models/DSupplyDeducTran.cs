using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DSupplyDeducTran
    {
        public long Id { get; set; }
        public long? Sno { get; set; }
        public string DedCode { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Posted { get; set; }
        public decimal? Balance { get; set; }
        public string TranType { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Yyear { get; set; }
        public string Isfrate { get; set; }
        public double? Rate { get; set; }
    }
}
