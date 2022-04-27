using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Drange
    {
        public string Dcode { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal Rate { get; set; }
        public bool Percentage { get; set; }
        public DateTime Audittime { get; set; }
        public string Auditid { get; set; }
    }
}
