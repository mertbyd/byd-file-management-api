namespace Byd.FileManagement.Permissions;

// islevi: Dosya kategorisi lookup'inin okuma ve yonetim yetki sabitlerini tanimlar.
// sistemdeki gorevi: Controller action'lari yetki string'ini buradan okur; inline string kullanilmaz.
public partial class FileManagementPermissions
{
    public static class FileCategories
    {
        public const string Default = GroupName + ".FileCategories";

        // Kategori listeleme ve detay okuma yetkisi.
        public const string View = Default + ".View";

        // Kategori olusturma, guncelleme ve pasiflestirme yetkisi.
        public const string Manage = Default + ".Manage";
    }
}
