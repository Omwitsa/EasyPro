using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPro.Models.Coffee
{
    public class MillerProductsWeight
    {
        [Key]
        public long Id { get; set; }
        public int WeightNoteNo { get; set; }
        public int Bags { get; set; }
        public int GrossWeight { get; set; }
    }
}
