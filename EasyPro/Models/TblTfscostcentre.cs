using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class TblTfscostcentre
    {
        public int Cstctrid { get; set; }
        public string Cstctrname { get; set; }
        public string Cstctrdescription { get; set; }
        public string DexchTtype { get; set; }
        public string DexchOrgcode { get; set; }
        public string Synchro { get; set; }
        public string FactcodeFk { get; set; }
    }
}
