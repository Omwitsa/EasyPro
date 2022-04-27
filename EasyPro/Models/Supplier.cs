using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Supplier
    {
        public short SupplierNumber { get; set; }
        public DateTime? DateBegan { get; set; }
        public string IdNumber { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string RegisteredBy { get; set; }
        public string Types { get; set; }
        public string Location { get; set; }
        public string Trader { get; set; }
        public string BranchSupplier { get; set; }
        public string Names { get; set; }
    }
}
