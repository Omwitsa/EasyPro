using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models
{
    public partial class DeliveryNote
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string NoteNo { get; set; }
        public string GrowersCode { get; set; }
        public string CoffeType { get; set; }
        public double? Qin { get; set; }
        public double? Qout { get; set; }
        public DateTime? LastDUpdated { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
        public double? OBal { get; set; }
        public string SupplierId { get; set; }
        public string Serialized { get; set; }
        public string Unserialized { get; set; }
        public int? Seria { get; set; }
        public decimal? Pprice { get; set; }
        public decimal? Sprice { get; set; }
        public string Branch { get; set; }
        public string Draccno { get; set; }
        public string Craccno { get; set; }
        public int? Ai { get; set; }
        public DateTime? Expirydate { get; set; }
        public long? Run { get; set; }
        public int? Process1 { get; set; }
        public int? Process2 { get; set; }
        public string Remarks { get; set; }
        public string saccocode { get; set; }
    }
}
