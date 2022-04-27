using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DInvoice
    {
        public long Id { get; set; }
        public string InvId { get; set; }
        public string Rno { get; set; }
        public string Vendor { get; set; }
        public DateTime? InvDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? Amount { get; set; }
        public string Desc { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public bool Paid { get; set; }
    }
}
