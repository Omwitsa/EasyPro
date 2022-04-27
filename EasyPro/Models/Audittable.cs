using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Audittable
    {
        public long Auditid { get; set; }
        public string UserName { get; set; }
        public DateTime? LoginDate { get; set; }
        public string LoginTime { get; set; }
        public string UserTransaction { get; set; }
        public string LogoffTime { get; set; }
        public string Moduleid { get; set; }
    }
}
