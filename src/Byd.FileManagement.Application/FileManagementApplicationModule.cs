using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

using Byd.CoreExtensions;
namespace Byd.FileManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationModule),
    typeof(FileManagementDomainModule),
    typeof(FileManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class FileManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<FileManagementApplicationModule>();
    }
}
