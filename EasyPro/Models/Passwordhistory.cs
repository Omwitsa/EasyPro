using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Passwordhistory
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public DateTime? TransDate { get; set; }
        public long? Ephnum { get; set; }
    }
}
