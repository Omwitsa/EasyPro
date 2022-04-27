using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TempGlTransaction
    {
        public DateTime Transdate { get; set; }
        public string Transtype { get; set; }
        public decimal Amount { get; set; }
        public string Chequeno { get; set; }
        public string DocumentNo { get; set; }
        public bool? Recon { get; set; }
        public string AccNo { get; set; }
        public string TransDescript { get; set; }
    }
}
