using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPreSet
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Deduction { get; set; }
        public string Remark { get; set; }
        public string StartDate { get; set; }
        public decimal? Rate { get; set; }
        public bool? Stopped { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string AuditId { get; set; }
        public bool? Rated { get; set; }
        public long? Status { get; set; }
        public long? Status2 { get; set; }
        public long? Status3 { get; set; }
        public long? Status4 { get; set; }
        public long? Status5 { get; set; }
        public long? Status6 { get; set; }
        public string saccocode { get; set; }
        public string BranchCode { get; set; }
    }
}
