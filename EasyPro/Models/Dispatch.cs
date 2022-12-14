﻿using System;
using System.ComponentModel.DataAnnotations;

namespace EasyPro.Models
{
    public class Dispatch
    {
        [Key]
        public long Id { get; set; }
        public string Dcode { get; set; }
        [Required]
        public string DName { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Transdate { get; set; }
        public decimal Dispatchkgs { get; set; }
        [Required]
        public decimal TIntake { get; set; }
        public string auditid { get; set; }
        public string Remarks { get; set; }
        public string Branch { get; set; }

    }
}