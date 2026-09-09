using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Options;
using Byd.FileManagement.Settings;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

namespace Byd.FileManagement;

/// <summary>
/// Tuketen host'un modulun acilis isini tetikledigi uzanti.
/// </summary>
// islevi: MinioSettings grubunun alt ayarlarini okuyup modulun blob container'ini baglar.
// sistemdeki gorevi: Modulun tuketen host'a actigi tek acilis kancasi. Ayar veritabanindan async
// okunur, container yapilandirmasi ise senkron sorulur; okuma burada bir kez yapilir ve sonuc
// dogrudan AbpBlobStoringOptions'a yazilir. Cagrilmazsa modulun container'i yapilandirilmaz ve
// ilk dosya islemi depolama saglayicisi bulunamadigi icin reddedilir.
public static class FileManagementApplicationBuilderExtensions
{
    // Modulun acilis isini calistirir.
    public static IApplicationBuilder UseFileManagement(this IApplicationBuilder app)
    {
        // Pipeline kurulumu senkron oldugu icin acilis isi burada beklenir. Istek yolunda degil,
        // process basina bir kez calisan acilis adimi oldugu icin bloklama kabul edilir.
        LoadFileStorageAsync(app.ApplicationServices).GetAwaiter().GetResult();
        return app;
    }

    // Depolama ayarini kendi unit of work'unde okur ve container'i baglar.
    // Ayar hic tanimlanmamissa acilis durdurulmaz: modulu tuketen host'un diger yetenekleri
    // dosya depolamadan bagimsiz calisir. Eksiklik ilk dosya isleminde bildirilir.
    private static async Task LoadFileStorageAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var moduleOptions = scope.ServiceProvider
            .GetRequiredService<IOptions<FileManagementModuleOptions>>().Value;
        if (!moduleOptions.IsDefaultStorageEnabled)
        {
            return;
        }

        var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        using var unitOfWork = unitOfWorkManager.Begin();

        var settingProvider = scope.ServiceProvider.GetRequiredService<ISettingProvider>();

        var storage = new FileStorageOptions
        {
            EndPoint = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.EndPoint),
            AccessKey = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.AccessKey),
            SecretKey = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.SecretKey),
            BucketName = await settingProvider.GetOrNullAsync(FileManagementSettings.Minio.BucketName),
            WithSsl = await settingProvider.IsTrueAsync(FileManagementSettings.Minio.WithSsl),
            CreateBucketIfNotExists =
                await settingProvider.IsTrueAsync(FileManagementSettings.Minio.CreateBucketIfNotExists)
        };

        await unitOfWork.CompleteAsync();

        // Adres grubun varlik isaretidir: bos ise grup hic doldurulmamis demektir ve acilis
        // durdurulmaz. Adres doluysa grup kullanilmak istenmistir; eksik kalan alan asagida
        // adiyla bildirilir.
        if (string.IsNullOrWhiteSpace(storage.EndPoint))
        {
            return;
        }

        storage.EnsureConfigured();

        // Container sozlugu canli bir nesnedir; cerceve her blob isleminde yapilandirmayi oradan
        // okur (DefaultBlobContainerConfigurationProvider.Get -> Containers.GetConfiguration).
        // Bu yuzden ayari burada yazmak yeterli, ayri bir saglayiciya gerek yok. Yazma acilista,
        // istekler karsilanmadan once ve tek is parcaciginda olur.
        scope.ServiceProvider.GetRequiredService<IOptions<AbpBlobStoringOptions>>().Value
            .Containers.Configure<StoredFileBlobContainer>(container =>
                container.UseMinio(minio =>
                {
                    // ! guvenli: EnsureConfigured bu dortlunun dolu oldugunu dogruladi.
                    minio.EndPoint = storage.EndPoint!;
                    minio.AccessKey = storage.AccessKey!;
                    minio.SecretKey = storage.SecretKey!;
                    minio.BucketName = storage.BucketName!;
                    minio.WithSSL = storage.WithSsl;
                    minio.CreateBucketIfNotExists = storage.CreateBucketIfNotExists;
                }));
    }
}
