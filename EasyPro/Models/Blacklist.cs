using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Blacklist
    {
        public long Id { get; set; }
        public string Phoneno { get; set; }
        public string Remarks { get; set; }
    }
}
