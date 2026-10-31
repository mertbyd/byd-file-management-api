using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Byd.FileManagement.Consts;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Options;
using Byd.FileManagement.Settings;
using Volo.Abp;
using Volo.Abp.Settings;

namespace Byd.FileManagement;

/// <summary>
/// Ayardan okunan depolama bilgisinin zorunlu alanlarini denetler.
/// </summary>
// islevi: MinioSettings grubunun zorunlu alanlarinin dolduruldugunu dogrular.
// sistemdeki gorevi: Acilis yukleyicisi container'i baglamadan once bu denetimden gecer; eksik
// alan MinIO'dan gelen anlamsiz bir mesaj yerine kendi kodumuzla ve alan adiyla bildirilir.
public static class FileStorageConfigurationExtensions
{
    // MinioSettings alanlarini tek kaynaktan tipli depolama sozlesmesine okur.
    public static async Task<FileStorageOptions> ReadFileStorageAsync(
        this ISettingProvider settingProvider,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settingProvider);

        return new FileStorageOptions
        {
            EndPoint = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.EndPoint),
            AccessKey = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.AccessKey),
            SecretKey = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.SecretKey),
            BucketName = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.BucketName),
            WithSsl = await settingProvider.IsTrueAsync(FileManagementSettings.Minio.WithSsl),
            CreateBucketIfNotExists =
                await settingProvider.IsTrueAsync(FileManagementSettings.Minio.CreateBucketIfNotExists),
            PresignedGetExpiry = await ReadExpiryAsync(
                settingProvider,
                FileManagementSettings.Minio.PresignedGetExpiry,
                FileStorageConsts.PresignedGetExpiryField,
                cancellationToken),
            PresignedPutExpiry = await ReadExpiryAsync(
                settingProvider,
                FileManagementSettings.Minio.PresignedPutExpiry,
                FileStorageConsts.PresignedPutExpiryField,
                cancellationToken)
        };
    }

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

    // Presigned istemcisinin endpoint dahil tum baglanti alanlarini kullanabildigini dogrular.
    public static void EnsurePresignedConfigured(this FileStorageOptions storage)
    {
        ArgumentNullException.ThrowIfNull(storage);

        EnsureProvided(storage.EndPoint, FileStorageConsts.EndPointField);
        storage.EnsureConfigured();
    }

    // Ayar metnini MinIO'nun bir saniye-yedi gun araligindaki sure sozlesmesine cevirir.
    private static async Task<TimeSpan> ReadExpiryAsync(
        ISettingProvider settingProvider,
        string settingName,
        string fieldName,
        CancellationToken cancellationToken = default)
    {
        var value = await settingProvider.GetOrNullAsync(settingName);
        if (!TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var expiry) ||
            expiry < TimeSpan.FromSeconds(1) ||
            expiry > StoredFileConsts.MaxPresignedExpiry)
        {
            throw CreateNotConfiguredException(fieldName);
        }

        return expiry;
    }

    // Tek bir alani denetler; bos ise alan adini hata verisine koyarak bildirir.
    private static void EnsureProvided(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw CreateNotConfiguredException(fieldName);
        }
    }

    // Gecersiz veya eksik depolama alanini kararli hata verisiyle bildirir.
    private static BusinessException CreateNotConfiguredException(string fieldName)
        => new BusinessException(FileStorageExceptionCodes.NotConfigured)
            .WithData(FileStorageExceptionCodes.SettingKeyData, fieldName);
}
