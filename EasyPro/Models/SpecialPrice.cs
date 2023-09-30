using System;

namespace EasyPro.Models
{
    public class SpecialPrice
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Code { get; set; }
        public int Month { get; set; }
        public bool IsFarmer { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public string Branch { get; set; }
        public string saccocode { get; set; }
    }
}
