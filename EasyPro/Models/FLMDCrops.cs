namespace EasyPro.Models
{
    public class FLMDCrops
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string CashCrops { get; set; }
        public decimal? CashCropsValue { get; set; }
        public string ConsumerCrops { get; set; }
        public decimal? ConsumerCropsValue { get; set; }
        public string Vegetables { get; set; }
        public decimal? VegetablesValue { get; set; }
        public string AnimalFeeds { get; set; }
        public decimal? AnimalFeedsValue { get; set; }
        public string SaccoCode { get; set; }
    }
}