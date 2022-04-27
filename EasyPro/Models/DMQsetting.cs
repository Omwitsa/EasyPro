using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMQsetting
    {
        public long Id { get; set; }
        public string RejId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public string Dvalue { get; set; }
        public string Reasons { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
    }
}
