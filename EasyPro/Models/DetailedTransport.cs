using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DetailedTransport
    {
        public short? SupplierNumber { get; set; }
        public double? Qnty { get; set; }
        public string Code { get; set; }
        public string Period { get; set; }
        public string Branch { get; set; }
        public double? Rate { get; set; }
    }
}
