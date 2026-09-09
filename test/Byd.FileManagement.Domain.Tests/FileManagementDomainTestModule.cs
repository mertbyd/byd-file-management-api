using Byd.FileManagement.Options;
using Volo.Abp.Modularity;

namespace Byd.FileManagement;

/// <summary>
/// Domain tiplerine ihtiyac duyan test host'larinin ortak beyanlarini kurar.
/// </summary>
// islevi: Modulun varsayilan MinIO kablolamasini test tarafinda kapatir.
// sistemdeki gorevi: Testler MinIO calistiramaz; byte'lar dosya sistemine yazilir.
[DependsOn(
    typeof(FileManagementDomainModule),
    typeof(FileManagementTestBaseModule)
)]
public class FileManagementDomainTestModule : AbpModule
{
    // Modul varsayilani MinIO'dur; test host'u bunu kapatip kendi saglayicisini kurar.
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<FileManagementModuleOptions>(options =>
        {
            // Test host'u byte'lari dosya sistemine yazar; modulun MinIO kablolamasi kurulmaz.
            options.IsDefaultStorageEnabled = false;
        });
    }
}
