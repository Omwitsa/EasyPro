using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Serialno
    {
        public int Serialid { get; set; }
        public string Serialno1 { get; set; }
        public string PCode { get; set; }
        public double? Used { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
