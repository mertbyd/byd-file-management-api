namespace Byd.FileManagement.Consts;

/// <summary>
/// Modulun veritabani sema adlarini tanimlar.
/// </summary>
// islevi: File modulunun kendi tablolarinin yazildigi sema adini derleme guvenli tek sabitte tutar.
// sistemdeki gorevi: Sema adinin tek kaynagidir; DbProperties ve EF konfigurasyonlari ayni sabiti okur, core semasina dokunmaz.
public static class DatabaseConsts
{
    // File modulunun kendi tablolarinin yazildigi sema.
    public const string ModuleSchema = "file_management";
}
