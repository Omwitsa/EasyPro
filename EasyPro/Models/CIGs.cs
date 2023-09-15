using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class CIGs
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string saccocode { get; set; }
    }
}
