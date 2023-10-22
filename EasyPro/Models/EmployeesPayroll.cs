using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class EmployeesPayroll
    {
        [Key]
        public long Id { get; set; }
        public string EmpNo { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? Basic { get; set; }
        public decimal? OtherAllowance { get; set; }
        public decimal? Gross { get; set; }
        public decimal? NHIF { get; set; }
        public decimal? NSSF { get; set; }
        public decimal? PAYE { get; set; }
        public decimal? OTHERS { get; set; }
        public decimal? STORE { get; set; }
        public decimal? OTHERDED { get; set; }
        public decimal? TOTALDED { get; set; }
        public decimal? NETPAY { get; set; }
        public DateTime ENDINGMONTH { get; set; }
        public DateTime? auditdatetime { get; set; }
        public string Audituser { get; set; }
        public string SaccoCode { get; set; }
    }
}