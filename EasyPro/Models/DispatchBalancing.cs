using System;

namespace EasyPro.Models
{
    public class DispatchBalancing
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Intake { get; set; }
        public decimal? Dispatch { get; set; }
        public decimal? CF { get; set; }
        public decimal? BF { get; set; }
        public decimal? Actuals { get; set; }
        public decimal? Spillage { get; set; }
        public decimal? Rejects { get; set; }
        public decimal? Varriance { get; set; }
        public string Saccocode { get; set; }
    }
}