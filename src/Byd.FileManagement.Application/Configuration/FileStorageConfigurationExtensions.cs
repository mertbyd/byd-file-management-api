using System;
using Byd.FileManagement.Consts;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Options;
using Volo.Abp;

namespace Byd.FileManagement;

/// <summary>
/// Ayardan okunan depolama bilgisinin zorunlu alanlarini denetler.
/// </summary>
// islevi: MinioSettings grubunun zorunlu alanlarinin dolduruldugunu dogrular.
// sistemdeki gorevi: Acilis yukleyicisi container'i baglamadan once bu denetimden gecer; eksik
// alan MinIO'dan gelen anlamsiz bir mesaj yerine kendi kodumuzla ve alan adiyla bildirilir.
public static class FileStorageConfigurationExtensions
{
    // Zorunlu alanlari denetler; eksik olani alan adiyla bildirir.
    //
    // NEDEN YUKLEME ANINDA. Denetim olmazsa eksik alan ilk dosya isleminde, MinIO'dan gelen
    // anlamsiz bir mesajla ortaya cikar ve hangi alanin bos oldugu hicbir yerde yazmaz.
    //
    // Hata modulun diger hatalari gibi kararli bir kod tasir ve metnini localization dosyalarindan
    // alir; eksik alanin adi hata verisinde tasinir ve metinde yerine gecer.
    //
    // EndPoint burada yok: yukleyici onu grubun varlik isareti olarak zaten denetliyor.
    public static void EnsureConfigured(this FileStorageOptions storage)
    {
        ArgumentNullException.ThrowIfNull(storage);

        EnsureProvided(storage.AccessKey, FileStorageConsts.AccessKeyField);
        EnsureProvided(storage.SecretKey, FileStorageConsts.SecretKeyField);
        EnsureProvided(storage.BucketName, FileStorageConsts.BucketNameField);
    }

    // Tek bir alani denetler; bos ise alan adini hata verisine koyarak bildirir.
    private static void EnsureProvided(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessException(FileStorageExceptionCodes.NotConfigured)
                .WithData(FileStorageExceptionCodes.SettingKeyData, fieldName);
        }
    }
}
