using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class User
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string UserPassword { get; set; }
        public string IsSuperUser { get; set; }
    }
}
