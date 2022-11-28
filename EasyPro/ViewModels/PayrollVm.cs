using System;

namespace EasyPro.ViewModels
{
    public class PayrollVm
    {
        public string Sno { get; set; }
        public string Names { get; set; }
        public string PhoneNo { get; set; }
        public string IdNo { get; set; }
        public string Bank { get; set; }
        public string AccNo { get; set; }
        public string Branch { get; set; }
        public double? Quantity { get; set; }
        public decimal? GrossPay { get; set; }
        public decimal? Transport { get; set; }
        public decimal? Registration { get; set; }
        public decimal? Advance { get; set; }
        public decimal? Others { get; set; }
        public decimal? Netpay { get; set; }
    }

    public class PayrollPeriod
    {
        public DateTime EndDate { get; set; }
    }
}
