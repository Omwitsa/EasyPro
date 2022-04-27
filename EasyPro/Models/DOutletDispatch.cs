using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DOutletDispatch
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Vehicle { get; set; }
        public string OutletName { get; set; }
        public double? Quantity { get; set; }
        public long? Run { get; set; }
    }
}
