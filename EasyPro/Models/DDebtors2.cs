using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDebtors2
    {
        public long Id { get; set; }
        public string Dcode { get; set; }
        public string Dname { get; set; }
        public string CertNo { get; set; }
        public string Locations { get; set; }
        public DateTime? TregDate { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
