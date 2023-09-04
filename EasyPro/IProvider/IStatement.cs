using EasyPro.ViewModels.FarmersVM;
using System;
using System.Threading.Tasks;

namespace EasyPro.IProvider
{
    public interface IStatement
    {
        Task<dynamic> GenerateStatement(StatementFilter filter);
    }
}
