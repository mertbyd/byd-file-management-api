namespace Byd.FileManagement.ExceptionCodes;

/// <summary>
/// Dosya kategorisi girdi dogrulama hata kodlarini tanimlar.
/// </summary>
// islevi: Kategori adi zorunluluk ve uzunluk hatalarina, gecersiz azami boyuta kararli kodlar verir.
// sistemdeki gorevi: Validator, localization ve istemciler icin tek hata sozlesmesi sahibidir.
public static class FileCategoryExceptionCodes
{
    public const string NameRequired = FileManagementErrorCodes.FileCategoryGroup + "0001";

    public const string NameTooLong = FileManagementErrorCodes.FileCategoryGroup + "0002";

    // Kategorinin azami dosya boyutu verildiyse sifirdan buyuk olmali.
    public const string MaxFileSizeInvalid = FileManagementErrorCodes.FileCategoryGroup + "0003";
}
