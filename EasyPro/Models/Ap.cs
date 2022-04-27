using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Ap
    {
        public long Pid { get; set; }
        public long PNo { get; set; }
        public string Paidto { get; set; }
        public decimal? PAmount { get; set; }
        public string Curr { get; set; }
        public string Purpose { get; set; }
        public string Accno { get; set; }
        public string Particulars { get; set; }
        public DateTime? PDate { get; set; }
        public string FAccNo { get; set; }
        public double? Exchangerate { get; set; }
        public string CheckedBy { get; set; }
        public string Chequeno { get; set; }
        public string Chequestatus { get; set; }
        public DateTime? DateCollected { get; set; }
        public string Approvedby { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string Receivedby { get; set; }
        public string Idno { get; set; }
        public string Paymentmode { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string PAccno { get; set; }
        public bool? Posted { get; set; }
        public string InvNo { get; set; }
        public decimal? Vat { get; set; }
        public string VatAccno { get; set; }
    }
}
