namespace Byd.FileManagement.ExceptionCodes;

/// <summary>
/// Depolama yapilandirmasi hata kodlarini tanimlar.
/// </summary>
// islevi: Zorunlu bir depolama ayarinin beyan edilmedigi durumun kararli kodunu verir.
// sistemdeki gorevi: Modul kablolamasi, localization ve log'lar icin tek hata sozlesmesi sahibidir.
public static class FileStorageExceptionCodes
{
    public const string Prefix = "FileManagement.FileStorage";

    // Zorunlu bir depolama ayari bos birakildi; hangi anahtarin eksik oldugu hata verisinde tasinir.
    public const string NotConfigured = Prefix + ":00001";

    // Eksik anahtarin adini tasiyan hata verisi alani; localization metni bu adla yer degistirir.
    public const string SettingKeyData = "SettingKey";
}
