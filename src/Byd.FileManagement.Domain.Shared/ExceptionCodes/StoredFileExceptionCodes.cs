using Byd.CoreExtensions.ExceptionCodes;

namespace Byd.FileManagement.ExceptionCodes;

/// <summary>
/// Saklanan dosya is kurali hata kodlarini tanimlar.
/// </summary>
// islevi: Dosya adindan content type cozulemedigi durumun kararli kodunu verir.
// sistemdeki gorevi: Manager, localization ve istemciler icin tek hata sozlesmesi sahibidir.
public static class StoredFileExceptionCodes
{
    // Dosya adinin uzantisi bilinen bir content type'a cozulemedi.
    public const string UnresolvedContentType = FileManagementErrorCodes.StoredFileGroup + "0001";

    public const string DataTooLarge = FileManagementErrorCodes.StoredFileGroup + "0002";

    // Dosya adi kalicilik sinirini asiyor.
    public const string FileNameTooLong = FileManagementErrorCodes.StoredFileGroup + "0003";

    // Ortak yukleme alanlarinin eksik deger kodu bounded validator sahibinden okunur.
    public const string RequiredValueMissing = GeneralExceptionCodes.RequiredValueMissing;
}
