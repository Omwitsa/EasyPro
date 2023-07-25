using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.ViewModels
{
    public class PayrollVm
    {
        public string Sno { get; set; }
        public string Names { get; set; }
        public string PhoneNo { get; set; }
        public string IdNo { get; set; }
        public string Bank { get; set; }
        public string AccNo { get; set; }
        public string Branch { get; set; }
        public double? Quantity { get; set; }
        public decimal? GrossPay { get; set; }
        public decimal? Registration { get; set; }
        public decimal? Transport { get; set; }
        [Display(Name = "Loan")]
        public decimal? Fsa { get; set; }
        public decimal? Advance { get; set; }
        public decimal? CurryForward { get; set; }
        public decimal? Clinical { get; set; }
        public decimal? AI { get; set; }
        public decimal? Tractor { get; set; }
        public decimal? Extension { get; set; }
        public decimal? SMS { get; set; }
        public decimal? Agrovet { get; set; }
        public decimal? Bonus { get; set; }
        [Display(Name = "Shares")]
        public decimal? Hshares { get; set; }
        public decimal? Others { get; set; }
        public decimal? Netpay { get; set; }
    }

    public class PayrollPeriod
    {
        public DateTime EndDate { get; set; }
    }
}
