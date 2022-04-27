using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TotalShare
    {
        public int? Sno { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string IdNumber { get; set; }
        public string Location { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Refunds { get; set; }
    }
}
