using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDailySumm
    {
        public DateTime? TransDate { get; set; }
        public decimal? Qnty { get; set; }
        public long Sno { get; set; }
        public decimal? Ppu { get; set; }
        public string Type { get; set; }
    }
}
