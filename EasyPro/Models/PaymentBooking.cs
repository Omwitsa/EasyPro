using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class PaymentBooking
    {
        public long Id { get; set; }
        public string VoucherNo { get; set; }
        public string Memberno { get; set; }
        public string Ccode { get; set; }
        public string Name { get; set; }
        public string PayeeDesc { get; set; }
        public string Particulars { get; set; }
        public DateTime? Transdate { get; set; }
        public decimal? Amount { get; set; }
        public string Chequeno { get; set; }
        public string Ptype { get; set; }
        public string Auditid { get; set; }
        public DateTime Audittime { get; set; }
        public DateTime? Datedeposited { get; set; }
        public bool? Posted { get; set; }
        public string Transactionno { get; set; }
    }
}
