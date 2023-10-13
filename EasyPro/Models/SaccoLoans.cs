using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPro.Models
{
    public class SaccoLoans
    {
        public long Id { get; set; }
        public string LoanNo { get; set; }
        public string LoanCode { get; set; }
        public string Sno { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Saccocode { get; set; }
        public string AuditId { get; set; }
        [NotMapped]
        public decimal? Balance { get; set; }
    }
}