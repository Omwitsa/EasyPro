using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransportersPayRoll
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public double? QntySup { get; set; }
        public decimal? Amnt { get; set; }
        public decimal? Subsidy { get; set; }
        public decimal? GrossPay { get; set; }
        public decimal? Agrovet { get; set; }
        public decimal? Tmshares { get; set; }
        public decimal? Fsa { get; set; }
        public decimal? Hshares { get; set; }
        public decimal? Advance { get; set; }
        public decimal? extension { get; set; }
        public decimal? SMS { get; set; }
        public decimal? Others { get; set; }
        public decimal? Totaldeductions { get; set; }
        public decimal? NetPay { get; set; }
        public string BankName { get; set; }
        public string AccNo { get; set; }
        public string Branch { get; set; }
        public int? Mmonth { get; set; }
        public int? Yyear { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndPeriod { get; set; }
        public double? Rate { get; set; }
        public int? Frate { get; set; }
        public string Isfrate { get; set; }
        public string SaccoCode { get; set; }
        public string BBranch { get; set; }
        public decimal? CLINICAL { get; set; }
        public decimal? AI { get; set; }
        public decimal? Tractor { get; set; }
        public decimal? VARIANCE { get; set; }
        public decimal? CurryForward { get; set; }
        public decimal? MIDPAY { get; set; }
    }
}
