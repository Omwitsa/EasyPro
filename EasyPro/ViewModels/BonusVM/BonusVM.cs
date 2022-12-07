using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.BonusVM
{
    public class BonusVM
    {
        public string Sno { get; set; }
        public string Deduction { get; set; }
        public DateTime? StartDate { get; set; }
        public string Remark { get; set; }
        public decimal Rate { get; set; }
    }
}
