using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Period
    {
        public int Id { get; set; }
        public long Period1 { get; set; }
        public string Description { get; set; }
        public long PeriodYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
    }
}
