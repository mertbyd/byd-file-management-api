using System;
using System.Collections.Generic;
using System.Threading;
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
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response {
    ///   Id           (Guid)     → Kayit Id'si
    ///   Name         (string)   → Kategori adi; dosyalarin adreslendigi tekil anahtar
    ///   Code         (int?)     → Istege bagli sayisal kod
    ///   CreationTime (DateTime) → Olusturulma zamani
    /// }
    /// Kategori, StoredFile kayitlarinin hangi belge turune ait oldugunu belirleyen metadata kaydidir.
    /// </remarks>
    [HttpGet(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.View)]
    public virtual async Task<Result<FileCategoryDto>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetAsync(id, cancellationToken);
        return result;
    }

    /// <summary>
    /// Kullanilabilir dosya kategorilerinin tamamini getirir.
    /// </summary>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response elemanlari tek kayit ucuyla ayni alanlari tasir; kayit yoksa bos liste doner.
    /// </remarks>
    [HttpGet]
    [Authorize(FileManagementPermissions.FileCategories.View)]
    public virtual async Task<Result<List<FileCategoryDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await AppService.GetListAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// Yeni bir dosya kategorisi olusturur.
    /// </summary>
    /// <param name="input">Kategori adi ve istege bagli sayisal kodu.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request {
    ///   Name (string) → Kategori adi; sistemde tekil olmalidir
    ///   Code (int?)   → Istege bagli sayisal kod
    /// }
    /// Response tek kayit ucuyla ayni alanlari tasir. Ayni adla ikinci kategori acilamaz.
    /// </remarks>
    [HttpPost]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result<FileCategoryDto>> CreateAsync(
        [FromBody] CreateFileCategoryDto input,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.CreateAsync(input, cancellationToken);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosya kategorisinin adini ve kodunu gunceller.
    /// </summary>
    /// <param name="id">Guncellenecek FileCategory kaydinin kimligi.</param>
    /// <param name="input">Yeni kategori adi ve istege bagli sayisal kodu.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request olusturma istegiyle ayni alanlari tasir. File modulu kaydin varligini ve kategori adinin
    /// tekilligini denetler. Response guncel kaydi tek kayit ucuyla ayni alanlarla tasir.
    /// </remarks>
    [HttpPut(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result<FileCategoryDto>> UpdateAsync(
        Guid id,
        [FromBody] UpdateFileCategoryDto input,
        CancellationToken cancellationToken = default)
    {
        var result = await AppService.UpdateAsync(id, input, cancellationToken);
        return result;
    }

    /// <summary>
    /// Kimligi verilen dosya kategorisini siler.
    /// </summary>
    /// <param name="id">Silinecek FileCategory kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Kategoriye bagli StoredFile kaydi varsa iliski kurali silmeyi engeller. Basarili istekte
    /// iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpDelete(FileCategoryRoutes.ById)]
    [Authorize(FileManagementPermissions.FileCategories.Manage)]
    public virtual async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await AppService.DeleteAsync(id, cancellationToken);
        return Result.NoContent();
    }
}
