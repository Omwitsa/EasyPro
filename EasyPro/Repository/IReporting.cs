using EasyPro.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPro.Repository
{
    public interface IReporting
    {
        List<UserMasterViewModel> GetUserwiseReport();
        List<UserIntakeViewModel> GetUserintakeReport();
        List<UserDeduViewModel> GetUserdeductionReport();
    }
}
