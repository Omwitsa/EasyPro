using System;
using System.Collections.Generic;
using EasyPro.Models;
using System.Linq;
using System.Threading.Tasks;
using EasyPro.ViewModels.TransportersVM;
using Microsoft.EntityFrameworkCore;

namespace EasyPro.ViewModels.TransportersVM
{
    public class TransportersVM
    {
        public DTransportDeduc DTransportDeduc { get; set; }
        public IEnumerable<DTransporter> DTransporter { get; set; }
    }
}
