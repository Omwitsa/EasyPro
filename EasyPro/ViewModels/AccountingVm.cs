using System;

namespace EasyPro.ViewModels
{
    public class AccountingVm
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

        public class BudgetVm
        {
            public long? Period { get; set; }
            public DateTime EndDate { get; set; }
            public decimal? BudgettedAmount { get; set; }
        }

        public class BudgetFilter
        {
            public string AccNo { get; set; }
            public DateTime Period { get; set; }
            public decimal Amount { get; set; }
            public bool Fixed { get; set; }
            public string Monthly { get; set; }
        }

        public class ComparisonVm
        {
            public string AccNo { get; set; }
            public string AccName { get; set; }
            public decimal? BudgettedAmount { get; set; }
            public decimal? ActualAmount { get; set; }
            public decimal? Variance { get; set; }
            public decimal? Percentage { get; set; }
        }
    }
}
