using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class MessageConfigs
    {
        [Key]
        public long Id { get; set; }
        public string? username { get; set; }
        public string? apiKey { get; set; }
        public string? saccocode { get; set; }
        public bool Closed { get; set; }
        public string? SenderId { get; set; }
        
        
    }
}
