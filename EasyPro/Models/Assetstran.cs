using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Assetstran
    {
        public int Assettransid { get; set; }
        public int? Year { get; set; }
        public string Assetcode { get; set; }
        public string Assetname { get; set; }
        public double? DepVal { get; set; }
        public decimal? AmountdepVal { get; set; }
        public decimal? Nrv { get; set; }
        public int? Quaters { get; set; }
        public DateTime? Transdate { get; set; }
        public int? Mmonth { get; set; }
        public bool? Posted { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
