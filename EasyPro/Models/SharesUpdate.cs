using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class SharesUpdate
    {
        public DateTime? Timestamp { get; set; }
        public string UpdatedBy { get; set; }
        public string Period { get; set; }
    }
}
