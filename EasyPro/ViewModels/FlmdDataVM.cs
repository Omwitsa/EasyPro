using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class FlmdDataVM
    {
        public IEnumerable<farmerdetail> farmerdetails { get; set; }
    }
    public class farmerdetail
    {
        public string Sno { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public decimal MilkKgs { get; set; }
        public decimal Assets { get; set; }
        public decimal Total { get; set; }
    }
}
