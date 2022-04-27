using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class EasyMaPolicy
    {
        public long Id { get; set; }
        public long PassLength { get; set; }
        public long PassExpire { get; set; }
        public long EnforcePassHistory { get; set; }
        public long Ephnum { get; set; }
        public long Minpexpire { get; set; }
        public long Pcomplexity { get; set; }
        public long Spreverse { get; set; }
    }
}
