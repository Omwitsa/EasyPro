using EasyPro.ViewModels.FarmersVM;
using System;
using System.Threading.Tasks;

namespace EasyPro.IProvider
{
    public interface IShares
    {
        Task<dynamic> deductshares(SharesFilter filter);
    }
}
