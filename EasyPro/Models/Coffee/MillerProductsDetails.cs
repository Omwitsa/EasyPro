using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPro.Models.Coffee
{
    public class MillerProductsDetails
    {
        [Key]
        public long Id { get; set; }
        public string Factory { get; set; }
        public string Saccocode { get; set; }
        public string CbkCode { get; set; }
        public string OutNumber { get; set; }
            
        public string Category { get; set; }
        public string Season { get; set; }
        public string MillerID { get; set; }
        public string Certification { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime MillingDate { get; set; }
        public decimal MillingLoss { get; set; }
        public decimal MoistureParch { get; set; }
        public decimal MostureClean { get; set; }
        public decimal MillingCHarges { get; set; }
        public decimal ExportsCost { get; set; }
        public decimal VatCost { get; set; }
        public DateTime AuditDateTime { get; set; }
        public DateTime Date { get; set; }
        public virtual List<MillerProductsWeight> Weights { get; set; } = new List<MillerProductsWeight>();
        public virtual List<MillerProducts> MilledProducts { get; set; } = new List<MillerProducts>();
    }
}
