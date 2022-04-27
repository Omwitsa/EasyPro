using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgOpbal
    {
        public string PCode { get; set; }
        public string PName { get; set; }
        public DateTime? Transdate { get; set; }
        public double? Bal { get; set; }
    }
}
