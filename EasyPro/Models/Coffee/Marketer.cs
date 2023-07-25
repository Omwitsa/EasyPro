using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models.Coffee
{
    public partial class Marketer
    {
        [Key]
        public long Id { get; set; }
        public string saccocode { get; set; }
        public string Factory { get; set; }
        public string MarketerName { get; set; }
        public string Category { get; set; }
        public string Grade { get; set; }
        public string Class { get; set; }
        public decimal Kgs { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime AuditDateTime { get; set; }
    }
}
