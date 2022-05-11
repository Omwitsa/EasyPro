using EasyPro.Models;
using EasyPro.ViewModels.FarmersVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.FarmersVM
{
    public class FarmersVM
    {
        public ProductIntake ProductIntake { get; set; }
        public IEnumerable<DSupplier> DSuppliers { get; set; }
    }
}
