using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Login1
    {
        public long Id { get; set; }
        public string UserLoginIds { get; set; }
        public string Ttime { get; set; }
        public string WkStation { get; set; }
        public string LogedOut { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}
