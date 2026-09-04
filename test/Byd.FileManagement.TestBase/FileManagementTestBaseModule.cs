using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

using Byd.CoreExtensions.Options;
namespace Byd.FileManagement;

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

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}
