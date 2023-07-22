using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models.Coffee
{
    public partial class Pulping_Drying
    {
        [Key]
        public long Id { get; set; }
        public string Factory { get; set; }
        public string saccocode { get; set; }
        public string Category { get; set; }
        public decimal Kgs { get; set; }
        public DateTime Date { get; set; }
        public DateTime AuditDateTime { get; set; }
    }
}
