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
        public decimal? Ai { get; set; }
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
        public DateTime? EndPeriod { get; set; }
        public double? Rate { get; set; }
        public int? Frate { get; set; }
        public string Isfrate { get; set; }
        public string SaccoCode { get; set; }
        public string BBranch { get; set; }
    }
}
