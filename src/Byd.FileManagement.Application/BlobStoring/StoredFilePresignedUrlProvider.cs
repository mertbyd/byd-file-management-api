using System;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel.Args;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Options;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;
using Volo.Abp.Timing;

namespace Byd.FileManagement.BlobStoring;

/// <summary>
/// StoredFile bloblari icin sureli MinIO GET ve PUT baglantilari uretir.
/// </summary>
// islevi: Blob adini cercevenin nesne anahtarina cevirip MinIO'nun presigned URL cagrilarina tasir.
// sistemdeki gorevi: IBlobContainer presigned URL sunmadigi icin kabul edilen MinIO sapmasini tek
// dosyada tutar; MinIO tipleri AppService, Controller, Manager ve DTO katmanlarina sizmaz.
public class StoredFilePresignedUrlProvider(
    ISettingProvider settingProvider,
    IClock clock,
    IMinioBlobNameCalculator blobNameCalculator,
    IBlobContainerConfigurationProvider configurationProvider)
    : ITransientDependency
{
    // Blob icin ayarli sure boyunca gecerli GET baglantisi uretir.
    public virtual async Task<(string Url, DateTime ExpiresAt)> CreateGetAsync(string blobName)
    {
        var storage = await ReadStorageAsync();
        var expirySeconds = (int)storage.PresignedGetExpiry.TotalSeconds;
        var url = await BuildClient(storage).PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(storage.BucketName!)
                .WithObject(CalculateObjectName(blobName))
                .WithExpiry(expirySeconds));

        return (url, clock.Now.AddSeconds(expirySeconds));
    }

    // Blob icin ayarli sure boyunca gecerli PUT baglantisi uretir.
    public virtual async Task<(string Url, DateTime ExpiresAt)> CreatePutAsync(string blobName)
    {
        var storage = await ReadStorageAsync();
        var expirySeconds = (int)storage.PresignedPutExpiry.TotalSeconds;
        var url = await BuildClient(storage).PresignedPutObjectAsync(
            new PresignedPutObjectArgs()
                .WithBucket(storage.BucketName!)
                .WithObject(CalculateObjectName(blobName))
                .WithExpiry(expirySeconds));

        return (url, clock.Now.AddSeconds(expirySeconds));
    }

    // Blob adini cercevenin bucket icinde kullandigi nesne anahtarina cevirir.
    //
    // NEDEN GEREKLI. IBlobContainer yazarken adi oldugu gibi kullanmaz; saglayici anahtari
    // IMinioBlobNameCalculator ile hesaplar ve kapsam onekini ekler (host/ veya tenants/{id}/).
    // Ham blob adiyla imzalanan URL var olmayan bir nesneyi gosterir ve MinIO 404 doner; hata
    // hicbir yerde aciklanmaz. Kural burada tekrar yazilmaz, cercevenin hesaplayicisi cagrilir:
    // biri hesaplayiciyi degistirirse presigned yol da kendiliginden uyar.
    private string CalculateObjectName(string blobName)
        => blobNameCalculator.Calculate(
            new BlobProviderGetArgs(
                StoredFileConsts.BlobContainerName,
                configurationProvider.Get(StoredFileConsts.BlobContainerName),
                blobName));

    // Runtime'da degisebilen ayarlari her cagri icin ortak okuyucudan tazeler.
    private async Task<FileStorageOptions> ReadStorageAsync()
    {
        var storage = await settingProvider.ReadFileStorageAsync();
        storage.EnsurePresignedConfigured();
        return storage;
    }

    // Presigned imzalamada kullanilan MinIO istemcisini guncel ayarlardan kurar.
    private static IMinioClient BuildClient(FileStorageOptions storage)
        => new MinioClient()
            .WithEndpoint(storage.EndPoint)
            .WithCredentials(storage.AccessKey, storage.SecretKey)
            .WithSSL(storage.WithSsl)
            .Build();
}
