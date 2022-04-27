using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutletbranch
    {
        public long Id { get; set; }
        public string Bcode1 { get; set; }
        public string Bname1 { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public int? Till { get; set; }
        public int? PhoneNo { get; set; }
        public string Dr { get; set; }
        public string Cr { get; set; }
        public long? Run { get; set; }
    }
}
