using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class AgReceiptVM
    {
        public DateTime? TDate { get; set; }
        public string PCode { get; set; }
        public string Remarks { get; set; }
        public decimal Qua { get; set; }
        public decimal? Amount { get; set; }
        public string SNo { get; set; }
        public string InvoiceNo { get; set; }
    }
    public class AgProductVM
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Openning { get; set; }
        public decimal AddedStock { get; set; }
        public decimal Dispatch { get; set; }
        public decimal StoreBal { get; set; }
        public decimal Sales { get; set; }
        public decimal Bal { get; set; }
        public decimal BPrice { get; set; }
        public decimal SPrice { get; set; }
        public string Branch { get; set; }
        public DateTime Date { get; set; }

    }
}
