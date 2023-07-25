using DocumentFormat.OpenXml.Wordprocessing;
using EasyPro.Constants;
using System.ComponentModel.DataAnnotations;
using System;

namespace EasyPro.Models
{
    public class StandingOrder
    {
        public long Id { get; set; }
        public string Sno { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TransDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Amount { get; set; }
        public int? Duration { get; set; }
        public string Description { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string SaccoCode { get; set; }
        public string Zone { get; set; }
        public bool Status { get; set; }
    }
}
