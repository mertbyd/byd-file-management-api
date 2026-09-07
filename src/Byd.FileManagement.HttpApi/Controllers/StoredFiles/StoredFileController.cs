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

    // Is kaydi ve kategoriyle adreslenen tek dosyayi getirir.
    [HttpGet(StoredFileRoutes.Slot)]
    [Authorize(FileManagementPermissions.StoredFiles.View)]
    public virtual async Task<Result<StoredFileDto>> GetBySlot(
        [FromQuery] Guid externalId,
        [FromQuery] Guid categoryId)
    {
        var result = await AppService.GetBySlotAsync(externalId, categoryId);
        return result;
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
