using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DCompany
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public string Location { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Fiscal { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public bool Acc { get; set; }
        public string Motto { get; set; }
        public string SendTime { get; set; }
        public string Smsno { get; set; }
        public decimal? Smscost { get; set; }
        public int Smsport { get; set; }
        public string Period { get; set; }
        [Display(Name = "Supplier Statement Note")]
        public string SupStatementNote { get; set; }
    }
}
