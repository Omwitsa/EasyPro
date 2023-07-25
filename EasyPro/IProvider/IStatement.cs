using EasyPro.ViewModels.FarmersVM;
using System;

namespace EasyPro.IProvider
{
    public interface IStatement
    {
        dynamic GenerateStatement(StatementFilter filter);
    }
}
