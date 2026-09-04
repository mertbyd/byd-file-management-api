using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions.EntityFrameworkCore;
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
// islevi: File DbContext'ini PostgreSQL, snake_case ve modul migration gecmis deposuyla ABP'ye kaydeder.
// sistemdeki gorevi: File entity/repository altyapisinin EF Core kompozisyon kokudur; core ile ayni veritabanini paylasir.
public class FileManagementEntityFrameworkCoreModule : AbpModule
{
    // PostgreSQL provider, snake_case ve gecmis deposu yalniz File DbContext'ine baglanir; core kendi context'ini yonetir.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<FileManagementDbContext>(configurationContext =>
            {
                configurationContext.UseNpgsql();
                configurationContext.DbContextOptions.UseSnakeCaseNamingConvention();
                configurationContext.DbContextOptions
                    .ReplaceService<IHistoryRepository, FileManagementHistoryRepository>();
            });
        });

        context.Services.AddAbpDbContext<FileManagementDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
