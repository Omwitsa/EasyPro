using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgSale
    {
        public string Pcode { get; set; }
        public string Pname { get; set; }
        public decimal Openning { get; set; }
        public decimal Purchases { get; set; }
        public decimal Sales { get; set; }
        public decimal? Balance { get; set; }
        public string Branch { get; set; }
        public DateTime? Date { get; set; }
    }
}
