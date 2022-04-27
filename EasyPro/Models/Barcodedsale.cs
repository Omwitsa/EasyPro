using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Barcodedsale
    {
        public int Serialid { get; set; }
        public string Serialcode { get; set; }
        public int? Units { get; set; }
        public string Stockname { get; set; }
        public decimal? Sellingprice { get; set; }
        public decimal? Totalamount { get; set; }
        public DateTime? Transdate { get; set; }
        public string Paymenttype { get; set; }
        public string Chequeno { get; set; }
        public string Batchno { get; set; }
    }
}
