using EasyPro.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.TransportersVM
{
    public class TransporterVm
    {
        public string TransCode { get; set; }
        public string TransName { get; set; }
        public DateTime? TdateDeduc { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
    }
}
