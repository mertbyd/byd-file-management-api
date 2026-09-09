using Volo.Abp.Settings;

namespace Byd.FileManagement.Settings;

/// <summary>
/// Modulun ayarlarini ABP setting management'a tanitir.
/// </summary>
// islevi: Her ayar ailesinin kendi partial metodunu cagirir.
// sistemdeki gorevi: Yeni bir ayar ailesi eklemek bu dosyaya tek satir eklemekten ibarettir;
// mevcut satirlar degismez. Deger burada tutulmaz; nerede saklanacagina ABP'nin deger saglayici
// zinciri karar verir.
public partial class FileManagementSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        AddMinioSettings(context);
    }
}
