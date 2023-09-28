using System;

namespace EasyPro.Models
{
    public class SocietyStandingOrder
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string GlAcc { get; set; }
        public string ContraAcc { get; set; }
        public bool HasRate { get; set; }
        public decimal Amount { get; set; }
        public string SaccoCode { get; set; }
        public string AuditId { get; set; }
    }
}
