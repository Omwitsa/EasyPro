using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DProductProcess
    {
        public DateTime? Date { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double? Quantity { get; set; }
        public string Branch { get; set; }
        public string Remarks { get; set; }
    }
}
