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
        public DateTime? Date { get; set; }
        public string SaccoCode { get; set; }
        public string Branch { get; set; }
    }
}