using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Gledger
    {
        public DateTime? Transdate { get; set; }
        public string Source { get; set; }
        public decimal? Debits { get; set; }
        public decimal? Credits { get; set; }
        public decimal? AccBal { get; set; }
        public string Chequeno { get; set; }
        public string Description { get; set; }
        public string Glname { get; set; }
        public bool Recon { get; set; }
        public string Auditid { get; set; }
    }
}
