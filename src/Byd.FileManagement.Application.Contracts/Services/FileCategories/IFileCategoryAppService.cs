using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Byd.FileManagement.Dtos.FileCategories;
using Volo.Abp.Application.Services;

namespace Byd.FileManagement.Services.FileCategories;

/// <summary>
/// Dosya kategorisi tam CRUD kullanim senaryolarini tanimlar.
/// </summary>
// islevi: FileCategory okuma, listeleme, olusturma, guncelleme ve silme kontratini acar.
// sistemdeki gorevi: Controller ile Application implementasyonu arasindaki public servis siniridir.
public interface IFileCategoryAppService : IApplicationService
{
    // Kimligi verilen dosya kategorisini getirir.
    Task<FileCategoryDto> GetAsync(Guid id);

    // Aktiflik filtresine uyan dosya kategorilerini getirir.
    Task<List<FileCategoryDto>> GetListAsync();

    // Dogrulanmis girdiden yeni dosya kategorisi olusturur.
    Task<FileCategoryDto> CreateAsync(CreateFileCategoryDto input);

    // Kimligi verilen dosya kategorisinin degistirilebilir alanlarini gunceller.
    Task<FileCategoryDto> UpdateAsync(Guid id, UpdateFileCategoryDto input);

    // Kimligi verilen dosya kategorisini siler.
    Task DeleteAsync(Guid id);
}
