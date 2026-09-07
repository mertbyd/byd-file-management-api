using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Options;
using Volo.Abp.Application;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace Byd.FileManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationModule),
    typeof(FileManagementDomainModule),
    typeof(FileManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringMinioModule)
    )]
// islevi: Uygulama servislerini kaydeder ve modulun tipli blob container'ini yapilandirir.
// sistemdeki gorevi: Depolama kablolamasi modulun isidir; tuketen host yalniz appsettings'e
// FileManagement:FileStorage bolumunu koyar, kablolama kodu yazmaz.
public class FileManagementApplicationModule : AbpModule
{
    // Modul kendi depolama kablolamasini varsayilan olarak kurar; kendi saglayicisini takan host kapatir.
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<FileManagementModuleOptions>(options =>
        {
            options.IsDefaultStorageEnabled = true;
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<FileManagementApplicationModule>();

        var moduleOptions = context.Services.ExecutePreConfiguredActions<FileManagementModuleOptions>();

        Configure<FileManagementModuleOptions>(options =>
        {
            options.IsDefaultStorageEnabled = moduleOptions.IsDefaultStorageEnabled;
        });

        if (moduleOptions.IsDefaultStorageEnabled)
        {
            ConfigureBlobStoring(context);
        }
    }

    // Modul yalniz kendi tipli container'ini yapilandirir; host'un varsayilan container'ina dokunmaz.
    private void ConfigureBlobStoring(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<StoredFileBlobContainer>(container =>
            {
                container.UseMinio(minio =>
                {
                    minio.EndPoint = configuration[FileStorageConsts.EndPointKey]!;
                    minio.AccessKey = configuration[FileStorageConsts.AccessKeyKey]!;
                    minio.SecretKey = configuration[FileStorageConsts.SecretKeyKey]!;
                    minio.BucketName = configuration[FileStorageConsts.BucketNameKey]!;
                    minio.WithSSL = Convert.ToBoolean(configuration[FileStorageConsts.WithSslKey]);
                    minio.CreateBucketIfNotExists =
                        Convert.ToBoolean(configuration[FileStorageConsts.CreateBucketIfNotExistsKey]);
                });
            });
        });
    }
}
