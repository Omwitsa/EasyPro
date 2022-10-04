using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class Deduction
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Accno { get; set; }
        public string SaccoCode { get; set; }
    }
}