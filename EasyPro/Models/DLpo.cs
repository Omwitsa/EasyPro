using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DLpo
    {
        public long Id { get; set; }
        public long Pno { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Serial { get; set; }
        public string RefNo { get; set; }
        public DateTime? Auditdatetieme { get; set; }
        public string Auditid { get; set; }
        public string Remarks { get; set; }
        public string Vendor { get; set; }
    }
}
