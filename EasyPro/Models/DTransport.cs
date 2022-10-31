using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DTransport
    {
        public long Id { get; set; }
        public string TransCode { get; set; }
        public long? Sno { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? Startdate { get; set; }
        public bool Active { get; set; }
        public DateTime? DateInactivate { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Isfrate { get; set; }
        public string saccocode { get; set; }
        public string producttype { get; set; }
        public string Branch { get; set; }
        public string Zone { get; set; }
    }
}
