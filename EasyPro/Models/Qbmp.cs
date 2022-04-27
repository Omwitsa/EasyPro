using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Qbmp
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Canno { get; set; }
        public string Tpc { get; set; }
        public string Tsc { get; set; }
        public string Alc { get; set; }
        public string Anr { get; set; }
        public string Remarks { get; set; }
        public string Pscore { get; set; }
        public string Cname { get; set; }
        public DateTime Auditdatetime { get; set; }
    }
}
