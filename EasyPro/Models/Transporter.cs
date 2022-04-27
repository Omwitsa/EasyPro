using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Transporter
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? IdNumber { get; set; }
        public DateTime? Date { get; set; }
        public double? RateCom { get; set; }
        public string BankName { get; set; }
        public string AccNumber { get; set; }
        public string Branch { get; set; }
        public string BranchTransporter { get; set; }
        public string Isfrate { get; set; }
        public int? Frate { get; set; }
        public double? Rate { get; set; }
    }
}
