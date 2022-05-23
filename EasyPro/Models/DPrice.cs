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
        [Key]
        public long Id { get; set; }
        public string Products { get; set; }
        public DateTime? Edate { get; set; }
        public decimal? Price { get; set; }
        public string SaccoCode { get; set; }

        // [NotMapped]
        //public List<DBranchProduct> Productcollection { get; set; }
        [NotMapped]
        public List<SelectListItem> Productt { get; set; }
    }
}
