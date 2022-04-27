using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DSmscompany
    {
        public long Id { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
