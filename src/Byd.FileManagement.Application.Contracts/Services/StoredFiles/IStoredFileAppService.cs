using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Byd.FileManagement.Dtos.StoredFiles;
using Volo.Abp.Application.Services;

namespace Byd.FileManagement.Services.StoredFiles;

/// <summary>
/// Saklanan dosya kullanim senaryolarini tanimlar.
/// </summary>
// islevi: Okuma, yukleme, indirme ve silme kontratini acar.
// sistemdeki gorevi: Controller ile Application implementasyonu arasindaki public servis siniridir.
public interface IStoredFileAppService : IApplicationService
{
    // Kimligi verilen saklanan dosya kaydini getirir.
    Task<StoredFileDto> GetAsync(Guid id);

    // Verilen is kayitlarinin dosyalarini toplu getirir.
    Task<List<StoredFileDto>> GetListByExternalIdsAsync(IEnumerable<Guid> externalIds);

    // Bir is kaydinin verilen kategorideki dosyalarini getirir.
    Task<List<StoredFileDto>> GetListByCategoryAsync(Guid externalId, Guid categoryId);

    // Verilen kategorideki dosyalari metadata ve byte icerikleriyle getirir.
    Task<List<StoredFileWithContentDto>> GetListWithContentByCategoryAsync(Guid categoryId);

    // Bir is kaydinin verilen kategorideki dosyalarini metadata ve byte icerikleriyle getirir.
    Task<List<StoredFileWithContentDto>> GetListWithContentByExternalIdAndCategoryAsync(
        Guid externalId,
        Guid categoryId);

    // Filtre, siralama ve sayfalama uygulanmis saklanan dosya listesini getirir.
    Task<List<StoredFileDto>> GetFilteredListAsync(StoredFileListInput input);

    // Ayni filtreye uyan toplam kayit sayisini getirir.
    Task<int> GetFilteredCountAsync(StoredFileListInput input);

    // Byte'lari depoya yazar ve okunabilir dosya kaydini dondurur.
    Task<StoredFileDto> UploadAsync(UploadStoredFileDto input);

    // Kimligi verilen dosyanin byte'larini depodan okur.
    Task<DownloadStoredFileDto> DownloadAsync(Guid id);

    // Kimligi verilen dosyanin kaydini ve byte'larini siler.
    Task DeleteAsync(Guid id);
}
