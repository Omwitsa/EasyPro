using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDcode
    {
        public long Id { get; set; }
        public string Dcode { get; set; }
        public string Description { get; set; }
        [Display(Name ="Dr")]
        public string Dedaccno { get; set; }
        [Display(Name = "Cr")]
        public string Contraacc { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
