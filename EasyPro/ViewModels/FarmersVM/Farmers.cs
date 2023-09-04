using EasyPro.Models;
using EasyPro.ViewModels.FarmersVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.ViewModels.FarmersVM
{
    public class FarmersVM
    {
        public ProductIntake ProductIntake { get; set; }
        public EmployeesDed EmployeesDed { get; set; }
        public IEnumerable<DSupplier> DSuppliers { get; set; }
        public IEnumerable<DTransporter> DTransporters { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }

    public class StatementFilter
    {
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public string Branch { get; set; }
        public string LoggedInUser { get; set; }
        public string Sacco { get; set; }
    }
}
