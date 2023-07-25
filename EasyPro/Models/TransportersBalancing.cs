using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Models
{
    public class TransportersBalancing
    {
        [Key]
        public long Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Transporter { get; set; }
        public string Quantity { get; set; }
        public string ActualBal { get; set; }
        public string Rejects { get; set; }
        public string Spillage { get; set; }
        public string Varriance { get; set; }
        public string Code { get; set; }
        public string Branch { get; set; }

    }
}
