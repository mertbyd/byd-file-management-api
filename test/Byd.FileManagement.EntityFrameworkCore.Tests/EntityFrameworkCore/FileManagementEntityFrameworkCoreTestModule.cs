using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions.EntityFrameworkCore;
using Byd.FileManagement.BlobStoring;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Byd.FileManagement.EntityFrameworkCore;

/// <summary>
/// Test host'unun veritabani baglantisini kurar ve baslangic verisini yukler.
/// </summary>
// islevi: Her test kosusu icin ayri bir PostgreSQL veritabani acar, modul grafigindeki tum
// DbContext'lerin semasini orada kurar ve acilista veri tohumlayicilarini calistirir.
// sistemdeki gorevi: Testler uretimdeki saglayiciya karsi kosar; bellek ici bir saglayici
// kullanilmaz, cunku tuketilen core modulu PostgreSQL saglayicisini host genelinde beyan eder.
// Byte deposu icin MinIO yerine dosya sistemi saglayicisi secilir: saglayici secimi host'un
// karari oldugu icin test host'u da baska bir saglayici secebilir. Modul farki gormez.
[DependsOn(
    typeof(FileManagementApplicationTestModule),
    typeof(FileManagementEntityFrameworkCoreModule),
    typeof(AbpBlobStoringFileSystemModule)
)]
public class FileManagementEntityFrameworkCoreTestModule : AbpModule
{
    private const string ConnectionStringTemplate =
        "Host=localhost;Port=5432;Database={0};Username=postgres;Password=postgres";

    private string _connectionString = string.Empty;
    private string _blobRootPath = string.Empty;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        _connectionString = CreateDatabaseAndGetConnectionString();

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = _connectionString;
        });

        ConfigureBlobStorage();
    }

    // Saglayici secimi host beyanidir; test host'u byte'lari gecici bir klasore yazar.
    private void ConfigureBlobStorage()
    {
        _blobRootPath = Path.Combine(Path.GetTempPath(), $"FileManagementTests_{Guid.NewGuid():N}");

        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<StoredFileBlobContainer>(container =>
            {
                container.UseFileSystem(fileSystem => fileSystem.BasePath = _blobRootPath);
            });
        });
    }

    // Veri tohumlama burada calisir; sema ve baglanti yalniz bu modulde vardir.
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    // Kosuya ozel veritabani ve byte klasoru host kapanirken dusurulur; artik birikmez.
    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        using (var dbContext = CreateDbContext<FileManagementDbContext>(_connectionString))
        {
            dbContext.Database.EnsureDeleted();
        }

        if (Directory.Exists(_blobRootPath))
        {
            Directory.Delete(_blobRootPath, recursive: true);
        }
    }

    // Her test host'u icin izole bir veritabani kurar; kosular birbirinin verisini gormez.
    private static string CreateDatabaseAndGetConnectionString()
    {
        var connectionString = string.Format(
            ConnectionStringTemplate,
            $"FileManagementTests_{Guid.NewGuid():N}");

        // Veritabanini core context acar; ayni cagri core tablolarini da kurar.
        using (var coreDbContext = CreateDbContext<CoreExtensionsDbContext>(connectionString))
        {
            coreDbContext.Database.EnsureCreated();
        }

        // Veritabani artik var; EnsureCreated ikinci semayi atlar, tablolar dogrudan eklenir.
        using (var fileDbContext = CreateDbContext<FileManagementDbContext>(connectionString))
        {
            fileDbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connectionString;
    }

    private static TDbContext CreateDbContext<TDbContext>(string connectionString)
        where TDbContext : DbContext
    {
        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return (TDbContext)Activator.CreateInstance(typeof(TDbContext), options)!;
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using var scope = context.ServiceProvider.CreateScope();
            await scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();
        });
    }
}
