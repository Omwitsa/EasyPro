using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DBonu
    {
        public string Sno { get; set; }
        public string Name { get; set; }
        public string Bank { get; set; }
        public string Bcode { get; set; }
        public string Branch { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public decimal? Amount { get; set; }
        public string Pby { get; set; }
        public DateTime Auditdatetime { get; set; }
    }
}
