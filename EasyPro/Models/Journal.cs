using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Journal
    {
        public long Jvid { get; set; }
        public string Vno { get; set; }
        public string Accno { get; set; }
        public string Name { get; set; }
        public string Naration { get; set; }
        public string Type { get; set; }
        public string Memberno { get; set; }
        public string Sharetype { get; set; }
        public string Loanno { get; set; }
        public decimal? Amount { get; set; }
        public string Transtype { get; set; }
        public string Auditid { get; set; }
        public DateTime? Transdate { get; set; }
        public DateTime Auditdate { get; set; }
        public bool Posted { get; set; }
        public DateTime? Posteddate { get; set; }
    }
}
