using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Cub
    {
        public long Cuid { get; set; }
        public string MemberNo { get; set; }
        public string AccNo { get; set; }
        public string Idno { get; set; }
        public string Payno { get; set; }
        public string AccountName { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public string Transdescription { get; set; }
        public decimal? AvailableBalance { get; set; }
        public decimal? Commission { get; set; }
        public DateTime? Transdate { get; set; }
        public string Vno { get; set; }
        public string ChequeNo { get; set; }
        public string Transtype { get; set; }
        public string Picture { get; set; }
        public string Signature { get; set; }
        public string Period { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdate { get; set; }
        public string Moduleid { get; set; }
        public bool? Active { get; set; }
        public bool? Closed { get; set; }
        public bool? Frozen { get; set; }
        public string Picture1 { get; set; }
        public decimal? Actualbalance { get; set; }
        public decimal? Unitmarked { get; set; }
        public decimal? Excess { get; set; }
        public int? Graceperiod { get; set; }
        public DateTime? Limitexpirydate { get; set; }
        public string Branchcode { get; set; }
        public bool? Main { get; set; }
        public string Controlacc { get; set; }
        public string BranchAccno { get; set; }
        public string MainAccno { get; set; }
        public string Curr { get; set; }
        public bool? Status { get; set; }
        public string MainAccHeader { get; set; }
        public bool? NoticeG { get; set; }
        public DateTime? Don { get; set; }
        public DateTime? Edon { get; set; }
        public decimal? Adbal { get; set; }
        public decimal? Hs { get; set; }
        public decimal? Ordshares { get; set; }
        public decimal? Loans { get; set; }
        public string Tgno1 { get; set; }
        public string Tgno2 { get; set; }
        public string Tgno3 { get; set; }
        public string Sex { get; set; }
        public string Category { get; set; }
        public decimal? Nhif { get; set; }
        public string Nomi1 { get; set; }
        public string Nomi1id { get; set; }
        public string Nomi2 { get; set; }
        public string Nomi2id { get; set; }
        public string Nomi3 { get; set; }
        public string Nomi3id { get; set; }
        public string Sig1 { get; set; }
        public string Sig1id { get; set; }
        public string Sig2 { get; set; }
        public string Sig2id { get; set; }
        public string Sig3 { get; set; }
        public string Sig3id { get; set; }
        public string Sig4 { get; set; }
        public string Sig4id { get; set; }
        public string Notes { get; set; }
        public bool? Ledger { get; set; }
        public bool? Issubledger { get; set; }
        public bool? Hassubledgers { get; set; }
        public DateTime? Regdate { get; set; }
    }
}
