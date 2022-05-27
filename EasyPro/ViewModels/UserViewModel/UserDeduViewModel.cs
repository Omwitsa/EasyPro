using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.Reports
{
    public class UserDeduViewModel
    {
        internal decimal? totalkgs;

        public string Sno { get; set; }
        public DateTime TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? DR { get; set; }
    }
}
