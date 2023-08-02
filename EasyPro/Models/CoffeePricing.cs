using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class CoffeePricing
    {
        [Key]
        public long Id { get; set; }
        public string saccocode { get; set; }
        public string Factory { get; set; }
        public string Category { get; set; }
        public string Grade { get; set; }
        public string Class { get; set; }
        public decimal TotalKgs { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime AuditDateTime { get; set; }
    }
}
