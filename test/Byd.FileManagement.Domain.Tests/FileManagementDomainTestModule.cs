using Volo.Abp.Modularity;

namespace Byd.FileManagement;

[DependsOn(
    typeof(FileManagementDomainModule),
    typeof(FileManagementTestBaseModule)
)]
public class FileManagementDomainTestModule : AbpModule
{

}
