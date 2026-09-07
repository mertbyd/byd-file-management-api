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

    // Kimligi verilen dosya kategorisini getirir.
    [HttpGet(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.View)]
    public virtual async Task<Result<FileCategoryDto>> Get(Guid id)
    {
        var result = await AppService.GetAsync(id);
        return result;
    }

    // Aktiflik filtresine uyan dosya kategorilerini getirir.
    [HttpGet]
    [Authorize(FileManagementPermissions.FileCategories.View)]
    public virtual async Task<Result<List<FileCategoryDto>>> GetList()
    {
        var result = await AppService.GetListAsync();
        return result;
    }

    // Yeni bir dosya kategorisi olusturur.
    [HttpPost]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result<FileCategoryDto>> Create(
        [FromBody] CreateFileCategoryDto input)
    {
        var result = await AppService.CreateAsync(input);
        return result;
    }

    // Kimligi verilen dosya kategorisini gunceller.
    [HttpPut(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result<FileCategoryDto>> Update(
        Guid id,
        [FromBody] UpdateFileCategoryDto input)
    {
        var result = await AppService.UpdateAsync(id, input);
        return result;
    }

    // Kimligi verilen dosya kategorisini siler.
    [HttpDelete(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result> Delete(Guid id)
    {
        await AppService.DeleteAsync(id);
        return Result.NoContent();
    }
}
