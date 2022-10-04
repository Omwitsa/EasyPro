using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Usergroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool CashBook { get; set; }
        public bool Transactions { get; set; }
        public bool Activity { get; set; }
        public bool Reports { get; set; }
        public bool Setup { get; set; }
        public bool Files { get; set; }
        public bool Accounts { get; set; }
        public bool AccountsPay { get; set; }
        public bool FixedAssets { get; set; }
        public bool SaccoReports { get; set; }
        public string SaccoCode { get; set; }
    }
}
