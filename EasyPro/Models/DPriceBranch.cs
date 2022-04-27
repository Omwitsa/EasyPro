using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPriceBranch
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public string Branch { get; set; }
        public bool? Active { get; set; }
    }
}
