using System.Threading;
using System.Threading.Tasks;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Etos.StoredFiles;
using Byd.FileManagement.Extensions.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.StoredFiles;
using Byd.FileManagement.Models.StoredFiles;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Byd.FileManagement.Handlers.StoredFiles;

/// <summary>
/// Depolamaya yazilan nesneyi bekleyen presigned yuklemeyle eslestirip dosya kaydini acar.
/// </summary>
// islevi: Onbellekteki bekleyen yuklemeyi bulur, boyut ve kategori kararlarini Manager'a sorar ve
// kaydi acar ya da yuklenen byte'i siler.
// sistemdeki gorevi: Presigned yolda satirin acildigi tek yerdir; kayit istek aninda degil, byte
// gercekten yazildiginda acilir.
//
// BYTE YOLUNDAN GELEN OLAY. UploadAsync de MinIO'ya yazar ve bildirim uretir; onbellekte karsiligi
// olmadigi icin burada hicbir sey yapilmaz.
//
// IKI KEZ GELIRSE. Satir onceki teslimde acildiysa tekrar acilmaz; yalnizca onbellek temizlenir.
//
// ONBELLEK NEDEN SONRA SILINIR. Silme unit of work'e baglidir. Commit patlarsa bilgi onbellekte
// kalir ve RabbitMQ'nun yeniden teslimi kaydi yine acabilir.
public class StoredFileUploadedEventHandler(
    IDistributedCache<StoredFilePendingUploadModel> pendingUploadCache,
    StoredFileManager manager,
    IStoredFileRepository repository,
    IBlobContainer<StoredFileBlobContainer> blobContainer,
    ICurrentTenant currentTenant,
    IDistributedEventBus distributedEventBus)
    : ILocalEventHandler<StoredFileUploadedEto>, ITransientDependency
{
    // Bekleyen yuklemenin tenant'ina gecip kaydi sonuclandirir ve bekleyen bilgiyi commit'e baglar.
    [UnitOfWork]
    public virtual async Task HandleEventAsync(StoredFileUploadedEto eventData)
    {
        var pending = await pendingUploadCache.GetAsync(eventData.BlobName);
        if (pending is null)
            return;
        using (currentTenant.Change(pending.TenantId))
            await CompleteAsync(pending, eventData.Size);
        await pendingUploadCache.RemoveAsync(eventData.BlobName, considerUow: true);
    }

    // Satir yoksa boyut ve kategori kararina gore kaydi acar ya da yuklemeyi reddeder.
    private async Task CompleteAsync(
        StoredFilePendingUploadModel pending,
        long size,
        CancellationToken cancellationToken = default)
    {
        if (await manager.TryGetByIdAsync(pending.Id, throwIfNotExists: false, cancellationToken) is not null)
            return;
        var entity = await manager.TryCreateFromPendingUploadAsync(pending, size, cancellationToken);
        if (entity is null)
        {
            await RejectAsync(pending, cancellationToken);
            return;
        }
        await repository.InsertAsync(entity, cancellationToken: cancellationToken);
        await distributedEventBus.PublishAsync(entity.ToUploadCompletedEto());
    }

    // Kabul edilmeyen yuklemenin byte'ini siler; kayit acilmadigi icin byte'in baska sahibi yoktur.
    private async Task RejectAsync(StoredFilePendingUploadModel pending, CancellationToken cancellationToken = default)
    {
        await blobContainer.DeleteAsync(pending.BlobName, cancellationToken);
    }
}
