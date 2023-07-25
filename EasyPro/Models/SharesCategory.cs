namespace EasyPro.Models
{
    public class SharesCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string SaccoCode { get; set; }
    }
}