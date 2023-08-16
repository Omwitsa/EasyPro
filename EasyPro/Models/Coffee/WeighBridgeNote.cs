using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models.Coffee
{
    public class WeighBridgeNote
    {
        [Key]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime AuditDateTime { get; set; }
        public string Miller { get; set; }
        public string Factory { get; set; }
        public string SaccoCode { get; set; }
        public string VehicleNo { get; set; }
        public string CbkCode { get; set; }
        public string OutreturnNo { get; set; }
        public string FirstWeight { get; set; }
        public string SecondWeight { get; set; }
        public string NetWeight { get; set; }
        public string Category { get; set; }
        public string NoBags { get; set; }
        public string Storage { get; set; }
        public string RecipientName { get; set; }
        public string RecipientIdno { get; set; }
        public string Selection { get; set; }
    }
}
