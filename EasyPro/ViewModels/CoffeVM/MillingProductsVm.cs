using EasyPro.Models;
using EasyPro.Models.Coffee;
using System.Collections.Generic;

namespace EasyPro.ViewModels.CoffeVM
{
    public class MillingProductsVm
    {
        public MillerProductsDetails MillerProd { get; set; }
        public List<MillerProductsWeight> Weights { get; set; } 
        public List<MillerProducts> MilledProducts { get; set; } 

    }
}
