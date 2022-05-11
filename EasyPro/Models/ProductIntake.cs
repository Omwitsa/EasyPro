﻿using EasyPro.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class ProductIntake
    {
        public long Id { get; set; }
        [Required]
        public long Sno { get; set; }
        [Required]
        public DateTime? TransDate { get; set; }
        public string ProductType { get; set; }
        public decimal? Qsupplied { get; set; }
        public decimal? Ppu { get; set; }
        public decimal? CR { get; set; }
        public decimal? DR { get; set; }
        public decimal? Balance { get; set; }
        public string Description { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Remarks { get; set; }
        public string AuditId { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public string Branch { get; set; }
    }
}
