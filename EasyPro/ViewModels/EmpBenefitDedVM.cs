using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class EmpEmployeePayrollVM
    {
        public long Id { get; set; }
        public string? EmpNo { get; set; }
        public string Name { get; set; }
        public decimal? Basic { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? OtherAllowance { get; set; }
        public decimal? Gross { get; set; }
        public decimal? NHIF { get; set; }
        public decimal? NSSF { get; set; }
        public decimal? PAYE { get; set; }
        public decimal? STORE { get; set; }
        public decimal? ADVANCE { get; set; }
        public decimal? LOAN { get; set; }
        public decimal? MILKRECOVERY { get; set; }
        public decimal? SAVINGS { get; set; }
        public decimal? OTHERDED { get; set; }
        public decimal? TOTALDED { get; set; }
        public decimal? NETPAY { get; set; }
        public DateTime? ENDINGMONTH { get; set; }
        public string User { get; set; }
    }

    public class EmpBenefitDedVM
    {
        public long Id { get; set; }
        public string EmpNo { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }

    }
}
