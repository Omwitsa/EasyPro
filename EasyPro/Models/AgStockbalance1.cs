using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgStockbalance1
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
        public long? SNo { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
