using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class BankRecon
    {
        public string AccNo { get; set; }
        public DateTime ReconDate { get; set; }
        public DateTime OpeningBalDate { get; set; }
        public decimal OpeningBal { get; set; }
        public decimal Receipts { get; set; }
        public decimal Payments { get; set; }
        public decimal Unpresented { get; set; }
        public decimal UnCredited { get; set; }
        public decimal DirectCredits { get; set; }
        public decimal DirectDebits { get; set; }
        public decimal StatementBal { get; set; }
        public decimal CashBookBal { get; set; }
        public long ReconId { get; set; }
    }
}
