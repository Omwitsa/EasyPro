using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPrice2
    {
        [Key]
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public string Branch { get; set; }
        public string SaccoCode { get; set; }
        public string Product { get; set; }
        public bool Active { get; set; }
    }
}
