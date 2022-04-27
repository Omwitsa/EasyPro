using System;
using System.Collections.Generic;

#nullable disable

namespace EasyPro.Models
{
    public partial class DSupplierDeduc
    {
        public long Id { get; set; }
        public long? Sno { get; set; }
        public DateTime? DateDeduc { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string Period { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Auditid { get; set; }
        public DateTime? Auditdatetime { get; set; }
        public long? Yyear { get; set; }
        public string Remarks { get; set; }
        public string Branch { get; set; }
        public int? Bonus { get; set; }
        public int Status1 { get; set; }
        public int Status2 { get; set; }
        public int Status3 { get; set; }
        public int Status4 { get; set; }
        public int Status5 { get; set; }
        public int Status6 { get; set; }
        public string Branchcode { get; set; }
    }
}
