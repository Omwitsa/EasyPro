using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Bank
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditTime { get; set; }
        public string AccNo { get; set; }
        public string AccType { get; set; }
        public string BankAccno { get; set; }
    }
}
