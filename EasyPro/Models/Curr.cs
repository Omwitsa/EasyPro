using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Curr
    {
        public string CurrCode { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public float? Rateaganistsource { get; set; }
        public short? Decimals { get; set; }
        public string Symbolposition { get; set; }
        public string ThousandSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public string NegativeDisplay { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
