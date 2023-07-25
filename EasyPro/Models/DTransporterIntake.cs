using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Models
{
    public class DTransporterIntake
    {
        public long Id { get; set; }
        public string TransCode { get; set; }
        public decimal? ActualKg { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public DateTime? AuditDate { get; set; }
        public string SaccoCode { get; set; }
        public string Branch { get; set; }
        public string Posted { get; set; }
    }
}