using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Models
{
    public class IndexVM
    {
        public List<ProductIntake> intakes { get; set; }
        public List<DSupplier> suppliers { get; set; }
    }
}
