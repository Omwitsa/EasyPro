using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DChangepro
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double? Quantity { get; set; }
        public string User { get; set; }
        public double? Pprice { get; set; }
        public double? Sprice { get; set; }
        public double? Balance { get; set; }
        public string Branch { get; set; }
    }
}
