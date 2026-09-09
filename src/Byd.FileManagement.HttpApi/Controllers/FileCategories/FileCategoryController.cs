using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.FileCategories;
using Byd.FileManagement.Permissions;
using Byd.FileManagement.Services.FileCategories;
using SystemStandards.Results;

namespace Byd.FileManagement.Controllers.FileCategories;

/// <summary>
/// Dosya kategorisi CRUD islemlerini HTTP uzerinden sunar.
/// </summary>
// islevi: Route ve request binding metadatasini tasiyip her istegi tek AppService cagrisina yonlendirir.
// sistemdeki gorevi: FileCategory tam CRUD dikey kesitinin ince HTTP giris noktasidir.
[ApiController]
[Route(FileCategoryRoutes.Root)]
public class FileCategoryController : FileManagementController
{
    /// <summary>FileCategory AppService'ini lazy cozer.</summary>
    private IFileCategoryAppService AppService =>
        LazyGetRequiredService<IFileCategoryAppService>();

    /// <summary>
    /// Kimligi verilen dosya kategorisini getirir.
    /// </summary>
    /// <param name="id">Getirilecek FileCategory kaydinin kimligi.</param>
    /// <remarks>
    /// Response value { Id, Name, Code, CreationTime }. Kategori, StoredFile kayitlarinin hangi
    /// belge turune ait oldugunu belirleyen metadata kaydidir.
    /// </remarks>
    [HttpGet(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.View)]
    public virtual async Task<Result<FileCategoryDto>> Get(Guid id)
    {
        var result = await AppService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Kullanilabilir dosya kategorilerinin tamamini getirir.
    /// </summary>
    /// <remarks>
    /// Response value elemanlari { Id, Name, Code, CreationTime } alanlarini tasir.
    /// </remarks>
    [HttpGet]
    [Authorize(FileManagementPermissions.FileCategories.View)]
    public virtual async Task<Result<List<FileCategoryDto>>> GetList()
    {
        var result = await AppService.GetListAsync();
        return result;
    }

    /// <summary>
    /// Yeni bir dosya kategorisi olusturur.
    /// </summary>
    /// <param name="input">Kategori adi ve istege bagli sayisal kodu.</param>
    /// <remarks>
    /// Request { Name, Code }. File modulu girdiyi dogrular, kategori adinin tekilligini denetler
    /// ve kalici kaydi olusturur. Response value { Id, Name, Code, CreationTime } alanlarini tasir.
    /// </remarks>
    [HttpPost]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result<FileCategoryDto>> Create(
        [FromBody] CreateFileCategoryDto input)
    {
        var result = await AppService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosya kategorisinin adini ve kodunu gunceller.
    /// </summary>
    /// <param name="id">Guncellenecek FileCategory kaydinin kimligi.</param>
    /// <param name="input">Yeni kategori adi ve istege bagli sayisal kodu.</param>
    /// <remarks>
    /// Request { Name, Code }. File modulu kaydin varligini ve kategori adinin tekilligini denetler.
    /// Response value guncel { Id, Name, Code, CreationTime } alanlarini tasir.
    /// </remarks>
    [HttpPut(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result<FileCategoryDto>> Update(
        Guid id,
        [FromBody] UpdateFileCategoryDto input)
    {
        var result = await AppService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosya kategorisini siler.
    /// </summary>
    /// <param name="id">Silinecek FileCategory kaydinin kimligi.</param>
    /// <remarks>
    /// Kategoriye bagli StoredFile kaydi varsa iliski kurali silmeyi engeller. Basarili istekte
    /// iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpDelete(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result> Delete(Guid id)
    {
        await AppService.DeleteAsync(id);
        return Result.NoContent();
    }
}
