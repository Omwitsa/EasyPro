using System;

namespace EasyPro.ViewModels
{
    public class JournalVm
    {
        public string DocumentNo { get; set; }
        public string TransDescript { get; set; }
        public DateTime TransDate { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public decimal Bal { get; set; }
    }

    public class JournalFilter
    {
        public string AccNo { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
