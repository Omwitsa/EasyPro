using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPayroll
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public decimal? Transport { get; set; }
        public decimal? Agrovet { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? extension { get; set; }
        public decimal? SMS { get; set; }
        public decimal? Tmshares { get; set; }
        public decimal? Fsa { get; set; }
        public decimal? Hshares { get; set; }
        public decimal? Advance { get; set; }
        public decimal? Others { get; set; }
        public decimal? Tdeductions { get; set; }
        public double? KgsSupplied { get; set; }
        public decimal? Gpay { get; set; }
        public decimal? Npay { get; set; }
        public int? Yyear { get; set; }
        public int? Mmonth { get; set; }
        public string Bank { get; set; }
        public string AccountNumber { get; set; }
        public string Bbranch { get; set; }
        public string Trader { get; set; }
        public string Sbranch { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndofPeriod { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Mainaccno { get; set; }
        public string Transportaccno { get; set; }
        public string Agrovetaccno { get; set; }
        public string Aiaccno { get; set; }
        public string Tmsharesaccno { get; set; }
        public string Fsaaccno { get; set; }
        public string Hsharesaccno { get; set; }
        public string Advanceaccno { get; set; }
        public string Otheraccno { get; set; }
        public string Netaccno { get; set; }
        public decimal? Subsidy { get; set; }
        public decimal? Cbo { get; set; }
        public string IdNo { get; set; }
        public decimal Tchp { get; set; }
        public decimal Midmonth { get; set; }
        public decimal? Deduct12 { get; set; }
        public decimal? Mpesa { get; set; }
        public string SaccoCode { get; set; }
        public string Branch { get; set; }
        public decimal? CLINICAL { get; set; }
        public decimal? AI { get; set; }
        public decimal? Tractor { get; set; }
        public decimal? CurryForward { get; set; }
        public decimal? MIDPAY { get; set; }
    }
}
