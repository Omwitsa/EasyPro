using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models.BosaModels
{
    public class Member
    {
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string IDNo { get; set; }
        public string CompanyCode { get; set; }
    }
}
