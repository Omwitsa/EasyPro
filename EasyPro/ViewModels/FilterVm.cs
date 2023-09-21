using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class FilterVm
    {
        [Display(Name = "Date From")]
        public DateTime? DateFrom { get; set; }
        [Display(Name = "Date To")]
        public DateTime? DateTo { get; set; }
        [Display(Name = "Branch Name")]
        public string Branch { get; set; }
        [Display(Name = "Debtor")]
        public string Debtor { get; set; }
        public string Transporter { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public string County { get; set; }
        public string Zone { get; set; }


    }

    public class ProductBalancingFilterVm
    {
        public DateTime? Date { get; set; }
        public string TCode { get; set; }
    }
}
