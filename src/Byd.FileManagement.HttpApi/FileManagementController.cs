using Byd.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Byd.FileManagement;

public abstract class FileManagementController : AbpControllerBase
{
    protected FileManagementController()
    {
        LocalizationResource = typeof(FileManagementResource);
    }
}
