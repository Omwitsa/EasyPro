using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class Transporter
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? IdNumber { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public double? RateCom { get; set; }
        public string BankName { get; set; }
        public string AccNumber { get; set; }
        [Required]
        public string Branch { get; set; }
        public string BranchTransporter { get; set; }
        public string Isfrate { get; set; }
        public int? Frate { get; set; }
        public double? Rate { get; set; }
    }
}
