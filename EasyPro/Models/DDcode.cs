using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDcode
    {
        public long Id { get; set; }
        public string Dcode { get; set; }
        public string Description { get; set; }
        public string Dedaccno { get; set; }
        public string Contraacc { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
