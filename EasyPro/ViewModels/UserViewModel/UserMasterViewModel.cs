using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.Reports
{
    public class UserMasterViewModel
    {
        public long Sno { get; set; }
        public DateTime? Regdate { get; set; }
        public string IdNo { get; set; }
        public string Names { get; set; }
        public string AccNo { get; set; }
        [Display(Name = "Bank Name")]
        public string Bcode { get; set; }
        [Display(Name = "Bank Branch")]
        public string Bbranch { get; set; }
        [Display(Name = "Gender")]
        public string Type { get; set; }
        public string Village { get; set; }
        public string Location { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string County { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        //public bool? Trader { get; set; }
        public bool Active { get; set; }
        //public bool Approval { get; set; }
        //public string Branch { get; set; }
        //
        //
        //public string Town { get; set; }
        //public string Email { get; set; }
        //public string TransCode { get; set; }
        //public string Freezed { get; set; }
    }
}
