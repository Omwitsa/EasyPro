using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class Parchment
    {
        [Key]
        public long Id { get; set; }
        public string PName { get; set; }
        public string saccocode { get; set; }
        public string Factory { get; set; }
    }
}
