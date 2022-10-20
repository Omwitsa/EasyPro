using EasyPro.Models;
using EasyPro.ViewModels.FarmersVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.EnquiryVM
{
    public class TransporterBalancingVM
    {
        public string TransCode { get; set; }
        public string TransName { get; set; }
        public DateTime? Date { get; set; }
        public string Quantity { get; set; }
        public string ActualBal { get; set; }
        public string Rejects { get; set; }
        public string Spillage { get; set; }
        public string Varriance { get; set; }
        public string Sno { get; set; }
        public DateTime TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal Qsupplied { get; set; }
    }
}
