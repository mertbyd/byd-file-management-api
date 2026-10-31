using Byd.FileManagement.Localization;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Byd.FileManagement;

public abstract class FileManagementAppService : ApplicationService
{
    protected FileManagementAppService()
    {
        LocalizationResource = typeof(FileManagementResource);
        ObjectMapperContext = typeof(FileManagementApplicationModule);
    }

    // ABP lazy provider uzerinden application bagimliliklarini repository-native sekilde cozer.
    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return LazyServiceProvider.LazyGetRequiredService<TService>();
    }
}
