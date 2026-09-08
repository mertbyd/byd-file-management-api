using System;
using Microsoft.Extensions.DependencyInjection;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Consts;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Options;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;

namespace Byd.FileManagement;

/// <summary>
/// Modulun tipli blob container'ini servis koleksiyonuna kaydeden uzanti.
/// </summary>
// islevi: Zorunlu depolama alanlarini denetler ve container'i MinIO saglayicisina baglar.
// sistemdeki gorevi: Depolama kablolamasinin tek kompozisyon noktasidir; modul sinifi tek satirla
// bu uzantiyi cagirir ve saglayici ayrintilarini gormez.
public static class FileStorageConfigurationExtensions
{
    // Depolama bilgilerini denetleyip modulun kendi container'ini yapilandirir.
    // Host'un varsayilan container'ina dokunulmaz; yalnizca tipli container hedeflenir.
    public static IServiceCollection AddFileManagementFileStorage(
        this IServiceCollection services,
        FileStorageOptions storage)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(storage);

        EnsureConfigured(storage);

        return services.Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<StoredFileBlobContainer>(container =>
            {
                container.UseMinio(minio =>
                {
                    // ! guvenli: hemen once EnsureConfigured calisti, bu dortlu bos olamaz.
                    minio.EndPoint = storage.EndPoint!;
                    minio.AccessKey = storage.AccessKey!;
                    minio.SecretKey = storage.SecretKey!;
                    minio.BucketName = storage.BucketName!;
                    minio.WithSSL = storage.WithSsl;
                    minio.CreateBucketIfNotExists = storage.CreateBucketIfNotExists;
                });
            });
        });
    }

    // Zorunlu alanlari acilista denetler; eksik olani anahtar adiyla bildirir.
    //
    // NEDEN ACILISTA. Bu degerler ilk dosya islemine kadar kullanilmaz. Denetim olmazsa eksik ayar
    // acilista degil, ilk yuklemede, MinIO'dan gelen anlamsiz bir mesajla ortaya cikar ve hangi
    // anahtarin eksik oldugu hicbir yerde yazmaz. Burada patlarsa log'un ilk satirinda yazar.
    //
    // Hata modulun diger hatalari gibi kararli bir kod tasir ve metnini localization dosyalarindan
    // alir; eksik anahtarin adi hata verisinde tasinir ve metinde yerine gecer. Tuketilen core da
    // ayni durumu boyle bildiriyor: CurrentModuleNameProvider, ModuleName beyan edilmemisse
    // BusinessException("CoreExtensions.ModuleSetting:00008") firlatiyor.
    private static void EnsureConfigured(FileStorageOptions storage)
    {
        EnsureProvided(storage.EndPoint, FileStorageConsts.EndPointKey);
        EnsureProvided(storage.AccessKey, FileStorageConsts.AccessKeyKey);
        EnsureProvided(storage.SecretKey, FileStorageConsts.SecretKeyKey);
        EnsureProvided(storage.BucketName, FileStorageConsts.BucketNameKey);
    }

    // Tek bir alani denetler; bos ise anahtar adini hata verisine koyarak bildirir.
    private static void EnsureProvided(string? value, string configurationKey)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessException(FileStorageExceptionCodes.NotConfigured)
                .WithData(FileStorageExceptionCodes.SettingKeyData, configurationKey);
        }
    }
}
