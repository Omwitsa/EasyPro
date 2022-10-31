using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMilkControl1
    {
        public long Id { get; set; }
        public DateTime? DispDate { get; set; }
        public double? DispQnty { get; set; }
        public double? DipQnty { get; set; }
        public double? InQnty { get; set; }
        public double? Variance { get; set; }
        public decimal? Price { get; set; }
        public string RefNo { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Dcode { get; set; }
        public string BranchCode { get; set; }
    }
}
