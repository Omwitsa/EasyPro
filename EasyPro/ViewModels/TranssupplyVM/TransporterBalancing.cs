using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.TranssupplyVM
{
    public class TransporterBalancing
    {
        public TransportersBalancing TransportersBalancing { get; set; }
        public IEnumerable<DTransporter> DTransporter { get; set; }

    }
}
