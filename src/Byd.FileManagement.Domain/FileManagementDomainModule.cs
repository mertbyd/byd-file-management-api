using Volo.Abp.Domain;
using Volo.Abp.Modularity;

using Byd.CoreExtensions;
namespace Byd.FileManagement;

[DependsOn(
    typeof(CoreExtensionsDomainModule),
    typeof(AbpDddDomainModule),
    typeof(FileManagementDomainSharedModule)
)]
public class FileManagementDomainModule : AbpModule
{

}
