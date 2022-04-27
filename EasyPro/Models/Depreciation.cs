using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Depreciation
    {
        public string Assetcode { get; set; }
        public short? Mmonth { get; set; }
        public long? Yyear { get; set; }
        public decimal? DepreciationAmt { get; set; }
        public string Uuser { get; set; }
    }
}
