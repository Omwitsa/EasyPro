using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Matchedreport
    {
        public long Id { get; set; }
        public long? ExId { get; set; }
        public long? BfubId { get; set; }
        public string Sno { get; set; }
        public string Names { get; set; }
        public long? Withdrawn { get; set; }
        public decimal? Amount { get; set; }
        public string Details { get; set; }
        public string Narration { get; set; }
        public string Phoneno { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Valuedate { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
