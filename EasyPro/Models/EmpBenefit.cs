using System;

namespace EasyPro.Models
{
    public class EmpBenefit
    {
        public long Id { get; set; }
        public string EmpNo { get; set; }
        public string EntType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Auditdate { get; set; }
        public string AuditId { get; set; }
        public string SaccoCode { get; set; }
    }
}