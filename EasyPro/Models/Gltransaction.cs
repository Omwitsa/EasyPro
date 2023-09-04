using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Gltransaction
    {
        public long Id { get; set; }
        public long? Local_Id { get; set; }
        public DateTime TransDate { get; set; }
        public decimal Amount { get; set; }
        public string DrAccNo { get; set; }
        public string CrAccNo { get; set; }
        public string DocumentNo { get; set; }
        public string Source { get; set; }
        public string TransDescript { get; set; }
        public DateTime AuditTime { get; set; }
        public string AuditId { get; set; }
        public int? Cash { get; set; }
        public int? Doc_Posted { get; set; }
        public string ChequeNo { get; set; }
        public bool? Dregard { get; set; }
        public string TimeTrans { get; set; }
        public string Transactionno { get; set; }
        public string Module { get; set; }
        public string SaccoCode { get; set; }
        public string Branch { get; set; }
        public string Pmode { get; set; }
        public string Refid { get; set; }
        public bool? Recon { get; set; }
        public int ReconId { get; set; }
        public long? Run { get; set; }
    }
}
