﻿using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DMilkintake2
    {
        public long Id { get; set; }
        public long Sno { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? Ppu { get; set; }
        public decimal? Pamount { get; set; }
        public string TransTime { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public bool Paid { get; set; }
        public int Lr { get; set; }
        public string Remark { get; set; }
        public string Comment { get; set; }
        public bool? Status1 { get; set; }
        public string Location { get; set; }
        public long? LocalId { get; set; }
        public long? Run { get; set; }
    }
}
