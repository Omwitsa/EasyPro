using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DRequisition
    {
        public long Id { get; set; }
        public string Rno { get; set; }
        public DateTime? ReqDate { get; set; }
        public DateTime? TransDate { get; set; }
        public string CostCentre { get; set; }
        public bool? ServiceReq { get; set; }
        public string Iname { get; set; }
        public string Make { get; set; }
        public double? Qnty { get; set; }
        public string Description { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditDatetime { get; set; }
        public string Rbatch { get; set; }
        public string Status { get; set; }
        public decimal? Pricing { get; set; }
    }
}
