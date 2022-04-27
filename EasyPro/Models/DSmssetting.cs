using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DSmssetting
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Dr { get; set; }
        public decimal? Cr { get; set; }
        public decimal? Balanace { get; set; }
    }
}
