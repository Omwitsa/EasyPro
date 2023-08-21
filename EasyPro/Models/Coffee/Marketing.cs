using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace EasyPro.Models.Coffee
{
    public partial class Marketing
    {
        [Key]
        public long Id { get; set; }
        public string OutTurn { get; set; }
        public string BulkNo { get; set; }
        public string Marks { get; set; }
        public string GrowerCode { get; set; }
        public string Council { get; set; }
        public string Mill { get; set; }
        public string Grade { get; set; }
        public string Bags { get; set; }
        public string Pockets { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }
        public string Buyer { get; set; }
        public decimal Gross { get; set; }
        public decimal AveragePrice { get; set; }
        [ForeignKey("MarketingDeduct")]//very important
        public long MarketingDeductId { get; set; }
        public virtual MarketingDeduct MarketingDeduct { get; set; }
    }
}
