using Byd.FileManagement.Localization;
using Volo.Abp.Authorization.Permissions;

namespace Byd.FileManagement.Permissions;

// islevi: Dosya kategorisi yetkilerini ABP yetki agacina ekler.
// sistemdeki gorevi: Ana provider'in Define metodu bu partial metodu cagirir.
public partial class FileManagementPermissionDefinitionProvider
{
    private void AddFileCategoriesPermissions(PermissionGroupDefinition group)
    {
        var fileCategories = group.AddPermission(
            FileManagementPermissions.FileCategories.Default,
            L(FileManagementLocalizationKeys.Permissions.FileCategories));

        fileCategories.AddChild(
            FileManagementPermissions.FileCategories.View,
            L(FileManagementLocalizationKeys.Permissions.FileCategoriesView));

        fileCategories.AddChild(
            FileManagementPermissions.FileCategories.Manage,
            L(FileManagementLocalizationKeys.Permissions.FileCategoriesManage));
    }
}
