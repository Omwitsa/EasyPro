using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class UserAccount
    {
        public long Userid { get; set; }
        public string UserName { get; set; }
        public string UserLoginIds { get; set; }
        public string Password { get; set; }
        public string UserGroup { get; set; }
        public string PassExpire { get; set; }
        public DateTime? DateCreated { get; set; }
        public long? Superuser { get; set; }
        public string AssignGl { get; set; }
        public string Branchcode { get; set; }
        public string Levels { get; set; }
        public bool? Authorize { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
        public string Sign { get; set; }
        public int? Phone { get; set; }
    }
}
