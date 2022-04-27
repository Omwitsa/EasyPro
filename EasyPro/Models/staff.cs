using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class staff
    {
        public string Pin { get; set; }
        public string Name { get; set; }
        public int? Id { get; set; }
        public DateTime? DateEmployeed { get; set; }
        public string Phone { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string AccountNumber { get; set; }
        public string Nhifnumber { get; set; }
        public string Nssfnumber { get; set; }
        public string EmployeeNumber { get; set; }
    }
}
