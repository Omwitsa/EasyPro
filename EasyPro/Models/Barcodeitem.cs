using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Barcodeitem
    {
        public int Serialid { get; set; }
        public string Serialcode { get; set; }
        public string Stockcode { get; set; }
        public string Stockname { get; set; }
        public string Units { get; set; }
        public string Category { get; set; }
        public decimal? Saleprice { get; set; }
        public int? Vatrate { get; set; }
        public decimal? Purchaseprice { get; set; }
        public DateTime? Receiptdate { get; set; }
    }
}
