using System;
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
        public DateTime Transdate { get; set; }
        public decimal Dispatchkgs { get; set; }
        [Required]
        public decimal TIntake { get; set; }
        public string auditid { get; set; }

    }
}