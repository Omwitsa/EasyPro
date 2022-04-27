using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DRegistration
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public DateTime? Transdate { get; set; }
        public decimal Amount { get; set; }
        public decimal Bal { get; set; }
        public string Transdescription { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdate { get; set; }
        public string Mno { get; set; }
        public long Toledgers { get; set; }
        public DateTime? Datepostedtoledger { get; set; }
        public string Userledger { get; set; }
        public long? LocalId { get; set; }
        public long? Run { get; set; }
    }
}
