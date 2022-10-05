using EasyPro.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels
{
    public class EmployeeVm
    {
        public string Empno { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Deduction { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
    }
}
