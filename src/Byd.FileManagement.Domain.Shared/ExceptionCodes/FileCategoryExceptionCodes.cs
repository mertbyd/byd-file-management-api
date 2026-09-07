namespace Byd.FileManagement.ExceptionCodes;

/// <summary>
/// Dosya kategorisi girdi dogrulama hata kodlarini tanimlar.
/// </summary>
// islevi: Kategori adi zorunluluk ve uzunluk hatalarina kararli kodlar verir.
// sistemdeki gorevi: Validator, localization ve istemciler icin tek hata sozlesmesi sahibidir.
public static class FileCategoryExceptionCodes
{
    public const string Prefix = "FileManagement.FileCategory";
    public const string NameRequired = Prefix + ":00001";
    public const string NameTooLong = Prefix + ":00002";
}
