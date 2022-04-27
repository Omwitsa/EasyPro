using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Tempmemberstatement
    {
        public string MemberNo { get; set; }
        public string RefNo { get; set; }
        public string Description { get; set; }
        public int? TransCode { get; set; }
        public decimal? Principal { get; set; }
        public decimal? Interest { get; set; }
        public decimal? MonthlyContr { get; set; }
        public decimal? Total { get; set; }
    }
}
