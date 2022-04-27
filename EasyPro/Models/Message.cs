using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Message
    {
        public long Id { get; set; }
        public string Telephone { get; set; }
        public string Content { get; set; }
        public string ProcessTime { get; set; }
        public string MsgType { get; set; }
        public bool? Replied { get; set; }
        public DateTime? DateReceived { get; set; }
        public string Source { get; set; }
        public string Code { get; set; }
    }
}
