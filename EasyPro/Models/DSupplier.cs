using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace EasyPro.Models
{
    [Table("DSupplier")]
    public partial class DSupplier
    {
        public long Id { get; set; }
        public long? LocalId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Sno should be grater than 1")]
        public string Sno { get; set; }
        public DateTime? Regdate { get; set; }
        public string IdNo { get; set; }
        public string Names { get; set; }
        public string AccNo { get; set; }
        public string Bcode { get; set; }
        public string Bbranch { get; set; }
        [Display(Name = "Gender")]
        public string Type { get; set; }
        public string Village { get; set; }
        public string Location { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string County { get; set; }
        public bool? Trader { get; set; }
        public bool Active { get; set; }
        public bool Approval { get; set; }
        public string Branch { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string Email { get; set; }
        public string TransCode { get; set; }
        public string Sign { get; set; }
        public string Photo { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Scode { get; set; }
        public bool Loan { get; set; }
        public string Compare { get; set; }
        public string Isfrate { get; set; }
        public string Frate { get; set; }
        public string Rate { get; set; }
        public int? Hast { get; set; }
        public string Br { get; set; }
        public string Mno { get; set; }
        public long? Branchcode { get; set; }
        public string HasNursery { get; set; }
        public long? Notrees { get; set; }
        public string Aarno { get; set; }
        public DateTime? Tmd { get; set; }
        public long Landsize { get; set; }
        public long Thcpactive { get; set; }
        public long Thcppremium { get; set; }
        public string Status { get; set; }
        public long? Status2 { get; set; }
        public long? Status3 { get; set; }
        public long? Status4 { get; set; }
        public long? Status5 { get; set; }
        public string Status6 { get; set; }
       
        public string Types { get; set; }
        public DateTime? Dob { get; set; }
        public string Freezed { get; set; }
        public string Mass { get; set; }
        public long? Status1 { get; set; }
        public long? Run { get; set; }
        public string Zone { get; set; }
    }
}
