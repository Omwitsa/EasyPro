using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgProducts1
    {
        public string PCode { get; set; }
        public string PName { get; set; }
        public string SNo { get; set; }
        public double? Qin { get; set; }
        public double? Qout { get; set; }
        public DateTime? DateEntered { get; set; }
        public DateTime? LastDUpdated { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
        public double? OBal { get; set; }
        public string SupplierId { get; set; }
        public string Serialized { get; set; }
        public string Unserialized { get; set; }
        public int? Seria { get; set; }
    }
}
