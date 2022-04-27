using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgClosingStock
    {
        public long Id { get; set; }
        public long Pcode { get; set; }
        public string Pname { get; set; }
        public decimal Pprice { get; set; }
        public decimal OpenStock { get; set; }
        public decimal ChangeInStock { get; set; }
        public decimal ClosingStock { get; set; }
        public decimal Qty { get; set; }
        public DateTime Transdate { get; set; }
        public string Auditid { get; set; }
        public decimal Opstock { get; set; }
        public DateTime Opdate { get; set; }
        public decimal? Sales { get; set; }
        public decimal? Purchase { get; set; }
    }
}
