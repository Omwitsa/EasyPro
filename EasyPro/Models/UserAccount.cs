﻿using System;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class UserAccount
    {
        public long Userid { get; set; }
        [Display(Name = "Names")]
        public string UserName { get; set; }
        [Display(Name = "User Code")]
        public string UserLoginIds { get; set; }
        public string Password { get; set; }
        [Display(Name = "User Group")]
        public string UserGroup { get; set; }
        public string PassExpire { get; set; }
        public DateTime? DateCreated { get; set; }
        public long? Superuser { get; set; }
        [Display(Name = "Assign Gl")]
        public string AssignGl { get; set; }
        [Display(Name = "Branch Code")]
        public string Branchcode { get; set; }
        public string Levels { get; set; }
        public bool? Authorize { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
        public string Sign { get; set; }
        public int? Phone { get; set; }
    }
}
