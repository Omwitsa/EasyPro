using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Tran
    {
        public string TransCode { get; set; }
        public string Sno { get; set; }
        public string Rate { get; set; }
        public string Startdate { get; set; }
        public string Active { get; set; }
        public string DateInactivate { get; set; }
        public string Auditid { get; set; }
        public string Auditdatetime { get; set; }
        public string Isfrate { get; set; }
    }
}
