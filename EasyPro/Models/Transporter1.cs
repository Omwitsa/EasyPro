using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Transporter1
    {
        public double? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double? IdNumber { get; set; }
        public DateTime? Date { get; set; }
        public double? RateCom { get; set; }
        public string BankName { get; set; }
        public string AccNumber { get; set; }
        public string Branch { get; set; }
        public string BranchTransporter { get; set; }
    }
}
