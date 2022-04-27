using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DReceipt
    {
        public long Id { get; set; }
        public string Rno { get; set; }
        public string Vendor { get; set; }
        public string DelNo { get; set; }
        public double? Qnty { get; set; }
        public DateTime? TransDate { get; set; }
        public string Remarks { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
