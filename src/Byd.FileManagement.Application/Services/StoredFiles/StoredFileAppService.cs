using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.StoredFiles;
using Byd.FileManagement.Mappers.StoredFiles;
using Volo.Abp.BlobStoring;

namespace Byd.FileManagement.Services.StoredFiles;

/// <summary>
/// Saklanan dosya okuma, yukleme, indirme ve silme orkestrasyonunu yurutur.
/// </summary>
// islevi: Girdi dogrulamasini, Manager kararlarini, repository yazmalarini ve blob depolama
// cagrilarini duz akisla birlestirir.
// sistemdeki gorevi: Controller ile StoredFile Manager/Repository/BlobStoring zinciri arasindaki
// tek public giris noktasidir.
public class StoredFileAppService : FileManagementAppService, IStoredFileAppService
{
    private static readonly StoredFileMapper Mapper = new();

    private StoredFileManager Manager =>
        LazyGetRequiredService<StoredFileManager>();

    private IStoredFileRepository Repository =>
        LazyGetRequiredService<IStoredFileRepository>();

    private IBlobContainer<StoredFileBlobContainer> BlobContainer =>
        LazyGetRequiredService<IBlobContainer<StoredFileBlobContainer>>();

    private IValidator<StoredFileListInput> ListValidator =>
        LazyGetRequiredService<IValidator<StoredFileListInput>>();

    private IValidator<UploadStoredFileDto> UploadValidator =>
        LazyGetRequiredService<IValidator<UploadStoredFileDto>>();

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

    // Verilen kategorideki dosya kayitlarini tek sorguda, byte'larini blob depodan getirir.
    public virtual async Task<List<StoredFileWithContentDto>> GetListWithContentByCategoryAsync(Guid categoryId)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        var entities = await Repository.GetListByCategoryIdAsync(categoryId, cancellationToken);

        return await ReadWithContentAsync(entities, cancellationToken);
    }

    // Bir is kaydinin verilen kategorideki dosya kayitlarini ve byte'larini getirir.
    public virtual async Task<List<StoredFileWithContentDto>> GetListWithContentByExternalIdAndCategoryAsync(
        Guid externalId,
        Guid categoryId)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        var entities = await Repository.GetListByCategoryAsync(externalId, categoryId, cancellationToken);

        return await ReadWithContentAsync(entities, cancellationToken);
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

    // Girdiyi dogrular ve iki fazli yazma akisini calistirir.
    public virtual async Task<StoredFileDto> UploadAsync(UploadStoredFileDto input)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        await UploadValidator.ValidateAndThrowAsync(input, cancellationToken);

        return await WriteAsync(input, cancellationToken);
    }

    // Kaydi guard'dan gecirir ve byte'lari depodan okuyup indirme sozlesmesine esler.
    public virtual async Task<DownloadStoredFileDto> DownloadAsync(Guid id)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        var data = await BlobContainer.GetAllBytesAsync(entity.BlobName, cancellationToken);

        return Mapper.Map(entity, data);
    }

    // Kimligi verilen kaydi ve byte'larini siler.
    public virtual async Task DeleteAsync(Guid id)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);

        await RemoveAsync(entity, cancellationToken);
    }

    // Satir once yazilir: byte yazimi olurse geride yalniz okunmayan bir Beklemede kayit kalir.
    // Ters sirada sahipsiz bir blob kalir ve kimse onu bulamaz.
    private async Task<StoredFileDto> WriteAsync(UploadStoredFileDto input, CancellationToken cancellationToken)
    {
        var entity = await Manager.CreateAsync(Mapper.Map(input), cancellationToken);
        await Repository.InsertAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        await BlobContainer.SaveAsync(entity.BlobName, input.Data, overrideExisting: false, cancellationToken);
        Manager.MarkAsApproved(entity);
        await Repository.UpdateAsync(entity, autoSave: true, cancellationToken: cancellationToken);

        return Mapper.Map(entity);
    }

    // Once satiri, sonra byte'lari siler. Ters sirada silme yarim kalirsa kullanici var olmayan
    // bir dosyayi gosteren kayit gorur; bu sirada ise gorunmeyen, suprulebilir bir artik kalir.
    private async Task RemoveAsync(StoredFile entity, CancellationToken cancellationToken)
    {
        await Repository.DeleteAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        await BlobContainer.DeleteAsync(entity.BlobName, cancellationToken);
    }

    // Her kaydin blob byte'larini okuyup metadata ile ayni sirada birlestirir.
    private async Task<List<StoredFileWithContentDto>> ReadWithContentAsync(
        List<StoredFile> entities,
        CancellationToken cancellationToken)
    {
        var files = new List<StoredFileWithContentDto>(entities.Count);
        foreach (var entity in entities)
        {
            var data = await BlobContainer.GetAllBytesAsync(entity.BlobName, cancellationToken);
            files.Add(Mapper.MapWithContent(entity, data));
        }

        return files;
    }
}
