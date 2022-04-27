using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TchpRate
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public decimal? Rate { get; set; }
        public string Type { get; set; }
    }
}
