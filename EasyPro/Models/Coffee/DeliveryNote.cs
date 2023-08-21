using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EasyPro.Models.Coffee
{
    public partial class DeliveryNote
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string NoteNo { get; set; }
        public string GrowersCode { get; set; }
        public string CoffeType { get; set; }
        public string VehicleNo { get; set; }
        public decimal? NoBags { get; set; }
        public string Driver { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public DateTime? AuditDate { get; set; }
        public string Branch { get; set; }
        public string saccocode { get; set; }
    }
}
