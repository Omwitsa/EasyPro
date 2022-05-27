using EasyPro.ViewModels;

namespace EasyPro.IProvider
{
    public interface IReportProvider
    {
        public byte[] GetIntakeReport(ProductIntakeVm intake);
    }
}
