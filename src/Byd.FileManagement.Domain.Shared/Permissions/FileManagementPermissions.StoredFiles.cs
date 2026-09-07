namespace Byd.FileManagement.Permissions;

/// <summary>
/// Saklanan dosya metadata okumasinin yetki sabitlerini tanimlar.
/// </summary>
// islevi: Dosya metadata okuma yetkisinin string sabitini tanimlar.
// sistemdeki gorevi: Controller action'lari yetki string'ini buradan okur; inline string kullanilmaz.
public partial class FileManagementPermissions
{
    public static class StoredFiles
    {
        public const string Default = GroupName + ".StoredFiles";

        // Dosya metadata detayi, filtreli listesi ve sayimini okuma yetkisi.
        public const string View = Default + ".View";
    }
}
