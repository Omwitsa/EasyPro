using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class DeductionType
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Accno { get; set; }
        public string SaccoCode { get; set; }
    }
}