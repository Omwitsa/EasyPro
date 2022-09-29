using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Drawnstock
    {
        public long Id { get; set; }
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
        public string Branch { get; set; }
        public int? Updated { get; set; }
        public string Buying { get; set; }
        public int? Ai { get; set; }
        public string Commission { get; set; }
        public string saccocode { get; set; }
    }
}
