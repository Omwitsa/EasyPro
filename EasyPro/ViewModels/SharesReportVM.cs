using EasyPro.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class SharesReportVM
    {
        public string Sno { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string IDNo { get; set; }
        public string PhoneNo { get; set; }
        public decimal? Shares { get; set; }
        public string Branch { get; set; }
    }
}
