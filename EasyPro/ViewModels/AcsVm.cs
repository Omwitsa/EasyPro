namespace EasyPro.ViewModels
{
    public class AcsVm
    {
        public string Name { get; set; }
        public decimal? LastMonthAmount { get; set; }
        public decimal? Last2MonthAmount { get; set; }
        public decimal? Last3MonthAmount { get; set; }
        public decimal? AverageAmount { get; set; }
        public decimal? Shares { get; set; }
        public decimal? FlmdValue { get; set; }
        public decimal? LiableAmount { get; set; }
    }
}
