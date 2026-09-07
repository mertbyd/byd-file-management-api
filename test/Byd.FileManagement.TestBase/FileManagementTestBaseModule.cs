using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions.Options;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;

namespace Byd.FileManagement;

/// <summary>
/// Tum test host'larinin ortak altyapisini kurar.
/// </summary>
// islevi: Yetkilendirmeyi test icin acar ve tuketilen core modulunun bekledigi modul kimligini beyan eder.
// sistemdeki gorevi: Veritabani gerektirmeyen ortak kurulumun sahibidir; veri tohumlama gibi
// veritabanina bagli adimlar DbContext'i olan EF Core test modulune aittir.
[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpGuidsModule)
)]
public class FileManagementTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysAllowAuthorization();

        // Test host'u da core modul ve ortam adini beyan eder; beyan olmadan core saglayicilari acilista durur.
        Configure<CoreExtensionsModuleOptions>(options =>
        {
            options.ModuleName = "FileManagement";
            options.EnvironmentName = "Test";
        });
    }
}
