using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Etos.StoredFiles;
using RabbitMQ.Client;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.RabbitMQ;

namespace Byd.FileManagement.BlobStoring;

/// <summary>
/// MinIO nesne yazma bildirimlerini dinleyip yerel yukleme olayina cevirir.
/// </summary>
// islevi: MinIO exchange'ine bagli kuyrugu dinler, bildirimi cozer ve her yazilan nesne icin
// StoredFileUploadedEto yayinlar.
// sistemdeki gorevi: RabbitMQ ile modulun is akisi arasindaki tasima siniridir; kayit kararlari
// StoredFileUploadedEventHandler'dadir.
//
// NEDEN DAGITIK HANDLER DEGIL. MinIO mesaji MessageId tasimaz; core'un gelen kutusunda message_id
// NOT NULL oldugu icin ABP'nin dagitik olay hattina giremez, ham consumer ile dinlenir.
//
// HATA OLURSA. Handler'daki hata burada yakalanmaz; ABP mesaji nack'leyip yeniden kuyruga koyar.
public class StoredFileUploadNotificationConsumer(
    IRabbitMqMessageConsumerFactory consumerFactory,
    IOptions<AbpRabbitMqEventBusOptions> eventBusOptions,
    ILocalEventBus localEventBus)
    : ISingletonDependency
{
    // MinIO exchange'ini ve uygulamaya ozel kuyrugu acar, bucket routing key'ine baglar.
    public virtual async Task StartAsync(string bucketName)
    {
        var consumer = consumerFactory.Create(
            new ExchangeDeclareConfiguration(
                StoredFileConsts.UploadNotificationExchangeName,
                ExchangeType.Direct,
                durable: true),
            new QueueDeclareConfiguration(
                eventBusOptions.Value.ClientName + StoredFileConsts.UploadNotificationQueueSuffix,
                durable: true));
        consumer.OnMessageReceived((_, args) => ProcessAsync(args.Body));
        await consumer.BindAsync(bucketName);
    }

    // Bildirimdeki her nesne icin yukleme olayini hemen isletir; cozulemeyen govde atlanir.
    public virtual async Task ProcessAsync(ReadOnlyMemory<byte> body)
    {
        if (!MinioObjectCreatedNotification.TryParse(body.Span, out var notification))
        {
            return;
        }
        foreach (var storedObject in notification.Objects)
        {
            await localEventBus.PublishAsync(
                new StoredFileUploadedEto(storedObject.BlobName, storedObject.Size),
                onUnitOfWorkComplete: false);
        }
    }
}
