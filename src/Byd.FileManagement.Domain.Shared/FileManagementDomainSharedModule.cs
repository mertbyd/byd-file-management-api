using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Byd.FileManagement.Localization;
using Volo.Abp.Domain;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

using Byd.CoreExtensions;
namespace Byd.FileManagement;

[DependsOn(
    typeof(CoreExtensionsDomainSharedModule),
    typeof(AbpValidationModule),
    typeof(AbpDddDomainSharedModule)
)]
public class FileManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<FileManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<FileManagementResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/FileManagement");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("FileManagement", typeof(FileManagementResource));
        });
    }
}
