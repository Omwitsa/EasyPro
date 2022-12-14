using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransporter
    {
        public long Id { get; set; }
        [Required]
        public string TransCode { get; set; }
        public string TransName { get; set; }
        [Required]
        public string CertNo { get; set; }
        public string Locations { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TregDate { get; set; }
        public string Email { get; set; }
        [Required]
        public string Phoneno { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
        public double? Subsidy { get; set; }
        public string Accno { get; set; }
        public string Bcode { get; set; }
        public string Bbranch { get; set; }
        public bool Active { get; set; }
        public string Tbranch { get; set; }
        public string Auditid { get; set; }
        public string Auditdatetime { get; set; }
        public string Isfrate { get; set; }
        public double? Rate { get; set; }
        public string Canno { get; set; }
        public bool Tt { get; set; }
        public string ParentT { get; set; }
        public long Ttrate { get; set; }
        public string Br { get; set; }
        public string Freezed { get; set; }
        public string PaymenMode { get; set; }
        public string Route { get; set; }
        
    }
}
