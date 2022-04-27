using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutsale
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Accno { get; set; }
        public long? Run { get; set; }
    }
}
