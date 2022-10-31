using System;

namespace EasyPro.Models
{
    public class Milktransfer
    {
        public long Id { get; set; }
        public DateTime? Transdate { get; set; }
        public decimal? fromStation { get; set; }
        public decimal? Tostation { get; set; }
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }
        public string auditid { get; set; }
        public decimal? Intake { get; set; }
        public string Code { get; set; }
    }
}