using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDebtorsparchase
    {
        public string Debtor { get; set; }
        public string Name { get; set; }
        public double? Kgs { get; set; }
        public int? Price { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        public string Branch { get; set; }
        public DateTime? Date { get; set; }
    }
}
