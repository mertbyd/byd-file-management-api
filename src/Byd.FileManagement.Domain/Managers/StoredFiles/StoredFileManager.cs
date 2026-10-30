using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Byd.CoreExtensions.Interfaces;
using Byd.CoreExtensions.Managers;
using Byd.CoreExtensions.Providers;
using Byd.FileManagement;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Models.StoredFiles;
using Volo.Abp;
using Volo.Abp.Caching;

namespace Byd.FileManagement.Managers.StoredFiles;

/// <summary>
/// Saklanan dosya kurallarini ve varlik guard'larini yonetir.
/// </summary>
// islevi: Yazma on kosullarini dogrular, dosya adini kanoniklestirir, modul adini host
// beyanindan yazar, blob adini uretir ve content type'in cozulebilirligini dogrular.
// sistemdeki gorevi: AppService'i is kararindan ve data-shell entity'yi davranistan uzak tutan
// domain sahibidir. Byte yazma/silme burada degildir; o is depolama sinirinin isidir ve Domain
// hicbir saglayici bagimliligi tasimaz.
public class StoredFileManager(
    IStoredFileRepository repository,
    FileCategoryManager categoryManager)
    : BaseManager<StoredFile, Guid>(repository)
{
    private CurrentModuleNameProvider ModuleNameProvider =>
        LazyServiceProvider.LazyGetRequiredService<CurrentModuleNameProvider>();

    private ICurrentOrganization CurrentOrganization =>
        LazyServiceProvider.LazyGetRequiredService<ICurrentOrganization>();

    private IDistributedCache<StoredFilePendingUploadModel> PendingUploadCache =>
        LazyServiceProvider.LazyGetRequiredService<IDistributedCache<StoredFilePendingUploadModel>>();

    // Kimligi verilen kaydi getirir; yoksa BaseManager lokalize NotFound firlatir.
    public virtual async Task<StoredFile> EnsureExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => (await TryGetByIdAsync(id, throwIfNotExists: true, cancellationToken))!;

    // Yazma on kosulu: dosyanin baglanacagi kategori gercekten var olmali.
    // Create modelini normalize edip byte'lari henuz yazilmamis yeni kaydi kurar.
    public virtual async Task<StoredFile> CreateAsync(
        StoredFileCreateModel model,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        await categoryManager.EnsureExistsAsync(model.CategoryId, cancellationToken);

        var id = GuidGenerator.Create();
        var originalFileName = NormalizeRequiredText(model.FileName);
        EnsureContentTypeResolvable(originalFileName);
        return new StoredFile(id)
        {
            // Modul adi cagrandan alinmaz; host'un beyan ettigi tek kaynaktan okunur.
            ModuleName = NormalizeRequiredText(ModuleNameProvider.Value),
            ExternalId = model.ExternalId,
            CategoryId = model.CategoryId,
            // Blob adi opak ve carpismasizdir; orijinal ad hicbir zaman adresleme icin kullanilmaz.
            BlobName = id.ToString("N"),
            OriginalFileName = originalFileName,
            // Kapsam cagirandan alinmaz; tenant ve organizasyon ortam baglamindan okunur. ABP de
            // IMultiTenant kaydi eklerken ayni tenant'i yazar; burada aciktan yazilir ki Manager'in
            // dondurdugu entity kaydedilmeden once de kendi kapsamini tasisin.
            TenantId = CurrentTenant.Id,
            OrganizationId = CurrentOrganization.Id
        };
    }

    // Bekleyen yuklemeden kaydi kurar. Kategori bu arada silindiyse ya da yazilan icerik kategorinin
    // sinirini asiyorsa null doner. Ad normalizasyonu ve icerik tipi kontrolu istek aninda CreateAsync'te
    // yapildi; burada tekrarlanmaz.
    public virtual async Task<StoredFile?> TryCreateFromPendingUploadAsync(
        StoredFilePendingUploadModel model,
        long sizeBytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        var category = await categoryManager.TryGetByIdAsync(
            model.CategoryId,
            throwIfNotExists: false,
            cancellationToken);
        if (category is null || sizeBytes > (category.MaxFileSize ?? StoredFileConsts.MaxDataLength))
        {
            return null;
        }

        return new StoredFile(model.Id)
        {
            TenantId = model.TenantId,
            OrganizationId = model.OrganizationId,
            CategoryId = model.CategoryId,
            ExternalId = model.ExternalId,
            ModuleName = model.ModuleName,
            BlobName = model.BlobName,
            OriginalFileName = model.OriginalFileName
        };
    }

    // Bekleyen yukleme bilgisini baglanti suresi ve olay gecikme payi boyunca saklar.
    public virtual async Task RememberPendingUploadAsync(
        StoredFilePendingUploadModel model,
        DateTime expiresAt,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
                expiresAt - Clock.Now + StoredFileConsts.PendingUploadCacheMargin
        };
        await PendingUploadCache.SetAsync(model.BlobName, model, options, token: cancellationToken);
    }

    // Icerik turu kolonda tutulmaz; her okumada dosya adindan uretilir. Cozulemeyen uzanti yazma aninda
    // reddedildigi icin burada ikinci bir dogrulama yapilmaz.
    public virtual string ResolveContentType(string originalFileName)
        => MimeTypes.GetMimeType(originalFileName);

    // Icerik tipi kolonda tutulmaz; her okumada dosya adindan cozulur. Bu yuzden cozulemeyen
    // bir ad kabul edilirse dosya sonradan dogru tiple sunulamaz, yazma aninda reddedilir.
    private static void EnsureContentTypeResolvable(string originalFileName)
    {
        if (!MimeTypes.TryGetMimeType(originalFileName, out _))
        {
            throw new BusinessException(StoredFileExceptionCodes.UnresolvedContentType);
        }
    }
}
