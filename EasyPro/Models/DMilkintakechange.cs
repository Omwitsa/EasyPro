using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMilkintakechange
    {
        public long Id { get; set; }
        public string Branch { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Userid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
