using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutsalesb
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public double? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Apaid { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public long? Run { get; set; }
    }
}
