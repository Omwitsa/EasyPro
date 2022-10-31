using System;

namespace EasyPro.Models
{
    public class milkcontrol2
    {
        public long Id { get; set; }
        public decimal? Intake { get; set; }
        public DateTime? transdate { get; set; }
        public decimal? SQuantity { get; set; }
        public decimal? Reject { get; set; }
        public decimal? cfa { get; set; }
        public decimal? Spillage { get; set; }
        public decimal? Bf { get; set; }
        public string auditid { get; set; }
        public decimal? FromStation { get; set; }
        public decimal? Tostation { get; set; }
        public string code { get; set; }
        public string Branch { get; set; }
    }
}