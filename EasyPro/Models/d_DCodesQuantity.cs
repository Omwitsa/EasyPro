using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Models
{
    public class d_DCodesQuantity
    {
        [Key]
        public long Id { get; set; }
        public string? Month { get; set; }
        public decimal? TotalSales { get; set; }
        public string saccocode { get; set; }
    }
}
