namespace Byd.FileManagement;

/// <summary>
/// Modulun hata kodu ad alanini ve aile gruplarini tanimlar.
/// </summary>
// islevi: Butun hata kodlarinin ortak on ekini ve her ailenin grup numarasini tek yerde tutar.
// sistemdeki gorevi: Ad alani ile ABP'nin MapCodeNamespace kaydinin tek kaynagidir.
//
// NEDEN AD ALANI DUZ. ABP mesaji ararken kodun iki noktadan onceki tamamini alir
// (Code.Split(':')[0]) ve kayit sozlugunde tam eslesme arar. Ad alanina aile eklenirse
// (ornek "FileManagement.StoredFile") kok kaydi eslesmez ve mesaj sessizce genel metne
// duser. Bu yuzden ad alani tektir, gruplama numarada tasinir; ABP'nin kendi modulleri de
// ayni sekilde yazar: "Volo.Abp.Identity:010002".
public static class FileManagementErrorCodes
{
    public const string RootNamespace = "FileManagement";

    // Saklanan dosya ailesi.
    public const string StoredFileGroup = RootNamespace + ":01";

    // Dosya kategorisi ailesi.
    public const string FileCategoryGroup = RootNamespace + ":02";

    // Depolama yapilandirmasi ailesi.
    public const string FileStorageGroup = RootNamespace + ":03";
}
