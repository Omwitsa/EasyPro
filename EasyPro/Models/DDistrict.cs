using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDistrict
    {
        public long Id { get; set; }
        public string Dcode { get; set; }
        public string Dname { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
