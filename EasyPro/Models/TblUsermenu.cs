using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TblUsermenu
    {
        public long Id { get; set; }
        public string Userloginid { get; set; }
        public string Menu { get; set; }
        public DateTime? Regdate { get; set; }
        public bool Enable { get; set; }
    }
}
