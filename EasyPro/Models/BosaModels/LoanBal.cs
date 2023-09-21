namespace EasyPro.Models.BosaModels
{
    public class LoanBal
    {
        public int Id { get; set; }
        public string LoanNo { get; set; }
        public string LoanCode { get; set; }
        public string MemberNo { get; set; }
        public decimal Balance { get; set; }
        public decimal? Installments { get; set; }
        public decimal? RepayRate { get; set; }
        public string Companycode { get; set; }
    }
}