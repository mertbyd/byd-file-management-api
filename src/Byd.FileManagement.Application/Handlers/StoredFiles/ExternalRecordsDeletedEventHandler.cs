using System.Collections.Generic;
using System.Threading.Tasks;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Etos.StoredFiles;
using Byd.FileManagement.Interfaces.StoredFiles;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Byd.FileManagement.Handlers.StoredFiles;

/// <summary>
/// Silinen is kayitlarina bagli dosya satirlarini ve byte'larini temizler.
/// </summary>
// islevi: ExternalRecordsDeletedEto olayindaki dis kimliklerin dosyalarini tenant sinirinda toplu bulur ve siler.
// sistemdeki gorevi: Is modullerindeki kayit silme olayi ile dosya modulunun fiziksel temizligi arasindaki handler'dir.
//
// NEDEN ONCE BYTE. Gelen kutu hata veren olayi yeniden dener. Satir once silinirse ve byte silme
// patlarsa, ikinci denemede satir bulunmaz ve byte sonsuza kadar kalir. Byte once silinirse ikinci
// deneme satiri yine bulur. Olmayan byte icin DeleteAsync false doner ve is tamamlanir. HTTP silme
// yolunun tersidir; orada yeniden deneme yoktur.
//
// NEDEN TENANT DEGISIYOR. Gelen kutu isleyicisinde ortam tenant'i yoktur. Olaydaki tenant'a gecilir
// ve tenant filtresi baska tenant'in dosyasina dokunulmasini engeller.
//
// IKI KEZ GELIRSE. Ikinci teslimde satir yoktur, hicbir sey yapilmaz.
//
// YUKLEMESI SUREN DOSYA. Presigned yuklemede satir yukleme bitince acilir. Yukleme surerken is kaydi
// silinirse, yukleme bittiginde satir yine acilir. Bu bilinen ve kabul edilmis bir kenar durumdur.
public class ExternalRecordsDeletedEventHandler(
    IStoredFileRepository repository,
    IBlobContainer<StoredFileBlobContainer> blobContainer,
    ICurrentTenant currentTenant)
    : IDistributedEventHandler<ExternalRecordsDeletedEto>, ITransientDependency
{
    // Olay tenant'ina gecip eslesen dosyalarin byte ve metadata kayitlarini temizler.
    [UnitOfWork]
    public virtual async Task HandleEventAsync(ExternalRecordsDeletedEto eventData)
    {
        using (currentTenant.Change(eventData.TenantId))
        {
            var files = await repository.GetListByExternalIdsAsync(eventData.ExternalIds);
            await RemoveAsync(files);
        }
    }

    // Yeniden teslimde byte artigi kalmamasi icin once icerikleri, sonra satirlari siler.
    private async Task RemoveAsync(List<StoredFile> files)
    {
        foreach (var file in files)
        {
            await blobContainer.DeleteAsync(file.BlobName);
        }
        await repository.DeleteManyAsync(files, autoSave: true);
    }
}
