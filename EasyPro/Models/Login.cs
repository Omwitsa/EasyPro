using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Login
    {
        public string Level { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string Branch { get; set; }
    }
}
