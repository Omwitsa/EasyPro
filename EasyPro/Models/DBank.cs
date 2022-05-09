using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DBank
    {
        public long Id { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditTime { get; set; }
        public string BankAccNo { get; set; }
        public string Accno { get; set; }
        public string AccType { get; set; }
    }
}
