using System;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel.Args;
using Byd.FileManagement.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;
using Volo.Abp.Timing;

namespace Byd.FileManagement.BlobStoring;

/// <summary>
/// StoredFile bloblari icin sureli MinIO GET ve PUT baglantilari uretir.
/// </summary>
// islevi: Blob adini MinIO'nun presigned URL SDK cagrilarina tasir.
// sistemdeki gorevi: IBlobContainer presigned URL sunmadigi icin kabul edilen MinIO sapmasini tek
// dosyada tutar; MinIO tipleri AppService, Controller, Manager ve DTO katmanlarina sizmaz.
public class StoredFilePresignedUrlProvider(
    ISettingProvider settingProvider,
    IClock clock)
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
                .WithObject(blobName)
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
                .WithObject(blobName)
                .WithExpiry(expirySeconds));

        return (url, clock.Now.AddSeconds(expirySeconds));
    }

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
