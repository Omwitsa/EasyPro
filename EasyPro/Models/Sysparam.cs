using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class Sysparam
    {
        public decimal? LoanInterest { get; set; }
        public decimal? ShareInterest { get; set; }
        public int? MinGuarantors { get; set; }
        public int? MaxGuarantors { get; set; }
        public decimal? LoanToShareRatio { get; set; }
        public int? MinLoanPeriod { get; set; }
        public decimal? MinTotShares { get; set; }
        public int? MaxLoans { get; set; }
        public int? MaxContribPeriod { get; set; }
        public decimal? BankInterest { get; set; }
        public int? MinDivPeriod { get; set; }
        public string DefFundId { get; set; }
        public decimal? DeductAmt { get; set; }
        public string SelfGuar { get; set; }
        public decimal? GuarShareRatio { get; set; }
        public string CompanyName { get; set; }
        public string AuditId { get; set; }
        public DateTime? AuditTime { get; set; }
        public int? ShareMaturity { get; set; }
        public int? Withdrawalnotice { get; set; }
        public decimal? InsPremium { get; set; }
        public decimal? ShareCapital { get; set; }
        public double? Depositinterest { get; set; }
        public decimal? RegFees { get; set; }
        public decimal? ByLaws { get; set; }
        public decimal? Loandefaulted { get; set; }
        public decimal? DormancyPeriod { get; set; }
        public short LoanRecoveryOption { get; set; }
    }
}
