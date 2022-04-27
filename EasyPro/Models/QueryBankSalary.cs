using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class QueryBankSalary
    {
        public string EmployeeNumber { get; set; }
        public string Name { get; set; }
        public int? Id { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string AccountNumber { get; set; }
        public decimal? NetSalary { get; set; }
        public string Period { get; set; }
    }
}
