using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.Permissions;
using Byd.FileManagement.Services.StoredFiles;
using SystemStandards.Results;

namespace Byd.FileManagement.Controllers.StoredFiles;

/// <summary>
/// Saklanan dosya okuma, yukleme, indirme ve silme islemlerini HTTP uzerinden sunar.
/// </summary>
// islevi: Route ve request binding metadatasini tasiyip her istegi tek AppService cagrisina yonlendirir.
// sistemdeki gorevi: StoredFile dikey kesitinin ince HTTP giris noktasidir.
[ApiController]
[Route(StoredFileRoutes.Root)]
public class StoredFileController : FileManagementController
{
    /// <summary>StoredFile AppService'ini lazy cozer.</summary>
    private IStoredFileAppService AppService =>
        LazyGetRequiredService<IStoredFileAppService>();

    /// <summary>
    /// Verilen is kayitlarina bagli dosyalarin metadata bilgilerini toplu getirir.
    /// </summary>
    /// <param name="externalIds">Dosyalari aranacak is kaydi kimlikleri.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   externalIds (Guid[]) → Dosyalari aranacak is kaydi kimlikleri
    /// }
    /// Response elemanlari StoredFile metadata alanlarini tasir; byte icerigi dondurulmez.
    /// </remarks>
    [HttpPost(StoredFileRoutes.ByExternalIds)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetListByExternalIdsAsync(
        [FromBody] List<Guid> externalIds,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetListByExternalIdsAsync(externalIds, cancellationToken);
        return result;
    }

    /// <summary>
    /// Bir is kaydinin verilen kategorideki dosya metadata kayitlarini getirir.
    /// </summary>
    /// <param name="externalId">Dosyalarin bagli oldugu is kaydi kimligi.</param>
    /// <param name="categoryId">Dosyalarin ait oldugu kategori kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   externalId (Guid) → Is kaydi kimligi
    ///   categoryId (Guid) → Dosya kategorisi kimligi
    /// }
    /// Response elemanlari StoredFile metadata alanlarini tasir; eslesen kayit yoksa bos liste doner.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByCategory)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetListByCategoryAsync(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetListByCategoryAsync(externalId, categoryId, cancellationToken);
        return result;
    }

    /// <summary>
    /// Verilen kategorideki tum dosyalari metadata ve byte icerikleriyle getirir.
    /// </summary>
    /// <param name="categoryId">Dosyalari aranacak kategori kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   categoryId (Guid) → Dosya kategorisi kimligi
    /// }
    /// Response elemanlari StoredFile metadata alanlarini ve JSON'da Base64 gorunen Data (byte[]) icerigini tasir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByCategoryWithContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileWithContentDto>>> GetListWithContentByCategoryAsync(
        [FromQuery] Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetListWithContentByCategoryAsync(categoryId, cancellationToken);
        return result;
    }

    /// <summary>
    /// Bir is kaydinin verilen kategorideki tum dosyalarini metadata ve byte icerikleriyle getirir.
    /// </summary>
    /// <param name="externalId">Dosyalarin bagli oldugu is kaydi kimligi.</param>
    /// <param name="categoryId">Dosyalarin ait oldugu kategori kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   externalId (Guid) → Is kaydi kimligi
    ///   categoryId (Guid) → Dosya kategorisi kimligi
    /// }
    /// Response elemanlari StoredFile metadata alanlarini ve JSON'da Base64 gorunen Data (byte[]) icerigini tasir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByExternalIdAndCategoryWithContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileWithContentDto>>> GetListWithContentByExternalIdAndCategoryAsync(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetListWithContentByExternalIdAndCategoryAsync(
            externalId,
            categoryId,
            cancellationToken);
        return result;
    }

    /// <summary>
    /// Dosya byte'larini blob depoya yazar ve olusan StoredFile kaydini dondurur.
    /// </summary>
    /// <param name="input">Is kaydi, kategori, dosya adi ve byte icerigi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   ExternalId (Guid)  → Dosyanin bagli oldugu is kaydi kimligi
    ///   CategoryId (Guid)  → Dosya kategorisi kimligi
    ///   FileName (string)  → Ozgun dosya adi
    ///   Data (byte[])      → JSON'da Base64 gonderilen dosya icerigi
    /// }
    /// Response StoredFile metadata alanlarini tasir.
    /// </remarks>
    [HttpPost(StoredFileRoutes.Upload)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result<StoredFileDto>> UploadAsync(
        [FromBody] UploadStoredFileDto input,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.UploadAsync(input, cancellationToken);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosyanin byte icerigini standart Result cevabi icinde getirir.
    /// </summary>
    /// <param name="id">Indirilecek StoredFile kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   id (Guid) → StoredFile kaydinin kimligi
    /// }
    /// Response {
    ///   OriginalFileName (string) → Ozgun dosya adi
    ///   ContentType (string)      → Dosya adindan cozulmus MIME turu
    ///   Data (byte[])             → JSON'da Base64 gorunen dosya icerigi
    /// }
    /// </remarks>
    [HttpGet(StoredFileRoutes.Download)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<DownloadStoredFileDto>> DownloadAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.DownloadAsync(id, cancellationToken);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosya icin dogrudan depolamaya giden sureli indirme baglantisi uretir.
    /// </summary>
    /// <param name="id">Presigned indirme baglantisi uretilecek StoredFile kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   id (Guid) → StoredFile kaydinin kimligi
    /// }
    /// Response {
    ///   OriginalFileName (string) → Ozgun dosya adi
    ///   Url (string)              → Sureli indirme baglantisi
    ///   ExpiresAt (DateTime)      → Baglantinin mutlak bitis zamani
    /// }
    /// </remarks>
    [HttpGet(StoredFileRoutes.PresignedUrl)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<PresignedUrlDto>> GetPresignedUrlAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetPresignedUrlAsync(id, cancellationToken);
        return result;
    }

    /// <summary>
    /// Bir is kaydinin verilen kategorideki tum dosyalari icin sureli indirme baglantilari uretir.
    /// </summary>
    /// <param name="externalId">Dosyalarin bagli oldugu is kaydi kimligi.</param>
    /// <param name="categoryId">Dosyalarin ait oldugu kategori kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   externalId (Guid) → Is kaydi kimligi
    ///   categoryId (Guid) → Dosya kategorisi kimligi
    /// }
    /// Response elemanlari OriginalFileName (string), Url (string) ve ExpiresAt (DateTime) alanlarini tasir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByExternalIdAndCategoryPresignedUrls)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<PresignedUrlDto>>> GetPresignedUrlsByExternalIdAndCategoryAsync(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetPresignedUrlsByExternalIdAndCategoryAsync(
            externalId,
            categoryId,
            cancellationToken);
        return result;
    }

    /// <summary>
    /// Dosya byte'i almadan sureli dogrudan yukleme baglantisi uretir.
    /// </summary>
    /// <param name="input">Is kaydi, kategori ve ozgun dosya adi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   ExternalId (Guid) → Dosyanin bagli oldugu is kaydi kimligi
    ///   CategoryId (Guid) → Dosya kategorisi kimligi
    ///   FileName (string) → Ozgun dosya adi
    /// }
    /// Response {
    ///   OriginalFileName (string) → Ozgun dosya adi
    ///   Url (string)              → Sureli PUT baglantisi
    ///   ExpiresAt (DateTime)      → Baglantinin mutlak bitis zamani
    /// }
    /// </remarks>
    [HttpPost(StoredFileRoutes.PresignedUpload)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result<PresignedUrlDto>> CreatePresignedUploadAsync(
        [FromBody] PresignedUploadStoredFileDto input,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.CreatePresignedUploadAsync(input, cancellationToken);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosyayi tarayicinin dogrudan indirebilecegi HTTP dosya cevabi olarak getirir.
    /// </summary>
    /// <param name="id">Indirilecek StoredFile kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   id (Guid) → StoredFile kaydinin kimligi
    /// }
    /// Response ham dosya govdesidir; Content-Type ve Content-Disposition basliklari kayitli dosyadan uretilir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.DownloadContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<FileContentResult> DownloadContentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var file = await AppService.DownloadAsync(id, cancellationToken);
        return File(file.Data, file.ContentType, file.OriginalFileName);
    }

    /// <summary>
    /// Kimligi verilen dosyanin metadata kaydini ve blob icerigini siler.
    /// </summary>
    /// <param name="id">Silinecek StoredFile kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   id (Guid) → Silinecek StoredFile kaydinin kimligi
    /// }
    /// Response basarili istekte iceriksizdir.
    /// </remarks>
    [HttpDelete(StoredFileRoutes.ById)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await AppService.DeleteAsync(id, cancellationToken);
        return Result.NoContent();
    }

    /// <summary>
    /// Kimligi verilen dosyanin metadata kaydini getirir.
    /// </summary>
    /// <param name="id">Getirilecek StoredFile kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   id (Guid) → StoredFile kaydinin kimligi
    /// }
    /// Response {
    ///   Id                   (Guid)      → Kayit Id'si
    ///   TenantId             (Guid?)     → Dosyanin tenant'i; bos ise host
    ///   OrganizationId       (Guid?)     → Dosyanin yuklendigi organizasyon
    ///   CategoryId           (Guid)      → Dosya kategorisi Id'si
    ///   CategoryName         (string)    → Dosya kategorisi adi
    ///   ExternalId           (Guid)      → Dosyanin bagli oldugu is kaydi Id'si
    ///   ModuleName           (string)    → Dosyayi yazan modul
    ///   OriginalFileName     (string)    → Yuklenen ozgun dosya adi
    ///   CreationTime         (DateTime)  → Yukleme zamani
    ///   LastModificationTime (DateTime?) → Son guncelleme zamani
    /// }
    /// Byte icerigi bu ucta dondurulmez; diger metadata uclari ayni alanlari tasir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ById)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<StoredFileDto>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetAsync(id, cancellationToken);
        return result;
    }

    /// <summary>
    /// Dosya metadata kayitlarini filtre, siralama ve sayfalama uygulayarak getirir.
    /// </summary>
    /// <param name="input">StoredFile filtre, siralama ve sayfalama bilgileri.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   Filters (StoredFileFilter?) → ModuleName, ExternalId, CategoryId, TenantId ve CreationTime filtreleri
    ///   Sorting (string?)           → Siralama ifadesi
    ///   SkipCount (int)            → Atlanacak kayit sayisi
    ///   MaxResultCount (int)       → Alinacak azami kayit sayisi
    /// }
    /// Response StoredFile metadata listesidir; byte icerigi dondurulmez.
    /// </remarks>
    [HttpPost(StoredFileRoutes.FilteredList)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetFilteredListAsync(
        [FromBody] StoredFileListInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetFilteredListAsync(input, cancellationToken);
        return result;
    }

    /// <summary>
    /// Verilen filtrelere uyan dosya metadata kayitlarinin toplam sayisini getirir.
    /// </summary>
    /// <param name="input">Sayimda kullanilacak StoredFile filtre bilgileri.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request filtreli liste ucuyla ayni filtre alanlarini tasir.
    /// Response {
    ///   value (int) → Filtrelere uyan kayit sayisi
    /// }
    /// </remarks>
    [HttpPost(StoredFileRoutes.FilteredCount)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<int>> GetFilteredCountAsync(
        [FromBody] StoredFileListInput input,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetFilteredCountAsync(input, cancellationToken);
        return result;
    }
}
