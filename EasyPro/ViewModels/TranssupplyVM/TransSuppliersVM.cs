using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.TranssupplyVM
{
    public class TransSuppliersVM
    {
        public long Id { get; set; }
        public string TransCode { get; set; }
        public string TransName { get; set; }
        public string Sno { get; set; }
        public string Names { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? Startdate { get; set; }
        //public bool? Active { get; set; }
        public DateTime? DateInactivate { get; set; }
        public string Morning { get; set; }
    }
}
