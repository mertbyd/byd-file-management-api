using Byd.CoreExtensions.ExceptionCodes;

namespace Byd.FileManagement.ExceptionCodes;

/// <summary>
/// Saklanan dosya is kurali hata kodlarini tanimlar.
/// </summary>
// islevi: Dosya adindan content type cozulemedigi durumun kararli kodunu verir.
// sistemdeki gorevi: Manager, localization ve istemciler icin tek hata sozlesmesi sahibidir.
public static class StoredFileExceptionCodes
{
    public const string Prefix = "FileManagement.StoredFile";

    // Dosya adinin uzantisi bilinen bir content type'a cozulemedi.
    public const string UnresolvedContentType = Prefix + ":00001";

    public const string DataTooLarge = Prefix + ":00002";

    // Dosya adi kalicilik sinirini asiyor.
    public const string FileNameTooLong = Prefix + ":00003";

    // Ortak yukleme alanlarinin eksik deger kodu bounded validator sahibinden okunur.
    public const string RequiredValueMissing = GeneralExceptionCodes.RequiredValueMissing;
}
