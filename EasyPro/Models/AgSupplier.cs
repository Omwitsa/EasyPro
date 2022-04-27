using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class AgSupplier
    {
        public long Id { get; set; }
        public string SuppId { get; set; }
        public string SuppName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string EmailAdd { get; set; }
    }
}
