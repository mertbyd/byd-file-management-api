using System;

namespace Byd.FileManagement.Consts;

/// <summary>
/// Saklanan dosya kaydinin tablo, indeks ve alan sinirlarini tanimlar.
/// </summary>
// islevi: Dosya metadata kaydinin kalicilik sabitlerini tek yerde tutar.
// sistemdeki gorevi: Domain, EF konfigurasyonu ve sonraki yukleme fazlarinin ortak sabit sozlesmesidir.
public static class StoredFileConsts
{
    // Saklanan dosya metadata kayitlarinin tutuldugu tablonun EF model adidir.
    public const string TableName = "StoredFiles";

    // Modulun byte'lari yazdigi blob container'inin adi.
    public const string BlobContainerName = "file-management";

    // Bir is kaydinin dosyalarini kategori kirilimiyle getiren okuma indeksinin adi.
    public const string ExternalCategoryIndex = "IX_StoredFiles_ExternalId_CategoryId";

    // Global tenant filtresinin her sorguya ekledigi kolonu karsilayan indeks adi.
    public const string TenantIndex = "IX_StoredFiles_TenantId";

    // Kaydi yazan modulun adini tasiyan kolonun azami uzunlugu.
    public const int MaxModuleNameLength = 128;

    // Blob adinin azami uzunlugu. Bugunku sema 32 karakterlik opak bir kimlik uretir; sinir
    // ileride yol bicimli bir adlandirmaya gecilebilsin diye genis birakildi.
    public const int MaxBlobNameLength = 512;

    // Kullanicinin yukledigi ozgun dosya adinin azami uzunlugu.
    public const int MaxOriginalFileNameLength = 260;

    // Yuklenebilir azami dosya boyutu (byte). Deger istek govdesinin bicimine gore secildi:
    // byte'lar JSON icinde base64 tasindigi icin 20 MB ikili veri yaklasik 27 MB govde eder ve
    // ASP.NET Core'un varsayilan 30 MB istek govdesi sinirinin altinda kalir.
    public const int MaxDataLength = 20 * 1024 * 1024;

    // MinIO presigned baglantilarinin kabul ettigi azami gecerlilik suresi.
    public static readonly TimeSpan MaxPresignedExpiry = TimeSpan.FromDays(7);

    // Bekleyen yuklemenin onbellekte, baglanti suresinin uzerine kaldigi pay. MinIO olayinin
    // RabbitMQ uzerinden gecikmeli ulasmasi bilgiyi dusurmesin diye vardir.
    public static readonly TimeSpan PendingUploadCacheMargin = TimeSpan.FromMinutes(5);

    // MinIO'nun yukleme bildirimlerini yayinladigi RabbitMQ exchange'i.
    public const string UploadNotificationExchangeName = "Byd.Minio";

    // Uygulamanin bildirim kuyrugunun, host'un ClientName degerine eklenen soneki.
    public const string UploadNotificationQueueSuffix = ".StoredFileUploads";
}
