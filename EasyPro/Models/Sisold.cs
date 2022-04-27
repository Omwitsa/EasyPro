using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Sisold
    {
        public int Id { get; set; }
        public string SNo { get; set; }
        public string PCode { get; set; }
        public string Supplier { get; set; }
        public DateTime? Transdate { get; set; }
        public string RNo { get; set; }
    }
}
