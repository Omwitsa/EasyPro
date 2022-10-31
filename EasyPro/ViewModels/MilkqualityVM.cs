using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class MilkqualityVM
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Name { get; set; }
        public DateTime? TransDate { get; set; }
        [Display(Name = "Approximate Kgs")]
        public double? Approximate { get; set; }
        [Display(Name = "Delivered Kgs")]
        public double? Delivered { get; set; }
        [Display(Name = "Transporter")]
        public string Transporter { get; set; }
        [Display(Name = "Organoloptic")]
        public string Organoloptic { get; set; }
        [Display(Name = "Alcohol")]
        public string Alcohol { get; set; }
        public string Antibiotic { get; set; }
        public string Reason { get; set; }
        public string Zone { get; set; }
    }
}
