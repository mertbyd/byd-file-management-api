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
/// Saklanan dosya metadata okuma islemlerini HTTP uzerinden sunar.
/// </summary>
// islevi: Route ve request binding metadatasini tasiyip her istegi tek AppService cagrisina yonlendirir.
// sistemdeki gorevi: StoredFile dikey kesitinin salt okunur ince HTTP giris noktasidir.
[ApiController]
[Route(StoredFileRoutes.Root)]
public class StoredFileController : FileManagementController
{
    /// <summary>StoredFile AppService'ini lazy cozer.</summary>
    private IStoredFileAppService AppService =>
        LazyGetRequiredService<IStoredFileAppService>();

    // Verilen is kayitlarinin butun dosyalarini getirir.
    [HttpPost(StoredFileRoutes.ByExternalIds)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetListByExternalIds(
        [FromBody] List<Guid> externalIds)
    {
        var result = await AppService.GetListByExternalIdsAsync(externalIds);
        return result;
    }

    // Bir is kaydinin verilen kategorideki dosyalarini getirir.
    [HttpGet(StoredFileRoutes.ByCategory)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetListByCategory(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId)
    {
        var result = await AppService.GetListByCategoryAsync(externalId, categoryId);
        return result;
    }

    // Byte'lari depoya yazar ve okunabilir dosya kaydini dondurur.
    [HttpPost(StoredFileRoutes.Upload)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result<StoredFileDto>> Upload([FromBody] UploadStoredFileDto input)
    {
        var result = await AppService.UploadAsync(input);
        return result;
    }

    // Kimligi verilen dosyanin byte'larini depodan okur.
    [HttpGet(StoredFileRoutes.Download)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<DownloadStoredFileDto>> Download(Guid id)
    {
        var result = await AppService.DownloadAsync(id);
        return result;
    }

    // Dosyayi tarayicinin dogrudan kaydedebilecegi bicimde dondurur.
    // Yukaridaki uctan farki cevabin bicimi: orada byte'lar Result zarfinin icinde base64 metin olarak
    // gider ve cagiran kodun cozmesi gerekir; burada govde dosyanin kendisidir ve Content-Disposition
    // basligi tarayiciya kaydettirir. Modul ici cagrilar yukaridaki ucu kullanmaya devam eder.
    [HttpGet(StoredFileRoutes.DownloadContent)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<FileContentResult> DownloadContent(Guid id)
    {
        var file = await AppService.DownloadAsync(id);
        return File(file.Data, file.ContentType, file.OriginalFileName);
    }

    // Kimligi verilen dosyanin kaydini ve byte'larini siler.
    [HttpDelete(StoredFileRoutes.ById)]
    [Authorize(FileManagementPermissions.StoredFiles.Manage)]
    public virtual async Task<Result> Delete(Guid id)
    {
        await AppService.DeleteAsync(id);
        return Result.NoContent();
    }

    // Kimligi verilen saklanan dosya kaydini getirir.
    [HttpGet(StoredFileRoutes.ById)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<StoredFileDto>> Get(Guid id)
    {
        var result = await AppService.GetAsync(id);
        return result;
    }

    // Filtreli ve sayfalanmis saklanan dosya listesini getirir.
    [HttpPost(StoredFileRoutes.FilteredList)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<List<StoredFileDto>>> GetFilteredList(
        [FromBody] StoredFileListInput input)
    {
        var result = await AppService.GetFilteredListAsync(input);
        return result;
    }

    // Ayni filtreye uyan toplam saklanan dosya kaydi sayisini getirir.
    [HttpPost(StoredFileRoutes.FilteredCount)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<int>> GetFilteredCount(
        [FromBody] StoredFileListInput input)
    {
        var result = await AppService.GetFilteredCountAsync(input);
        return result;
    }
}
