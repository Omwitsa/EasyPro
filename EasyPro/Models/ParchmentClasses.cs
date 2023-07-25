using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class ParchmentClasses
    {
        [Key]
        public long Id { get; set; }
        public string PClasses { get; set; }
        public string saccocode { get; set; }
        public string Factory { get; set; }
    }
}
