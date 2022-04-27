using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AssetsRegister
    {
        public long Asid { get; set; }
        public string Assetcode { get; set; }
        public string Assetname { get; set; }
        public double Deprate { get; set; }
        public string Accno { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CurrentValue { get; set; }
        public DateTime Pdate { get; set; }
        public string SerialNo { get; set; }
        public string ContraAccno { get; set; }
    }
}
