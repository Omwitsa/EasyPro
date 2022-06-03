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
        public DateTime? Edate { get; set; }
        public decimal? Price { get; set; }
        public string SaccoCode { get; set; }
        public string SubsidyQty { get; set; }
        public decimal? SubsidyPrice { get; set; }
        public string DrAccNo { get; set; }
        public string CrAccNo { get; set; }

        // [NotMapped]
        //public List<DBranchProduct> Productcollection { get; set; }
        [NotMapped]
        public List<SelectListItem> Productt { get; set; }
    }
}
