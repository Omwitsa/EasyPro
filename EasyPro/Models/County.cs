using System;

namespace EasyPro.Models
{
    public class County
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public bool Closed { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
