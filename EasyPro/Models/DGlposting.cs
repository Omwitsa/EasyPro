using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DGlposting
    {
        public long Id { get; set; }
        public int Mmonth { get; set; }
        public int Yyear { get; set; }
        public decimal? Namount { get; set; }
        public bool Posted { get; set; }
    }
}
