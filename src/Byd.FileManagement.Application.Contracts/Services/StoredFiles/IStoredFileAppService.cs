using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Byd.FileManagement.Dtos.StoredFiles;
using Volo.Abp.Application.Services;

namespace Byd.FileManagement.Services.StoredFiles;

/// <summary>
/// Saklanan dosya metadata okuma kullanim senaryolarini tanimlar.
/// </summary>
// islevi: Tekil okuma, kategori okumasi, toplu okuma, filtreli liste ve sayim kontratini acar.
// sistemdeki gorevi: Controller ile Application implementasyonu arasindaki public servis siniridir.
public interface IStoredFileAppService : IApplicationService
{
    // Kimligi verilen saklanan dosya kaydini getirir.
    Task<StoredFileDto> GetAsync(Guid id);

    // Verilen is kayitlarinin dosyalarini toplu getirir.
    Task<List<StoredFileDto>> GetListByExternalIdsAsync(IEnumerable<Guid> externalIds);

    // Bir is kaydinin verilen kategorideki dosyalarini getirir.
    Task<List<StoredFileDto>> GetListByCategoryAsync(Guid externalId, Guid categoryId);

    // Filtre, siralama ve sayfalama uygulanmis saklanan dosya listesini getirir.
    Task<List<StoredFileDto>> GetFilteredListAsync(StoredFileListInput input);

    // Ayni filtreye uyan toplam kayit sayisini getirir.
    Task<int> GetFilteredCountAsync(StoredFileListInput input);
}
