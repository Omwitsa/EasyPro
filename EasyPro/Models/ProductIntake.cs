using EasyPro.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPro.Models
{
    [Table("ProductIntake")]
    public class ProductIntake
    {
        public long Id { get; set; }
        [Required]
        public string Sno { get; set; }
        [Required]
        public DateTime TransDate { get; set; }
        public TimeSpan TransTime { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? Ppu { get; set; }
        public decimal? CR { get; set; }
        public decimal? DR { get; set; }
        public decimal? Balance { get; set; }
        public string Description { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool Paid { get; set; }
        public string Remarks { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Branch { get; set; }
        public string SaccoCode { get; set; }
    }
}
