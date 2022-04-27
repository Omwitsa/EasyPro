using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DShare
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public decimal? Bal { get; set; }
        public string IdNo { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Loc { get; set; }
        public string Type { get; set; }
        public DateTime? TransDate { get; set; }
        public string Pmode { get; set; }
        public bool? Cash { get; set; }
        public string Period { get; set; }
        public decimal? Amnt { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditDateTime { get; set; }
        public decimal Shares { get; set; }
        public DateTime? Regdate { get; set; }
        public string Mno { get; set; }
        public decimal Amount { get; set; }
        public decimal? Premium { get; set; }
        public decimal? Spu { get; set; }
    }
}
