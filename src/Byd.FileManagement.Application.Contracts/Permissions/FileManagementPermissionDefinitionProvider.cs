using Byd.FileManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Byd.FileManagement.Permissions;

/// <summary>
/// Modulun yetki agacini ABP'ye tanimlar.
/// </summary>
// islevi: Yetki grubunu acar ve her ozellik ailesinin kendi partial metodunu cagirir.
// sistemdeki gorevi: Yeni bir ozellik ailesi eklemek bu dosyaya tek satir eklemekten ibarettir;
// mevcut satirlar degismez.
public partial class FileManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            FileManagementPermissions.GroupName,
            L(FileManagementLocalizationKeys.Permissions.Group));

        AddFileCategoriesPermissions(group);
        AddStoredFilesPermissions(group);
    }

    // Localization anahtarini modul kaynagina baglar.
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FileManagementResource>(name);
    }
}
