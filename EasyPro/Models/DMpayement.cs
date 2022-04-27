using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMpayement
    {
        public long Id { get; set; }
        public int? Sno { get; set; }
        public string PhoneNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string RefNo { get; set; }
    }
}
