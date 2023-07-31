using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace EasyPro.Models
{
    public partial class DPrice
    {
        public long Id { get; set; }
        public string Products { get; set; }
        public DateTime Edate { get; set; }
        public decimal? Price { get; set; }
        public string SaccoCode { get; set; }
        [Display(Name = "Subsidy Quantity")]
        public decimal? SubsidyQty { get; set; }
        [Display(Name = "Subsidy Price")]
        public decimal? SubsidyPrice { get; set; }
        [Display(Name = "Dr Account No")]
        public string DrAccNo { get; set; }
        [Display(Name = "Cr Account No")]
        public string CrAccNo { get; set; }
        [Display(Name = "Transport Dr Account No")]
        public string TransportDrAccNo { get; set; }
        [Display(Name = "Transport Cr Account No")]
        public string TransportCrAccNo { get; set; }

        // [NotMapped]
        //public List<DBranchProduct> Productcollection { get; set; }
        [NotMapped]
        public List<SelectListItem> Productt { get; set; }
    }
}
