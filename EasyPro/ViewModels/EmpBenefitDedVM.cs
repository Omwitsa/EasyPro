using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class EmpBenefitDedVM
    {
        public long Id { get; set; }
        public string EmpNo { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
    }
}
