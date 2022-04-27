using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Param
    {
        public long Paramid { get; set; }
        public string Arcontrol { get; set; }
        public string Apcontrol { get; set; }
        public string Acontrol { get; set; }
        public string REarnings { get; set; }
        public string PDep { get; set; }
        public string ADep { get; set; }
        public float? Vat { get; set; }
        public string VatC { get; set; }
        public string GenerateReceiptno { get; set; }
    }
}
