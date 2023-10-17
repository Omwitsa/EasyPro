using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPro.Models.BosaModels
{
    public class SContrib
    {
        public long Id { get; set; }
        public string MemberNo { get; set; }
        public decimal? Amount { get; set; }
        public string CompanyCode { get; set; }
        public string Remarks { get; set; }
        public string ReceiptNo { get; set; }
        public string Sharescode { get; set; }
        [NotMapped]
        public decimal? Paid { get; set; }
    }
}
