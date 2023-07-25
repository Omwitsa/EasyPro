using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyPro.Models
{
    public class milkcontrol2
    {
        public long Id { get; set; }
        public decimal? Intake { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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
        [NotMapped]
        public string Total { get; set; }
    }
}