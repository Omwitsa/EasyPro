using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class CashbookTransaction
    {
        public long CashId { get; set; }
        public DateTime? Transdate { get; set; }
        public string Vno { get; set; }
        public string Transtype { get; set; }
        public decimal? Amount { get; set; }
        public string Chequeno { get; set; }
        public string Accno { get; set; }
        public string Transdescription { get; set; }
        public int? IdNo { get; set; }
        public int? Reconciled { get; set; }
        public string Pd { get; set; }
        public string Transby { get; set; }
    }
}
