using System;
using System.Threading;
using System.Threading.Tasks;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.CoreExtensions.Managers;
using Byd.CoreExtensions.Providers;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Models.StoredFiles;
using Volo.Abp;

namespace Byd.FileManagement.Managers.StoredFiles;

/// <summary>
/// Saklanan dosya kurallarini ve varlik guard'larini yonetir.
/// </summary>
// islevi: Yazma on kosullarini dogrular, dosya adini kanoniklestirir, modul adini host
// beyanindan yazar, blob adini uretir ve content type'in cozulebilirligini dogrular.
// sistemdeki gorevi: AppService'i is kararindan ve data-shell entity'yi davranistan uzak tutan
// domain sahibidir.
public class StoredFileManager(
    IStoredFileRepository repository,
    FileCategoryManager categoryManager)
    : BaseManager<StoredFile, Guid>(repository)
{
    private CurrentModuleNameProvider ModuleNameProvider =>
        LazyServiceProvider.LazyGetRequiredService<CurrentModuleNameProvider>();

    // Kimligi verilen kaydi getirir; yoksa BaseManager lokalize NotFound firlatir.
    public virtual async Task<StoredFile> EnsureExistsAsync(Guid id, CancellationToken cancellationToken)
        => (await TryGetByIdAsync(id, throwIfNotExists: true, cancellationToken))!;

    // Slot bos ise ortak lokalize NotFound hatasini firlatir.
    public virtual async Task<StoredFile> EnsureSlotExistsAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken)
        => await repository.GetBySlotAsync(externalId, categoryId, cancellationToken)
           ?? throw new BusinessException(GeneralExceptionCodes.EntityNotFound);

    // Yazma on kosulu: dosyanin baglanacagi kategori slotu gercekten var olmali.
    // Create modelini normalize edip yeni kaydi kurar.
    public virtual async Task<StoredFile> CreateAsync(
        StoredFileCreateModel model,
        CancellationToken cancellationToken)
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
            // Kapsam cagirandan alinmaz; tenant ortam baglamindan okunur.
            // OrganizationId su an bos kalir: ortam baglamini veren ICurrentOrganizationUnit
            // tuketmedigimiz referans core'da duruyor; kaynak baglanana kadar kolon null tasir.
            TenantId = CurrentTenant.Id
        };
    }

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
