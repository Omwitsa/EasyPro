using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class MilkControlVm
    {
        public long Id { get; set; }
        public decimal? Intake { get; set; }
        public DateTime? transdate { get; set; }
        public decimal? SQuantity { get; set; }
        public decimal? Reject { get; set; }
        public decimal? cfa { get; set; }
        public decimal? Spillage { get; set; }
        public decimal? Bf { get; set; }
        public string auditid { get; set; }
        public decimal? FromStation { get; set; }
        public decimal? Tostation { get; set; }
        public string code { get; set; }
        public string Branch { get; set; }
        public decimal? Varriance { get; set; }
    }
}
