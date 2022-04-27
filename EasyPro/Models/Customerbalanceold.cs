using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Customerbalanceold
    {
        public long CustomerBalanceId { get; set; }
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
        public DateTime? AuditDate { get; set; }
        public string ModuleId { get; set; }
        public string AccD { get; set; }
        public DateTime? ValueDate { get; set; }
        public decimal? ActualBalance { get; set; }
        public bool? Cash { get; set; }
        public string Bcode { get; set; }
        public bool? Rebuild { get; set; }
        public string TransNo { get; set; }
        public bool Reconciled { get; set; }
        public string Transfers { get; set; }
        public int? Dregard { get; set; }
    }
}
