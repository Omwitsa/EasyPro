using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class PaySlip
    {
        [Key]
        public long Id { get; set; }
        public string EmpNo { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime PDate { get; set; }
        public string AuditId { get; set; }
        public string SaccoCode { get; set; }
    }
}