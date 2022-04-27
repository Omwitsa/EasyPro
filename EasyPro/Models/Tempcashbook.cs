using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Tempcashbook
    {
        public string MCode { get; set; }
        public string MName { get; set; }
        public decimal Amount { get; set; }
        public decimal DMonth { get; set; }
        public decimal DYear { get; set; }
        public string TransType { get; set; }
        public DateTime TransDate { get; set; }
        public string DocumentNo { get; set; }
    }
}
