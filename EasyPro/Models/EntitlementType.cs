namespace EasyPro.Models
{
    public class EntitlementType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Taxable { get; set; }
        public string SaccoCode { get; set; }
    }
}