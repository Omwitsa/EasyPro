using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class CustomerBalance
    {
        public long Customerbalanceid { get; set; }
        public string CustomerNo { get; set; }
        public string Idno { get; set; }
        public string PayrollNo { get; set; }
        public string AccName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string AccNo { get; set; }
        public string TransDescription { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal? Commission { get; set; }
        public string ChequeNo { get; set; }
        public string Period { get; set; }
        public bool? Posted { get; set; }
        public bool? Locked { get; set; }
        public string TransType { get; set; }
        public bool? Status { get; set; }
        public string Vno { get; set; }
        public string Auditid { get; set; }
        public DateTime Auditdate { get; set; }
        public string Moduleid { get; set; }
        public string Accd { get; set; }
        public DateTime? Valuedate { get; set; }
        public decimal? Actualbalance { get; set; }
        public bool? Cash { get; set; }
        public string Bcode { get; set; }
        public bool? Rebuild { get; set; }
        public string Transno { get; set; }
        public bool? Reconciled { get; set; }
        public string Transfers { get; set; }
        public int? Dregard { get; set; }
        public string SCode { get; set; }
    }
}
