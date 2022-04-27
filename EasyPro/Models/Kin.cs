using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Kin
    {
        public string MemberNo { get; set; }
        public string KinNames { get; set; }
        public string KinNo { get; set; }
        public string Address { get; set; }
        public string Idno { get; set; }
        public string Relationship { get; set; }
        public string HomeTelNo { get; set; }
        public string Witness { get; set; }
        public int? Percentage { get; set; }
        public string OfficeTelNo { get; set; }
        public DateTime? SignDate { get; set; }
        public string KinSigned { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditTime { get; set; }
        public string Comments { get; set; }
    }
}
