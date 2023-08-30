using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EasyPro.Models.Coffee
{
    public class MillerProducts
    {
        [Key]
        public long Id { get; set; }
        public string Grade { get; set; }
        public decimal Bags { get; set; }
        public decimal Pkts { get; set; }
        public decimal NetKgs { get; set; }
        public decimal PercentageTotal { get; set; }
        public int MillClass { get; set; }
        public string BulkNo { get; set; }
       
    }
}
