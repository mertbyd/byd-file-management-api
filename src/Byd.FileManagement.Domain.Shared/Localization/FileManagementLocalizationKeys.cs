namespace Byd.FileManagement.Localization;

// islevi: Modul localization kaynaklarinda kod tarafindan kullanilan kararli anahtarlari tanimlar.
// sistemdeki gorevi: Izin tanimlarinin localization JSON anahtarlariyla compile-time tek sozlesmede
// bulusmasini saglar; provider'lar inline string tasimaz.
public static class FileManagementLocalizationKeys
{
    public static class Permissions
    {
        public const string Group = "Permission:FileManagement";
        public const string FileCategories = "Permission:FileCategories";
        public const string FileCategoriesView = "Permission:FileCategories.View";
        public const string FileCategoriesManage = "Permission:FileCategories.Manage";
    }
}
