using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgMoringaintake
    {
        public long Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public decimal? System { get; set; }
        public decimal? Actual { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Auditedatetime { get; set; }
    }
}
