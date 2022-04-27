using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DHeader
    {
        public long Id { get; set; }
        public string Hcode { get; set; }
        public string Hname { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
