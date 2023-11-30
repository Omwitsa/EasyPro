using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class ag_ProductConverted
    {
        [Key]
        public long Id { get; set; }
        public DateTime? TDate { get; set; }
        public string p_code { get; set; }
        public string p_name { get; set; }
        public decimal? Quantity { get; set; }
        public string p_code_converted { get; set; }
        public string p_name_converted { get; set; }
        public decimal? Quantity_converted { get; set; }
        public string user_id { get; set; }
        public DateTime? audit_date { get; set; }
        public string Branch { get; set; }
        public string saccocode { get; set; }
    }
}
