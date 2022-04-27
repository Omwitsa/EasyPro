using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TblMenu
    {
        public long Id { get; set; }
        public string Menu { get; set; }
        public string Alias { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? RegDate { get; set; }
    }
}
