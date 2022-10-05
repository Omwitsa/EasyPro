using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class EmployeesDed
    {
        [Key]
        public long Id { get; set; }
        public string Empno { get; set; }
        public DateTime? Date { get; set; }
        public string Deduction { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public string AuditId { get; set; }
        public string saccocode { get; set; }
    }
}