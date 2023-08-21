using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace EasyPro.Models.Coffee
{
    public partial class MarketingDeduct
    {
        [Key]
        public long Id { get; set; }
        public string saccocode { get; set; }
        public string Factory { get; set; }
        public string Remarks { get; set; }
        public string one { get; set; }
        public string second { get; set; }
        public decimal Amount { get; set; }
        public virtual List<Marketing> Marketing { get; set; }= new List<Marketing>();
        public DateTime AuditDateTime { get; set; }
        public DateTime Date { get; set; }
    }
}
