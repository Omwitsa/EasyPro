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
        [Required]
        public string Factory { get; set; }
        [Required]
        public string Saccocode { get; set; }
        [Required]
        public string CbkCode { get; set; }
        [Required]
        public string OutNumber { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Season { get; set; }
        [Required]
        public string MillerID { get; set; }
        [Required]
        public string Certification { get; set; }
        [Required]
        public DateTime DeliveryDate { get; set; }
        [Required]
        public DateTime MillingDate { get; set; }
        [Required]
        public decimal MillingLoss { get; set; }
        [Required]
        public decimal MoistureParch { get; set; }
        [Required]
        public decimal MostureClean { get; set; }
        [Required]
        public decimal MillingCHarges { get; set; }
        [Required]
        public decimal ExportsCost { get; set; }
        [Required]
        public decimal VatCost { get; set; }
        [Required]
        public DateTime AuditDateTime { get; set; }
       
        public DateTime Date { get; set; }
       
        //public virtual List<MillerProductsWeight> Weights { get; set; } //= new List<MillerProductsWeight>();
     
        public virtual List<MillerProducts> MillerProductslist { get; set; } = new List<MillerProducts>();
        public virtual List<MillerProductsWeight> MillerProductsWeight { get; set; } = new List<MillerProductsWeight>();
        //public virtual List<MillerProducts> MilledProducts { get; set; } = new List<MillerProducts>();
    }
}
