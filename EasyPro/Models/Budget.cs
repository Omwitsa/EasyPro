using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Budget
    {
        public long Id { get; set; }
        public string Accno { get; set; }
        public long? Mmonth { get; set; }
        public long? Yyear { get; set; }
        public decimal? Actual { get; set; }
        public decimal? Budgetted { get; set; }
        public decimal? Variance { get; set; }
        public DateTime? BudgetDate { get; set; }
        public string SaccoCode { get; set; }
    }
}
