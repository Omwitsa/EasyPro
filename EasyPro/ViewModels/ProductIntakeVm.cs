using EasyPro.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class ProductIntakeVm
    {
        public string Sno { get; set; }
        public string SupName { get; set; }
        public DateTime? TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? Cumlative { get; set; }
        public decimal? Ppu { get; set; }
        public decimal? CR { get; set; }
        public decimal? DR { get; set; }
        public decimal? Balance { get; set; }
        public string Description { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Remarks { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Branch { get; set; }
        public string SaccoCode { get; set; }
        [Display(Name = "Dr Account No")]
        public string DrAccNo { get; set; }
        [Display(Name = "Cr Account No")]
        public string CrAccNo { get; set; }
    }
}
