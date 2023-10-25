using System;

namespace EasyPro.Models
{
    public class Inventory
    {
        public long Id { get; set; }
        public string Product { get; set; }
        public decimal? Quantity { get; set; }
        public string Saccocode { get; set; }
        public string Branch { get; set; }
        public string AuditId { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}