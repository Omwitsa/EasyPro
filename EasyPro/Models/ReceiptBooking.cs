using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class ReceiptBooking
    {
        public long Id { get; set; }
        public string ReceiptNo { get; set; }
        public string Memberno { get; set; }
        public string Ccode { get; set; }
        public string Name { get; set; }
        public DateTime? Transdate { get; set; }
        public decimal? Amount { get; set; }
        public string Chequeno { get; set; }
        public string Ptype { get; set; }
        public string Auditid { get; set; }
        public DateTime Auditdatetime { get; set; }
        public DateTime? Datedeposited { get; set; }
        public bool? Posted { get; set; }
        public string Draccno { get; set; }
        public string Craccno { get; set; }
        public string Ref { get; set; }
        public string RefNo { get; set; }
        public string Companycode { get; set; }
        public string Transactionno { get; set; }
    }
}
