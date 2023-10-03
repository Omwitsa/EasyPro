using System;

namespace EasyPro.Models
{
    public class SaccoShares
    {
        public long Id { get; set; }
        public string SharesCode { get; set; }
        public string Sno { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Saccocode { get; set; }
        public string AuditId { get; set; }
    }
}