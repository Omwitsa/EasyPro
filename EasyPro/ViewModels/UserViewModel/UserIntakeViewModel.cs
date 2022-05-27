using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.Reports
{
    public class UserIntakeViewModel
    {
        internal decimal? totalkgs;

        public string Sno { get; set; }
        public DateTime TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? Ppu { get; set; }
        public decimal? CR { get; set; }
        public decimal? Balance { get; set; }
    }
}
