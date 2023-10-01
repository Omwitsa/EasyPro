using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class ag_Products45
    {
        [Key]
        public long Id { get; set; }
        public DateTime? Date_Entered { get; set; }
        public string p_code { get; set; }
        public string p_name { get; set; }
        public decimal? Initpprice { get; set; }
        public decimal? Initsprice { get; set; }
        public decimal? Initbal { get; set; }
        public decimal? Newpprice { get; set; }
        public decimal? Newsprice { get; set; }
        public decimal? Newbal { get; set; }
        public string saccocode { get; set; }
        public string Branch { get; set; }
        public string User { get; set; }
        public DateTime? audit_date { get; set; }
        
        
    }
}
