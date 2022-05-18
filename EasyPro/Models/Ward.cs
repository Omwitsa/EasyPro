using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Models
{
    public class Ward
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SubCounty { get; set; }
        public string Contact { get; set; }
        public bool Closed { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
