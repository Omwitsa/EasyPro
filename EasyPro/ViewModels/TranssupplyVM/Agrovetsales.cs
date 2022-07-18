using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.TranssupplyVM
{
    public class Agrovetsales
    {
        public AgReceipt AgReceipt { get; set; }
        public IEnumerable<DTransporter> DTransporter { get; set; }
        public IEnumerable<DSupplier> DSuppliers { get; set; }
        public IEnumerable<AgProduct> AgProductobj { get; set; }

    }
}
