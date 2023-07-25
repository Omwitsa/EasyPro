using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class ParchmentFactoryPricing
    {
        [Key]
        public long Id { get; set; }
        public string Factory { get; set; }
        public string Parchment { get; set; }
        public string Grading { get; set; }
        public string Class { get; set; }
        public decimal Price { get; set; }
        public string saccocode { get; set; }
        public DateTime Date { get; set; }
        public string Year { get; set; }
        public DateTime AuditDateTime { get; set; }
    }
}
