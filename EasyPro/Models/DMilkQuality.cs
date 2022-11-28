using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMilkQuality
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public DateTime? RejDate { get; set; }
        public double? ApproxKgs { get; set; }
        public double? DeKgs { get; set; }
        public double? ContCpcity { get; set; }
        public string Ttransporter { get; set; }
        public string Conttype { get; set; }
        public string TransMode { get; set; }
        public string Organoleptic { get; set; }
        public int? Rez { get; set; }
        public double? Lact { get; set; }
        public double? PlateCount { get; set; }
        public string Alcohol { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public double? Pcheck { get; set; }
        public double? Dramsk { get; set; }
        public string RejReasons { get; set; }
        public string Auditid { get; set; }
        public string code { get; set; }
        public string Branch { get; set; }
        public string Antibiotic { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
