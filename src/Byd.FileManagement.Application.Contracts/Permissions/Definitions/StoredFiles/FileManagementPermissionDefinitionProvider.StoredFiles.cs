using Byd.FileManagement.Localization;
using Volo.Abp.Authorization.Permissions;

namespace Byd.FileManagement.Permissions;

// islevi: Saklanan dosya yetkilerini ABP yetki agacina ekler.
// sistemdeki gorevi: Ana provider'in Define metodu bu partial metodu cagirir.
public partial class FileManagementPermissionDefinitionProvider
{
    private void AddStoredFilesPermissions(PermissionGroupDefinition group)
    {
        var storedFiles = group.AddPermission(
            FileManagementPermissions.StoredFiles.Default,
            L(FileManagementLocalizationKeys.Permissions.StoredFiles));

        storedFiles.AddChild(
            FileManagementPermissions.StoredFiles.View,
            L(FileManagementLocalizationKeys.Permissions.StoredFilesView));
    }
}
