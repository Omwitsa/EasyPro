using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class SoldItem
    {
        public long Id { get; set; }
        public string Product { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Amount { get; set; }
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }
        public string Saccocode { get; set; }
        public string Branch { get; set; }
        public string AuditId { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}