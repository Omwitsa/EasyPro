using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Audittrans1
    {
        public long Transid { get; set; }
        public string Transtable { get; set; }
        public string Transdescription { get; set; }
        public DateTime Transdate { get; set; }
        public decimal Amount { get; set; }
        public string Auditid { get; set; }
        public DateTime Audittime { get; set; }
    }
}
