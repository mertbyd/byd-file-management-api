using System;
using System.Threading.Tasks;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Etos.StoredFiles;
using Byd.FileManagement.Handlers.StoredFiles;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Services.StoredFiles;
using Shouldly;
using Volo.Abp.BlobStoring;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// Silinen dis kayit olayinin dosya satiri ve byte temizligini dogrular.
/// </summary>
// islevi: Handler'in hedef dosyalari sildigini, diger adresleri ve tenant'lari korudugunu kanitlar.
// sistemdeki gorevi: Dagitik olay yeniden teslimi ve tenant filtresinin gercek PostgreSQL uzerindeki guvencesidir.
public class ExternalRecordsDeletedEventHandlerTests : FileManagementEntityFrameworkCoreTestBase
{
    private static readonly byte[] Content = "olayla silinecek icerik"u8.ToArray();

    // Verilen dis kimlige ait satir ve blob iceriginin birlikte silindigini dogrular.
    [Fact]
    public async Task Should_delete_rows_and_blobs_of_given_external_ids()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var externalId = Guid.NewGuid();
            var uploaded = await UploadAsync(externalId, await CreateCategoryIdAsync());

            await HandleAsync(null, externalId);

            (await Repository.FindAsync(entity => entity.Id == uploaded.Id)).ShouldBeNull();
            (await BlobContainer.ExistsAsync(uploaded.Id.ToString("N"))).ShouldBeFalse();
        });
    }

    // Olay listesinde olmayan dis kimligin satir ve blob icerigine dokunulmadigini dogrular.
    [Fact]
    public async Task Should_not_touch_files_of_other_external_ids()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var categoryId = await CreateCategoryIdAsync();
            var deleted = await UploadAsync(Guid.NewGuid(), categoryId);
            var kept = await UploadAsync(Guid.NewGuid(), categoryId);

            await HandleAsync(null, deleted.ExternalId);

            (await Repository.FindAsync(entity => entity.Id == deleted.Id)).ShouldBeNull();
            (await Repository.FindAsync(entity => entity.Id == kept.Id)).ShouldNotBeNull();
            (await BlobContainer.ExistsAsync(kept.Id.ToString("N"))).ShouldBeTrue();
        });
    }

    // Ayni olayin ikinci tesliminin hata vermeden bos islem olmasini dogrular.
    [Fact]
    public async Task Should_be_no_op_on_redelivery()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var externalId = Guid.NewGuid();
            await UploadAsync(externalId, await CreateCategoryIdAsync());

            await HandleAsync(null, externalId);
            await HandleAsync(null, externalId);
        });
    }

    // Olay tenant'inin baska tenant'a ait dosya satiri ve byte'larini silemedigini dogrular.
    [Fact]
    public async Task Should_not_delete_files_of_another_tenant()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var ownerTenantId = Guid.NewGuid();
            var eventTenantId = Guid.NewGuid();
            var externalId = Guid.NewGuid();
            var categoryId = await CreateCategoryIdAsync();
            StoredFileDto uploaded;

            using (CurrentTenant.Change(ownerTenantId))
            {
                uploaded = await UploadAsync(externalId, categoryId);
            }

            await HandleAsync(eventTenantId, externalId);

            using (CurrentTenant.Change(ownerTenantId))
            {
                (await Repository.FindAsync(entity => entity.Id == uploaded.Id)).ShouldNotBeNull();
                (await BlobContainer.ExistsAsync(uploaded.Id.ToString("N"))).ShouldBeTrue();
            }
        });
    }

    private IStoredFileRepository Repository => GetRequiredService<IStoredFileRepository>();
    private IBlobContainer<StoredFileBlobContainer> BlobContainer =>
        GetRequiredService<IBlobContainer<StoredFileBlobContainer>>();
    private ICurrentTenant CurrentTenant => GetRequiredService<ICurrentTenant>();

    // Verilen tenant ve dis kimlikle handler'i dogrudan calistirir.
    private Task HandleAsync(Guid? tenantId, Guid externalId)
        => GetRequiredService<ExternalRecordsDeletedEventHandler>().HandleEventAsync(
            new ExternalRecordsDeletedEto
            {
                TenantId = tenantId,
                ExternalIds = [externalId]
            });

    // Verilen adrese gercek AppService akisi uzerinden dosya yukler.
    private Task<StoredFileDto> UploadAsync(Guid externalId, Guid categoryId)
        => GetRequiredService<IStoredFileAppService>().UploadAsync(
            new UploadStoredFileDto
            {
                ExternalId = externalId,
                CategoryId = categoryId,
                FileName = "olay.pdf",
                Data = Content
            });

    // Testin dosya baglayacagi kalici kategoriyi acar.
    private async Task<Guid> CreateCategoryIdAsync()
        => (await FileCategoryTestDataBuilder.InsertAsync(
            GetRequiredService<FileCategoryManager>(),
            GetRequiredService<IFileCategoryRepository>())).Id;
}
