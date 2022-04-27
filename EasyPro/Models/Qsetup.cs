using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Qsetup
    {
        public long Id { get; set; }
        public string Quality { get; set; }
        public string Irate { get; set; }
        public DateTime Auditdate { get; set; }
        public string Auditid { get; set; }
    }
}
