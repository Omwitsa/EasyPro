using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.EnquiryVM
{
    public class TransportersEnquiryVM
    {
        public IEnumerable<DTmpTransEnquery> DTmpTransEnquery { get; set; }
        public IEnumerable<DTransporter> DTransporters { get; set; }
        public FilterVm FilterVm { get; set; }
    }
}
