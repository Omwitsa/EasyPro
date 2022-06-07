using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTmpTransEnquery
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime? TransDate { get; set; }
        public string Sno { get; set; }
        public double? Amount { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Dr { get; set; }
        public decimal? Bal { get; set; }
        public string sacco { get; set; }
    }
}
