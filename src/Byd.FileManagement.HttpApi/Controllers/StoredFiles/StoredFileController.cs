using System;
using System.Collections.Generic;
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
    /// <remarks>
    /// Request: ExternalId listesi.
    /// Response value elemanlari dosya Id, ExternalId, kapsam, kategori, modul, dosya adi ve audit
    /// alanlarini tasir. Bu endpoint byte icerigi dondurmez.
    /// </remarks>
    [HttpPost(StoredFileRoutes.ByExternalIds)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetListByExternalIds(
        [FromBody] List<Guid> externalIds)
    {
        var result = await AppService.GetListByExternalIdsAsync(externalIds);
        return result;
    }

    /// <summary>
    /// Bir is kaydinin verilen kategorideki dosya metadata kayitlarini getirir.
    /// </summary>
    /// <param name="externalId">Dosyalarin bagli oldugu is kaydi kimligi.</param>
    /// <param name="categoryId">Dosyalarin ait oldugu kategori kimligi.</param>
    /// <remarks>
    /// Response value elemanlari StoredFile metadata alanlarini tasir. Eslesen kayit yoksa bos liste
    /// doner; dosya byte'lari bu endpointte okunmaz.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByCategory)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetListByCategory(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId)
    {
        var result = await AppService.GetListByCategoryAsync(externalId, categoryId);
        return result;
    }

    /// <summary>
    /// Verilen kategorideki tum dosyalari metadata ve byte icerikleriyle getirir.
    /// </summary>
    /// <param name="categoryId">Dosyalari aranacak kategori kimligi.</param>
    /// <remarks>
    /// Response value elemanlari StoredFile metadata alanlarini ve Data degerini tasir.
    /// Data, MinIO/blob depodan okunan byte[] icerigidir ve JSON icinde Base64 olarak gorunur.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByCategoryWithContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileWithContentDto>>> GetListWithContentByCategory(
        [FromQuery] Guid categoryId)
    {
        var result = await AppService.GetListWithContentByCategoryAsync(categoryId);
        return result;
    }

    /// <summary>
    /// Bir is kaydinin verilen kategorideki tum dosyalarini metadata ve byte icerikleriyle getirir.
    /// </summary>
    /// <param name="externalId">Dosyalarin bagli oldugu is kaydi kimligi.</param>
    /// <param name="categoryId">Dosyalarin ait oldugu kategori kimligi.</param>
    /// <remarks>
    /// Yalniz iki kimlikle de eslesen kayitlar doner. Her elemanda StoredFile metadata alanlari ve
    /// MinIO/blob depodan okunan Data bulunur; byte[] JSON cevabinda Base64 olarak gosterilir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByExternalIdAndCategoryWithContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileWithContentDto>>> GetListWithContentByExternalIdAndCategory(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId)
    {
        var result = await AppService.GetListWithContentByExternalIdAndCategoryAsync(externalId, categoryId);
        return result;
    }

    /// <summary>
    /// Dosya byte'larini blob depoya yazar ve olusan StoredFile kaydini dondurur.
    /// </summary>
    /// <param name="input">Is kaydi, kategori, dosya adi ve byte icerigi.</param>
    /// <remarks>
    /// Request { ExternalId, CategoryId, Data, FileName }. Data byte[] oldugu icin JSON isteginde
    /// Base64 gonderilir. File modulu metadata kaydini acar, icerigi ayarli blob saglayicisina yazar
    /// Response value StoredFile metadata bilgisidir.
    /// </remarks>
    [HttpPost(StoredFileRoutes.Upload)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result<StoredFileDto>> Upload([FromBody] UploadStoredFileDto input)
    {
        var result = await AppService.UploadAsync(input);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosyanin byte icerigini standart Result cevabi icinde getirir.
    /// </summary>
    /// <param name="id">Indirilecek StoredFile kaydinin kimligi.</param>
    /// <remarks>
    /// Response value { Data, OriginalFileName, ContentType }. Data blob depodan okunan byte[]
    /// degeridir ve JSON icinde Base64 olarak gorunur.
    /// </remarks>
    [HttpGet(StoredFileRoutes.Download)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<DownloadStoredFileDto>> Download(Guid id)
    {
        var result = await AppService.DownloadAsync(id);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosya icin dogrudan depolamaya giden sureli indirme baglantisi uretir.
    /// </summary>
    /// <param name="id">Presigned indirme baglantisi uretilecek StoredFile kaydinin kimligi.</param>
    /// <remarks>
    /// Response value kayit Id, imzali Url ve mutlak ExpiresAt zamanini tasir. Dosya byte'lari
    /// uygulama tarafindan okunmaz; istemci icerigi dogrudan MinIO'dan indirir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.PresignedUrl)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<PresignedUrlDto>> GetPresignedUrl(Guid id)
    {
        var result = await AppService.GetPresignedUrlAsync(id);
        return result;
    }

    /// <summary>
    /// Bir is kaydinin verilen kategorideki tum dosyalari icin sureli indirme baglantilari uretir.
    /// </summary>
    /// <param name="externalId">Dosyalarin bagli oldugu is kaydi kimligi.</param>
    /// <param name="categoryId">Dosyalarin ait oldugu kategori kimligi.</param>
    /// <remarks>
    /// Response value elemanlari kayit Id, OriginalFileName, imzali Url ve mutlak ExpiresAt
    /// zamanini tasir. Eslesen kayit yoksa bos liste doner. Dosya byte'lari uygulama tarafindan
    /// okunmaz; istemci her icerigi dogrudan MinIO'dan indirir.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ByExternalIdAndCategoryPresignedUrls)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<PresignedUrlDto>>> GetPresignedUrlsByExternalIdAndCategory(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId)
    {
        var result = await AppService.GetPresignedUrlsByExternalIdAndCategoryAsync(externalId, categoryId);
        return result;
    }

    /// <summary>
    /// Dosya byte'i almadan metadata kaydi ve sureli dogrudan yukleme baglantisi uretir.
    /// </summary>
    /// <param name="input">Is kaydi, kategori ve ozgun dosya adi.</param>
    /// <remarks>
    /// Request byte icermez. Response value yeni kaydin Id, imzali PUT Url ve mutlak ExpiresAt
    /// zamanini tasir; istemci byte'lari dogrudan MinIO'ya yollar. Yukleme tamamlanmazsa kalan
    /// iceriksiz metadata kaydinin temizligi bu endpointin degil, F6b oksuz taramasinin isidir.
    /// </remarks>
    [HttpPost(StoredFileRoutes.PresignedUpload)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result<PresignedUrlDto>> CreatePresignedUpload(
        [FromBody] PresignedUploadStoredFileDto input)
    {
        var result = await AppService.CreatePresignedUploadAsync(input);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosyayi tarayicinin dogrudan indirebilecegi HTTP dosya cevabi olarak getirir.
    /// </summary>
    /// <param name="id">Indirilecek StoredFile kaydinin kimligi.</param>
    /// <remarks>
    /// Response govdesi dosyanin ham byte'laridir; Result zarfi veya Base64 JSON bulunmaz.
    /// Content-Type ve Content-Disposition basliklari kayitli dosya adiyla olusturulur.
    /// </remarks>
    [HttpGet(StoredFileRoutes.DownloadContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<FileContentResult> DownloadContent(Guid id)
    {
        var file = await AppService.DownloadAsync(id);
        return File(file.Data, file.ContentType, file.OriginalFileName);
    }

    /// <summary>
    /// Kimligi verilen dosyanin metadata kaydini ve blob icerigini siler.
    /// </summary>
    /// <param name="id">Silinecek StoredFile kaydinin kimligi.</param>
    /// <remarks>
    /// File modulu once StoredFile kaydini, ardindan ayni kaydin BlobName degeriyle fiziksel icerigi
    /// siler ve basarili istekte iceriksiz cevap dondurur.
    /// </remarks>
    [HttpDelete(StoredFileRoutes.ById)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result> Delete(Guid id)
    {
        await AppService.DeleteAsync(id);
        return Result.NoContent();
    }

    /// <summary>
    /// Kimligi verilen dosyanin metadata kaydini getirir.
    /// </summary>
    /// <param name="id">Getirilecek StoredFile kaydinin kimligi.</param>
    /// <remarks>
    /// Response value dosya Id, ExternalId, kapsam, kategori, modul, ozgun dosya adi ve audit
    /// alanlarini tasir. Dosya byte'lari bu endpointte okunmaz.
    /// </remarks>
    [HttpGet(StoredFileRoutes.ById)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<StoredFileDto>> Get(Guid id)
    {
        var result = await AppService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Dosya metadata kayitlarini filtre, siralama ve sayfalama uygulayarak getirir.
    /// </summary>
    /// <param name="input">StoredFile filtre, siralama ve sayfalama bilgileri.</param>
    /// <remarks>
    /// ModuleName, ExternalId, CategoryId, TenantId ve CreationTime alanlari tipli filtre olarak
    /// kullanilabilir. Response value StoredFile metadata listesidir; byte icerigi dondurulmez.
    /// </remarks>
    [HttpPost(StoredFileRoutes.FilteredList)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetFilteredList(
        [FromBody] StoredFileListInput input)
    {
        var result = await AppService.GetFilteredListAsync(input);
        return result;
    }

    /// <summary>
    /// Verilen filtrelere uyan dosya metadata kayitlarinin toplam sayisini getirir.
    /// </summary>
    /// <param name="input">Sayimda kullanilacak StoredFile filtre bilgileri.</param>
    /// <remarks>
    /// Liste endpointiyle ayni filtre kurallari uygulanir; response value eslesen kayit sayisidir.
    /// </remarks>
    [HttpPost(StoredFileRoutes.FilteredCount)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<int>> GetFilteredCount(
        [FromBody] StoredFileListInput input)
    {
        var result = await AppService.GetFilteredCountAsync(input);
        return result;
    }
}
