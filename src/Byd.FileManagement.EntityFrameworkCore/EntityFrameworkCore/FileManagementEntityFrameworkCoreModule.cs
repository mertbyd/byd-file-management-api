using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions.EntityFrameworkCore;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace Byd.FileManagement.EntityFrameworkCore;

[DependsOn(
    typeof(CoreExtensionsEntityFrameworkCoreModule),
    typeof(FileManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule)
)]
// islevi: File DbContext'ini PostgreSQL saglayicisiyla ABP'ye kaydeder.
// sistemdeki gorevi: File entity/repository altyapisinin EF Core kompozisyon kokudur; core ile ayni veritabanini paylasir.
public class FileManagementEntityFrameworkCoreModule : AbpModule
{
    // PostgreSQL saglayicisi yalniz File DbContext'ine baglanir; core kendi context'ini yonetir.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<FileManagementDbContext>(configurationContext =>
            {
                configurationContext.UseNpgsql();
            });
        });

        context.Services.AddAbpDbContext<FileManagementDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        context.Services.AddTransient<IModuleSettingRepository, EfCoreModuleSettingRepository>();
    }
}
