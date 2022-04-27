using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class SwiftMessage
    {
        public long Id { get; set; }
        public int SaccoCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public int Msgstatus { get; set; }
        public DateTime Auditdate { get; set; }
    }
}
