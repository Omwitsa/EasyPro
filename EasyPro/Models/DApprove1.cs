using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DApprove1
    {
        public long Id { get; set; }
        public string Rno { get; set; }
        public bool Approved { get; set; }
        public string Description { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
