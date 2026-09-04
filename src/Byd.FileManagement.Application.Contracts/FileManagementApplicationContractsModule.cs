using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

using Byd.CoreExtensions;
namespace Byd.FileManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationContractsModule),
    typeof(FileManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class FileManagementApplicationContractsModule : AbpModule
{

}
