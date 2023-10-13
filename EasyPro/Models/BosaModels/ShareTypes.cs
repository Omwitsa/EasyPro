using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models.BosaModels
{
    public class ShareTypes
    {
        [Key]
        public string SharesCode { get; set; }
        public string SharesType { get; set; }
        public string CompanyCode { get; set; }
    }
}