using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Tmpaginganalysis
    {
        public string LoanNo { get; set; }
        public string MemberNo { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public string Companycode { get; set; }
        public string Companyname { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Repayrate { get; set; }
        public DateTime? Lastdate { get; set; }
    }
}
