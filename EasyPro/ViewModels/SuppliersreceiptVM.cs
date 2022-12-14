using EasyPro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.SuppliersreceiptVM
{
    public class Suppliersreceipt
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        
        public DateTime TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal Qsupplied { get; set; }
        public decimal Cumalative { get; set; }
        public IEnumerable<ProductIntakeVm> ProductIntakeVm { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Town { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

    }
}
