using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models.Coffee
{
    using System.Collections.Generic;
    public partial class Customer
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
