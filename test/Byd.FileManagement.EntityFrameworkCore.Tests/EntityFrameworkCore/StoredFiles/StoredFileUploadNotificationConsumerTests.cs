using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Etos.StoredFiles;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Models.StoredFiles;
using Byd.FileManagement.Services.StoredFiles;
using Shouldly;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// MinIO yukleme bildirimlerinin bekleyen presigned yuklemeleri tamamlamasini dogrular.
/// </summary>
// islevi: Basari ve tamamlanma olayi, boyut reddi, bilinmeyen nesne, bozuk mesaj, yeniden teslim ve silinen kategori akislarini sinar.
// sistemdeki gorevi: Ham RabbitMQ consumer'inin kalicilik ve onbellek sozlesmesini gercek PostgreSQL ile korur.
public class StoredFileUploadNotificationConsumerTests : FileManagementEntityFrameworkCoreTestBase
{
    private readonly IDistributedEventBus _distributedEventBus = Substitute.For<IDistributedEventBus>();

    // Yayinlanan olaylari RabbitMQ'ya gondermeden dogrulamak icin dagitik olay yolunu sahteyle degistirir.
    protected override void AfterAddApplication(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(_distributedEventBus));
    }

    // Gecerli bildirimin satiri actigini ve bekleyen bilgiyi sildigini dogrular.
    [Fact]
    public async Task Should_create_row_and_remove_cache_after_upload_event()
    {
        var pending = await CreatePendingAsync();

        await NotifyAsync(pending.BlobName, 123);

        await WithUnitOfWorkAsync(async () =>
        {
            (await Repository.FindAsync(entity => entity.Id == pending.Id)).ShouldNotBeNull();
            (await Cache.GetAsync(pending.BlobName)).ShouldBeNull();
        });
        await _distributedEventBus.Received(1).PublishAsync(
            Arg.Is<StoredFileUploadCompletedEto>(completed => completed.Id == pending.Id),
            Arg.Any<bool>(),
            Arg.Any<bool>());
    }

    // Boyut sinirini asan bildirimin satir acmadan bekleyen bilgiyi sildigini dogrular.
    [Fact]
    public async Task Should_reject_oversized_upload_and_remove_cache()
    {
        var pending = await CreatePendingAsync();

        await NotifyAsync(pending.BlobName, StoredFileConsts.MaxDataLength + 1L);

        await WithUnitOfWorkAsync(async () =>
        {
            (await Repository.FindAsync(entity => entity.Id == pending.Id)).ShouldBeNull();
            (await Cache.GetAsync(pending.BlobName)).ShouldBeNull();
        });
        await _distributedEventBus.DidNotReceiveWithAnyArgs().PublishAsync(default(StoredFileUploadCompletedEto)!);
    }

    // Onbellekte karsiligi olmayan blob bildiriminin bos islem oldugunu dogrular.
    [Fact]
    public async Task Should_ignore_blob_without_pending_cache_entry()
    {
        await NotifyAsync(Guid.NewGuid().ToString("N"), 123);

        await WithUnitOfWorkAsync(async () =>
            (await Repository.GetCountAsync()).ShouldBe(0));
    }

    // Bozuk JSON bildiriminin hata firlatmadan atlandigini dogrular.
    [Fact]
    public async Task Should_ignore_malformed_json()
    {
        await Consumer.ProcessAsync("{"u8.ToArray());
    }

    // Ayni bildirimin yeniden tesliminde ikinci satir acilmadigini dogrular.
    [Fact]
    public async Task Should_create_single_row_on_redelivery()
    {
        var pending = await CreatePendingAsync();

        await NotifyAsync(pending.BlobName, 123);
        await NotifyAsync(pending.BlobName, 123);

        await WithUnitOfWorkAsync(async () =>
        {
            var files = await Repository.GetListByExternalIdsAsync([pending.ExternalId]);
            files.ShouldHaveSingleItem().Id.ShouldBe(pending.Id);
        });
    }

    // Olaydan once silinen kategorinin satir acilmasini engelledigini dogrular.
    [Fact]
    public async Task Should_reject_upload_when_category_was_deleted()
    {
        var pending = await CreatePendingAsync();
        await DeleteCategoryAsync(pending.CategoryId);

        await NotifyAsync(pending.BlobName, 123);

        await WithUnitOfWorkAsync(async () =>
        {
            (await Repository.FindAsync(entity => entity.Id == pending.Id)).ShouldBeNull();
            (await Cache.GetAsync(pending.BlobName)).ShouldBeNull();
        });
    }

    private StoredFileUploadNotificationConsumer Consumer =>
        GetRequiredService<StoredFileUploadNotificationConsumer>();
    private IStoredFileRepository Repository => GetRequiredService<IStoredFileRepository>();
    private IDistributedCache<StoredFilePendingUploadModel> Cache =>
        GetRequiredService<IDistributedCache<StoredFilePendingUploadModel>>();

    // Yeni kategori ve presigned istek acip onbellekteki modeli dondurur.
    private Task<StoredFilePendingUploadModel> CreatePendingAsync()
        => WithUnitOfWorkAsync(async () =>
        {
            var category = await FileCategoryTestDataBuilder.InsertAsync(
                GetRequiredService<FileCategoryManager>(),
                GetRequiredService<IFileCategoryRepository>());
            var result = await GetRequiredService<IStoredFileAppService>().CreatePresignedUploadAsync(
                new PresignedUploadStoredFileDto
                {
                    ExternalId = Guid.NewGuid(),
                    CategoryId = category.Id,
                    FileName = "olay.pdf"
                });
            var blobName = GetBlobName(result.Url);
            return (await Cache.GetAsync(blobName))!;
        });

    // Verilen blob adina MinIO biciminde yazma bildirimi yollar.
    private Task NotifyAsync(string blobName, long size)
    {
        var json = $$$$"""{"EventName":"s3:ObjectCreated:Put","Records":[{"s3":{"object":{"key":"host%2F{{{{blobName}}}}","size":{{{{size}}}}}}}]}""";
        return Consumer.ProcessAsync(Encoding.UTF8.GetBytes(json));
    }

    // Imzali URL yolunun son parcasindan opak blob adini alir.
    private static string GetBlobName(string url)
    {
        var path = Uri.UnescapeDataString(new Uri(url).AbsolutePath);
        return path[(path.LastIndexOf('/') + 1)..];
    }

    // Bekleyen yuklemenin kategorisini ayri unit of work icinde siler.
    private Task DeleteCategoryAsync(Guid categoryId)
        => WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IFileCategoryRepository>();
            var category = await repository.FindAsync(entity => entity.Id == categoryId);
            await repository.DeleteAsync(category!, autoSave: true);
        });
}
