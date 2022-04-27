using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Tmpcashbook
    {
        public long Id { get; set; }
        public DateTime TransDate { get; set; }
        public string DocumentNo { get; set; }
        public decimal Amount { get; set; }
        public string TransType { get; set; }
        public string Source { get; set; }
    }
}
