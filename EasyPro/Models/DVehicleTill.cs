using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DVehicleTill
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Vehicle { get; set; }
        public string TillNo { get; set; }
        public string Phone { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string UserId { get; set; }
        public string Dr { get; set; }
        public string Cr { get; set; }
        public long? Run { get; set; }
    }
}
