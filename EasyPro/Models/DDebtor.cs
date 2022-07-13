using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDebtor
    {
        public long Id { get; set; }
        public string Dcode { get; set; }
        [Required]
        public string Dname { get; set; }
        [Required]
        public string CertNo { get; set; }
        public string Locations { get; set; }
        [Required]
        public DateTime? TregDate { get; set; }
        public string Email { get; set; }
        public string Phoneno { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
        public double? Subsidy { get; set; }
        public string Accno { get; set; }
        public string Bcode { get; set; }
        public string Bbranch { get; set; }
        public bool? Active { get; set; }
        public string Tbranch { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public decimal? Price { get; set; }
        public string AccDr { get; set; }
        public string AccCr { get; set; }
        public double? Crate { get; set; }
        public string Drcess { get; set; }
        public string Crcess { get; set; }
        public bool Capp { get; set; }
    }
}
