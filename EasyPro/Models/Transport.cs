using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Transport
    {
        public string Code { get; set; }
        public int? SupplierNumber { get; set; }
        public string SupplierName { get; set; }
        public double? Rate { get; set; }
        public DateTime? Date { get; set; }
        public string Branch { get; set; }
        public int? Frate { get; set; }
        public string Isfrate { get; set; }
    }
}
