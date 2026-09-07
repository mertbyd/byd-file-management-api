using Byd.FileManagement.Localization;
using Volo.Abp.Application.Services;

namespace Byd.FileManagement;

public abstract class FileManagementAppService : ApplicationService
{
    protected FileManagementAppService()
    {
        LocalizationResource = typeof(FileManagementResource);
        ObjectMapperContext = typeof(FileManagementApplicationModule);
    }
}
