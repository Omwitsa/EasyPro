namespace EasyPro.Models
{
    public class FLMDLand
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        public string Location { get; set; }
        public string PlotNumber { get; set; }
        public decimal? PlotValue { get; set; }
        public double? TotalAcres { get; set; }
        public double? AcresCrops { get; set; }
        public double? AcresBuilding { get; set; }
        public double? AcresLivestock { get; set; }
        public double? acresUnusedLand { get; set; }
        public string SaccoCode { get; set; }
    }
}
