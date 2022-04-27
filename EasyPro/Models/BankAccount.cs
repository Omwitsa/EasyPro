using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class BankAccount
    {
        public long Id { get; set; }
        public DateTime? Transdate { get; set; }
        public string AccName { get; set; }
        public string Pvcno { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Piro { get; set; }
        public string Chequeno { get; set; }
    }
}
