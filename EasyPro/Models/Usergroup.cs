using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class Usergroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Registration { get; set; }
        public bool Activity { get; set; }
        public bool Reports { get; set; }
        public bool Setup { get; set; }
        public bool Files { get; set; }
        public bool Accounts { get; set; }
        public bool Deductions { get; set; }
        public bool Staff { get; set; }
        public bool Store { get; set; }
        public bool SaccoReports { get; set; }
        public string SaccoCode { get; set; }
    }
}
