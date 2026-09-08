using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Byd.CoreExtensions;
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
    // Modulun varsayilanlarini koyar: kendi kablolamasini kurar ve depolama bilgilerini ayar
    // bolumunden tipli options'a baglar.
    //
    // NEDEN OKUMA BURADA. ABP once bagimli modulun PreConfigureServices'ini, sonra tuketen host'un
    // kendi PreConfigureServices'ini calistirir. Okuma burada oldugu icin host hicbir sey yazmadan
    // calisir; degerleri baska bir kaynaktan (kod, vault, ortam degiskeni) vermek isteyen host ise
    // ayni options'i kendi PreConfigureServices'inde ezer ve sonra calistigi icin kazanir.
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        PreConfigure<FileManagementModuleOptions>(options =>
        {
            options.IsDefaultStorageEnabled = true;
            configuration.GetSection(FileStorageConsts.SectionName).Bind(options.FileStorage);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<FileManagementApplicationModule>();

        var moduleOptions = context.Services.ExecutePreConfiguredActions<FileManagementModuleOptions>();

        Configure<FileManagementModuleOptions>(options =>
        {
            options.IsDefaultStorageEnabled = moduleOptions.IsDefaultStorageEnabled;
            options.FileStorage = moduleOptions.FileStorage;
        });

        if (moduleOptions.IsDefaultStorageEnabled)
        {
            context.Services.AddFileManagementFileStorage(moduleOptions.FileStorage);
        }
    }
}
