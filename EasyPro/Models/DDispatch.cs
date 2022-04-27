using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DDispatch
    {
        public long Id { get; set; }
        public DateTime? Transdate { get; set; }
        public string Descrip { get; set; }
        public double Intake { get; set; }
        public double Dipping { get; set; }
        public double Dispatch { get; set; }
        public string Auditid { get; set; }
        public DateTime Auditdate { get; set; }
    }
}
