﻿using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DBranch
    {
        public long Id { get; set; }
        public string Bcode { get; set; }
        public string Bname { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public long? LocalId { get; set; }
        public long? Run { get; set; }
    }
}
