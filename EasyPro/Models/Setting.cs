using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Setting
    {
        public DateTime? Edate { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string Period { get; set; }
    }
}
