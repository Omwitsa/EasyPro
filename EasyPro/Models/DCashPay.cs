using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DCashPay
    {
        public long Id { get; set; }
        public string PayId { get; set; }
        public DateTime? TransDate { get; set; }
        public string Cracc { get; set; }
        public string Dracc { get; set; }
        public string Payee { get; set; }
        public string Descr { get; set; }
        public string Vno { get; set; }
        public decimal? Amount { get; set; }
    }
}
