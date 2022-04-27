using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class StaffPayroll
    {
        public string Pin { get; set; }
        public string Name { get; set; }
        public decimal? Basic { get; set; }
        public decimal? HouseAllowance { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string AccountNumber { get; set; }
        public decimal? Overtime { get; set; }
        public decimal? MAllowance { get; set; }
        public decimal? OtherBenefits { get; set; }
        public decimal? GrossPay { get; set; }
        public decimal? Nhif { get; set; }
        public decimal? Nssf { get; set; }
        public decimal? Sdr { get; set; }
        public decimal? Advances { get; set; }
        public int? Id { get; set; }
        public string Nssfnumber { get; set; }
        public string Nhifnumber { get; set; }
        public string EmpNumber { get; set; }
        public decimal? Paye { get; set; }
        public decimal? OtherDeductions { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? SpfEmployer { get; set; }
        public decimal? NssfEmployer { get; set; }
        public string PayPoint { get; set; }
        public string Period { get; set; }
    }
}
