using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Tbbalance
    {
        public long Id { get; set; }
        public string Accno { get; set; }
        public string Accname { get; set; }
        public decimal Amount { get; set; }
        public string Transtype { get; set; }
        public bool? Closed { get; set; }
        public DateTime? Transdate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string AccType { get; set; }
        public string AccGroup { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public decimal Obal { get; set; }
    }
}
