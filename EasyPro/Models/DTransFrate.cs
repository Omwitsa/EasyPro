using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransFrate
    {
        public long Id { get; set; }
        public string TransCode { get; set; }
        public DateTime? Period { get; set; }
        public double? Rate { get; set; }
        public long? Days { get; set; }
        public decimal Amount { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public decimal Total { get; set; }
        public string Isfrate { get; set; }
    }
}
