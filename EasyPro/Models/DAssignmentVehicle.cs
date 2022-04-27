using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DAssignmentVehicle
    {
        public long Id { get; set; }
        public string AccnoV { get; set; }
        public string Vehicle { get; set; }
        public string ExpenseAcc { get; set; }
        public string ExpeLedger { get; set; }
        public bool? Active { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
    }
}
