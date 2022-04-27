using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Gltransactions2
    {
        public long Id { get; set; }
        public string Accname { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Available { get; set; }
        public string Accno { get; set; }
        public string Transdescription { get; set; }
        public string ChequeNo { get; set; }
        public string Transtype { get; set; }
        public DateTime? Transdate { get; set; }
        public string AuditId { get; set; }
        public int Cash { get; set; }
    }
}
