using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgPaging
    {
        public long Id { get; set; }
        public string Pcode { get; set; }
        public DateTime? Ldate { get; set; }
        public DateTime? Ltdate { get; set; }
        public long? Dy { get; set; }
        public DateTime? Auditdate { get; set; }
        public string Audit { get; set; }
        public string Grade { get; set; }
    }
}
