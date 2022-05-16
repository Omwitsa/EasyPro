using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Models
{
    public class SupplierEnquiry
    {
        public long Id { get; set; }
        public long? LocalId { get; set; }
        public long Sno { get; set; }
        public DateTime? Regdate { get; set; }
        public string IdNo { get; set; }
        public string Names { get; set; }
        public string AccNo { get; set; }
        public string Bcode { get; set; }
        public string Bbranch { get; set; }
        public string Type { get; set; }
        public string Village { get; set; }
        public string Location { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public bool? Trader { get; set; }
        public bool? Active { get; set; }
        public bool? Approval { get; set; }
        public string Branch { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string Email { get; set; }
        public string TransCode { get; set; }
        public string Sign { get; set; }
        public string Photo { get; set; }
    }
    public class intake
    {
        public long Id { get; set; }
        public long Sno { get; set; }
        [DisplayName("TransDate")]
        [DisplayFormat(ApplyFormatInEditMode=true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? Ppu { get; set; }
        public decimal? CR { get; set; }
        public decimal? DR { get; set; }
        public decimal? Balance { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Branch { get; set; }
    }
}