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

    private StoredFilePresignedUrlProvider PresignedUrlProvider =>
        LazyGetRequiredService<StoredFilePresignedUrlProvider>();

    private IValidator<StoredFileListInput> ListValidator =>
        LazyGetRequiredService<IValidator<StoredFileListInput>>();

    private IValidator<UploadStoredFileDto> UploadValidator =>
        LazyGetRequiredService<IValidator<UploadStoredFileDto>>();

    private IValidator<PresignedUploadStoredFileDto> PresignedUploadValidator =>
        LazyGetRequiredService<IValidator<PresignedUploadStoredFileDto>>();

    // Kimligi verilen kaydi Manager guard'i uzerinden getirir.
    public virtual async Task<StoredFileDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        return Mapper.Map(entity);
    }

    // Verilen is kayitlarinin dosyalarini tek repository sorgusuyla getirip esler.
    public virtual async Task<List<StoredFileDto>> GetListByExternalIdsAsync(
        IEnumerable<Guid> externalIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(externalIds);
        var entities = await Repository.GetListByExternalIdsAsync(externalIds, cancellationToken);
        return Mapper.Map(entities);
    }

    // Is kaydinin verilen kategorideki dosyalarini getirip esler; bos kategori bos liste doner.
    public virtual async Task<List<StoredFileDto>> GetListByCategoryAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetListByCategoryAsync(
            externalId,
            categoryId,
            cancellationToken);
        return Mapper.Map(entities);
    }

    // Verilen kategorideki dosya kayitlarini tek sorguda, byte'larini blob depodan getirir.
    public virtual async Task<List<StoredFileWithContentDto>> GetListWithContentByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetListByCategoryIdAsync(categoryId, cancellationToken);

        return await ReadWithContentAsync(entities, cancellationToken);
    }

    // Bir is kaydinin verilen kategorideki dosya kayitlarini ve byte'larini getirir.
    public virtual async Task<List<StoredFileWithContentDto>> GetListWithContentByExternalIdAndCategoryAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetListByCategoryAsync(externalId, categoryId, cancellationToken);

        return await ReadWithContentAsync(entities, cancellationToken);
    }

    // Liste girdisini dogrular, dinamik sorguyu repository'de calistirir ve sonucu esler.
    public virtual async Task<List<StoredFileDto>> GetFilteredListAsync(
        StoredFileListInput input,
        CancellationToken cancellationToken = default)
    {
        await ListValidator.ValidateAndThrowAsync(input, cancellationToken);
        var entities = await Repository.GetFilteredListAsync(input, cancellationToken);
        return Mapper.Map(entities);
    }

    // Liste girdisini dogrular ve ayni filtreye uyan repository kayit sayisini dondurur.
    public virtual async Task<int> GetFilteredCountAsync(
        StoredFileListInput input,
        CancellationToken cancellationToken = default)
    {
        await ListValidator.ValidateAndThrowAsync(input, cancellationToken);
        return await Repository.GetFilteredCountAsync(input, cancellationToken);
    }

    // Girdiyi dogrular ve iki fazli yazma akisini calistirir.
    public virtual async Task<StoredFileDto> UploadAsync(
        UploadStoredFileDto input,
        CancellationToken cancellationToken = default)
    {
        await UploadValidator.ValidateAndThrowAsync(input, cancellationToken);
        // Satir once yazilir: byte yazimi olurse geride yalniz iceriksiz bir kayit kalir.
        // Ters sirada sahipsiz bir blob kalir ve kimse onu bulamaz.
        var entity = await Manager.CreateAsync(Mapper.Map(input), cancellationToken);
        await Repository.InsertAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        await BlobContainer.SaveAsync(entity.BlobName, input.Data, overrideExisting: false, cancellationToken);

        return Mapper.Map(entity);
    }

    // Kaydi guard'dan gecirir ve byte'lari depodan okuyup indirme sozlesmesine esler.
    public virtual async Task<DownloadStoredFileDto> DownloadAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        var data = await BlobContainer.GetAllBytesAsync(entity.BlobName, cancellationToken);

        return Mapper.Map(entity, data, Manager.ResolveContentType(entity.OriginalFileName));
    }

    // Kaydi guard'dan gecirip byte tasimayan sureli GET baglantisini dondurur.
    public virtual async Task<PresignedUrlDto> GetPresignedUrlAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        var (url, expiresAt) = await PresignedUrlProvider.CreateGetAsync(entity.BlobName, cancellationToken);

        return Mapper.MapToPresignedUrl(entity, url, expiresAt);
    }

    // Is kaydinin verilen kategorideki her dosyasi icin sureli GET baglantisini tek cagrida uretir.
    //
    // NEDEN LISTE. Bir is kaydinin ayni kategoride birden fazla dosyasi olabilir. Tekil uc ile
    // baglanti almak once liste ucundan kimlikleri cekmeyi, sonra her kimlik icin ayri istek
    // atmayi gerektirirdi. Imzalama yerel bir hesap oldugu icin dongu ag cagrisi yapmaz.
    public virtual async Task<List<PresignedUrlDto>> GetPresignedUrlsByExternalIdAndCategoryAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetListByCategoryAsync(
            externalId,
            categoryId,
            cancellationToken);

        var urls = new List<PresignedUrlDto>(entities.Count);
        foreach (var entity in entities)
        {
            var (url, expiresAt) = await PresignedUrlProvider.CreateGetAsync(entity.BlobName, cancellationToken);
            urls.Add(Mapper.MapToPresignedUrl(entity, url, expiresAt));
        }

        return urls;
    }

    // Kayit burada acilmaz. Bilgi onbellekte bekler; MinIO "yazildi" olayi gelince
    // StoredFileUploadNotificationConsumer satiri acar. Yukleme hic yapilmazsa onbellek suresi
    // dolar ve geride hicbir sey kalmaz.
    public virtual async Task<PresignedUrlDto> CreatePresignedUploadAsync(
        PresignedUploadStoredFileDto input,
        CancellationToken cancellationToken = default)
    {
        await PresignedUploadValidator.ValidateAndThrowAsync(input, cancellationToken);
        var entity = await Manager.CreateAsync(Mapper.Map(input), cancellationToken);
        var (url, expiresAt) = await PresignedUrlProvider.CreatePutAsync(entity.BlobName, cancellationToken);
        await Manager.RememberPendingUploadAsync(Mapper.MapToPendingUpload(entity), expiresAt, cancellationToken);

        return Mapper.MapToPresignedUrl(entity, url, expiresAt);
    }

    // Kimligi verilen kaydi ve byte'larini siler.
    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        // Once satir, sonra byte'lar silinir. Ters sirada silme yarim kalirsa kullanici var olmayan
        // bir dosyayi gosteren kayit gorur; bu sirada ise gorunmeyen, suprulebilir bir artik kalir.
        await Repository.DeleteAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        await BlobContainer.DeleteAsync(entity.BlobName, cancellationToken);
    }



    // Her kaydin blob byte'larini okuyup metadata ile ayni sirada birlestirir.
    private async Task<List<StoredFileWithContentDto>> ReadWithContentAsync(
        List<StoredFile> entities,
        CancellationToken cancellationToken = default)
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
