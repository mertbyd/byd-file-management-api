using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.StoredFiles;
using Byd.FileManagement.Mappers.StoredFiles;

namespace Byd.FileManagement.Services.StoredFiles;

/// <summary>
/// Saklanan dosya metadata okuma orkestrasyonunu yurutur.
/// </summary>
// islevi: Liste girdisini dogrular, Manager guard'larini ve repository sorgularini Mapperly
// ciktisiyla birlestirir.
// sistemdeki gorevi: Controller ile StoredFile Manager/Repository zinciri arasindaki tek public
// giris noktasidir.
public class StoredFileAppService : FileManagementAppService, IStoredFileAppService
{
    private static readonly StoredFileMapper Mapper = new();

    private StoredFileManager Manager =>
        LazyGetRequiredService<StoredFileManager>();

    private IStoredFileRepository Repository =>
        LazyGetRequiredService<IStoredFileRepository>();

    private IValidator<StoredFileListInput> ListValidator =>
        LazyGetRequiredService<IValidator<StoredFileListInput>>();

    // Kimligi verilen kaydi Manager guard'i uzerinden getirir.
    public virtual async Task<StoredFileDto> GetAsync(Guid id)
    {
        var entity = await Manager.EnsureExistsAsync(id, CancellationTokenProvider.Token);
        return Mapper.Map(entity);
    }

    // Verilen is kayitlarinin dosyalarini tek repository sorgusuyla getirip esler.
    public virtual async Task<List<StoredFileDto>> GetListByExternalIdsAsync(IEnumerable<Guid> externalIds)
    {
        ArgumentNullException.ThrowIfNull(externalIds);
        var entities = await Repository.GetListByExternalIdsAsync(externalIds, CancellationTokenProvider.Token);
        return Mapper.Map(entities);
    }

    // Is kaydinin verilen kategorideki dosyalarini getirip esler; bos kategori bos liste doner.
    public virtual async Task<List<StoredFileDto>> GetListByCategoryAsync(Guid externalId, Guid categoryId)
    {
        var entities = await Repository.GetListByCategoryAsync(
            externalId,
            categoryId,
            CancellationTokenProvider.Token);
        return Mapper.Map(entities);
    }

    // Liste girdisini dogrular, dinamik sorguyu repository'de calistirir ve sonucu esler.
    public virtual async Task<List<StoredFileDto>> GetFilteredListAsync(StoredFileListInput input)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        await ListValidator.ValidateAndThrowAsync(input, cancellationToken);
        var entities = await Repository.GetFilteredListAsync(input, cancellationToken);
        return Mapper.Map(entities);
    }

    // Liste girdisini dogrular ve ayni filtreye uyan repository kayit sayisini dondurur.
    public virtual async Task<int> GetFilteredCountAsync(StoredFileListInput input)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        await ListValidator.ValidateAndThrowAsync(input, cancellationToken);
        return await Repository.GetFilteredCountAsync(input, cancellationToken);
    }
}
