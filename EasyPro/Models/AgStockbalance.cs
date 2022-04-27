using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgStockbalance
    {
        public int Trackid { get; set; }
        public string PCode { get; set; }
        public string Productname { get; set; }
        public decimal? Openningstock { get; set; }
        public decimal? Changeinstock { get; set; }
        public double? Stockbalance { get; set; }
        public DateTime? Transdate { get; set; }
        public int? Companyid { get; set; }
        public string Subclass { get; set; }
        public string RNo { get; set; }
        public string SNo { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
        public decimal Pprice { get; set; }
        public decimal Sprice { get; set; }
        public double Rlevel { get; set; }
        public DateTime Auditdate1 { get; set; }
        public string Branch { get; set; }
        public int? Ai { get; set; }
        public DateTime? Expirydate { get; set; }
        public int? Process1 { get; set; }
        public int? Process2 { get; set; }
        public string Remarks { get; set; }
    }
}
