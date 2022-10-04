using System;

namespace EasyPro.Models
{
    public class Employee
    {
        public long Id { get; set; }
        public string EmpNo { get; set; }
        public string MemberNo { get; set; }
        public string SaccoCode { get; set; }
        public string Surname { get; set; }
        public string Othernames { get; set; }
        public string IDNO { get; set; }
        public string Category { get; set; }
        public string Department { get; set; }
        public string EmpStatus { get; set; }
        public string nationality { get; set; }
        public string gender { get; set; }
        public DateTime? DOB { get; set; }
        public string maritalstatus { get; set; }
        public string RELIGION { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string NSSFCode { get; set; }
        public string NHIFCode { get; set; }
        public string PinNo { get; set; }
        public string eposition { get; set; }
        public string BankAccNo { get; set; }
        public string Bank { get; set; }
        public DateTime? AUDITTIME { get; set; }
        public DateTime? EmpDate { get; set; }
    }
}