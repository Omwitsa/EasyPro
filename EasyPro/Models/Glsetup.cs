using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class Glsetup
    {
        [Key]
        public long Glid { get; set; }
        public string GlCode { get; set; }
        public string GlAccName { get; set; }
        public string AccNo { get; set; }
        public string GlAccType { get; set; }
        public string GlAccGroup { get; set; }
        public string GlAccMainGroup { get; set; }
        public string NormalBal { get; set; }
        public string GlAccStatus { get; set; }
        public decimal OpeningBal { get; set; }
        public decimal CurrentBal { get; set; }
        public decimal? Bal { get; set; }
        public decimal? CurrCode { get; set; }
        public string AuditOrg { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Curr { get; set; }
        public decimal? Actuals { get; set; }
        public decimal? Budgetted { get; set; }
        public DateTime? TransDate { get; set; }
        public bool? IsSubLedger { get; set; }
        public string AccCategory { get; set; }
        public decimal NewGlopeningBal { get; set; }
        public DateTime NewGlopeningBalDate { get; set; }
        public string Branch { get; set; }
        public string Hcode { get; set; }
        public string Mcode { get; set; }
        public string Hname { get; set; }
        public string Header { get; set; }
        public string Mheader { get; set; }
        public int? Iorder { get; set; }
        public int? Border { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public bool IsRearning { get; set; }
        public bool Issuspense { get; set; }
        public long? Run { get; set; }
        public string saccocode { get; set; }
    }
}
