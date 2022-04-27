using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Session
    {
        public long Id { get; set; }
        public string SessionId { get; set; }
        public string Dtime { get; set; }
        public DateTime? DDate { get; set; }
        public string Activity { get; set; }
        public string Status { get; set; }
    }
}
