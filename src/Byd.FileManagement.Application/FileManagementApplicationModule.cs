using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions;
using Byd.FileManagement.Options;
using Volo.Abp.Application;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.RabbitMQ;

namespace Byd.FileManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationModule),
    typeof(FileManagementDomainModule),
    typeof(FileManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringMinioModule),
    typeof(AbpRabbitMqModule)
    )]
// islevi: Uygulama servislerini kaydeder ve modulun depolama varsayilanini koyar.
// sistemdeki gorevi: Baglanti bilgisi ABP setting management'ta MinioSettings grubunda durur ve
// container acilista UseFileManagement icinde baglanir. Modul hicbir yapilandirma dosyasi
// okumaz.
public class FileManagementApplicationModule : AbpModule
{
    // Modulun varsayilanini koyar: kendi depolama kablolamasini kurar.
    //
    // NEDEN BURADA. ABP once bagimli modulun PreConfigureServices'ini, sonra tuketen host'un
    // kendi PreConfigureServices'ini calistirir. Varsayilan burada oldugu icin host hicbir sey
    // yazmadan calisir; kendi depolamasini kuracak host ayni options'i kendi
    // PreConfigureServices'inde ezer ve sonra calistigi icin kazanir.
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
    }
}
