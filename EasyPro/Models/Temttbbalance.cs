using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Temttbbalance
    {
        public long Id { get; set; }
        public DateTime TransDate { get; set; }
        public string DocumentNo { get; set; }
        public decimal Amount { get; set; }
        public string AccNo { get; set; }
        public string TransType { get; set; }
        public decimal? Budget { get; set; }
    }
}
