using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Damagedstock
    {
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Totalamount { get; set; }
        public string Productid { get; set; }
        public string Productname { get; set; }
        public string Username { get; set; }
        public string Priceeach { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public int? BranchCode { get; set; }
        public int? Pocode { get; set; }
        public int? Updated { get; set; }
    }
}
