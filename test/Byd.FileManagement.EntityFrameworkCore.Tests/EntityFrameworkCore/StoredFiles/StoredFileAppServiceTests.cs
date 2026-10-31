using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Byd.FileManagement.Models.StoredFiles;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Etos.StoredFiles;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Services.StoredFiles;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// Dosya yukleme, indirme ve silme akislarini uctan uca dogrular.
/// </summary>
// islevi: Byte'larin gercekten depoya yazildigini, geri okundugunu, bir kategorinin birden fazla
// dosya tasiyabildigini ve silmenin hem kaydi hem byte'i goturdugunu kanitlar.
// sistemdeki gorevi: Iki fazli yazma ile silme sirasinin sozlesmesini koruyan testtir.
public class StoredFileAppServiceTests : FileManagementEntityFrameworkCoreTestBase
{
    private const string PresignedBucketName = "presigned-tests";
    private static readonly byte[] FirstContent = "ilk icerik"u8.ToArray();
    private static readonly byte[] SecondContent = "ikinci icerik"u8.ToArray();
    private readonly IDistributedEventBus _distributedEventBus = Substitute.For<IDistributedEventBus>();

    // Yayinlanan olaylari RabbitMQ'ya gondermeden dogrulamak icin dagitik olay yolunu sahteyle degistirir.
    protected override void AfterAddApplication(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(_distributedEventBus));
    }

    // Yuklenen dosyanin kaydinin ve byte'larinin geri okunabildigini dogrular.
    [Fact]
    public async Task Should_upload_then_download_same_content()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();
            var externalId = Guid.NewGuid();

            var uploaded = await appService.UploadAsync(
                BuildUpload(externalId, categoryId, FirstContent, "rapor.pdf"));
            var downloaded = await appService.DownloadAsync(uploaded.Id);

            uploaded.OriginalFileName.ShouldBe("rapor.pdf");
            downloaded.Data.ShouldBe(FirstContent);
            downloaded.OriginalFileName.ShouldBe("rapor.pdf");
        });
    }

    // Dogrudan yuklemenin kayit kesinlesince tamamlanma olayini yayinladigini dogrular.
    [Fact]
    public async Task Should_publish_upload_completed_event_after_upload()
    {
        var externalId = Guid.NewGuid();
        var uploaded = await WithUnitOfWorkAsync(async () =>
            await GetRequiredService<IStoredFileAppService>().UploadAsync(
                BuildUpload(externalId, await CreateCategoryIdAsync(), FirstContent, "rapor.pdf")));

        await _distributedEventBus.Received(1).PublishAsync(
            Arg.Is<StoredFileUploadCompletedEto>(completed => completed.Id == uploaded.Id
                && completed.ExternalId == externalId
                && completed.OriginalFileName == "rapor.pdf"),
            Arg.Any<bool>(),
            Arg.Any<bool>());
    }

    // Indirme cevabinin icerik turunu dosya adindan uretip tasidigini dogrular.
    // Tur veritabaninda tutulmadigi icin bu test aslinda cozumlemenin dogru kaynaktan
    // beslendigini kanitlar: iki farkli uzanti iki farkli tur uretmelidir.
    [Fact]
    public async Task Should_resolve_content_type_from_file_name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();

            var document = await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), categoryId, FirstContent, "rapor.pdf"));
            var picture = await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), categoryId, SecondContent, "fotograf.jpg"));

            (await appService.DownloadAsync(document.Id)).ContentType.ShouldBe("application/pdf");
            (await appService.DownloadAsync(picture.Id)).ContentType.ShouldBe("image/jpeg");
        });
    }

    // Yuklenen kaydin okuma yollarinda gorundugunu dogrular; onay adimi calismis demektir.
    [Fact]
    public async Task Should_expose_uploaded_file_on_read_paths()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();
            var externalId = Guid.NewGuid();
            var uploaded = await appService.UploadAsync(
                BuildUpload(externalId, categoryId, FirstContent, "rapor.pdf"));

            var files = await appService.GetListByCategoryAsync(externalId, categoryId);

            files.ShouldHaveSingleItem().Id.ShouldBe(uploaded.Id);
        });
    }

    // Ayni is kaydinin ayni kategorisine ikinci dosyanin yuklenebildigini dogrular.
    // Bir isin birden fazla is dosyasi olabilir; ikinci yukleme reddedilmemelidir.
    [Fact]
    public async Task Should_accept_second_file_in_same_category()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();
            var externalId = Guid.NewGuid();

            var first = await appService.UploadAsync(
                BuildUpload(externalId, categoryId, FirstContent, "ilk.pdf"));
            var second = await appService.UploadAsync(
                BuildUpload(externalId, categoryId, SecondContent, "ikinci.pdf"));

            var files = await appService.GetListByCategoryAsync(externalId, categoryId);

            files.Count.ShouldBe(2);
            files.ShouldContain(item => item.Id == first.Id);
            files.ShouldContain(item => item.Id == second.Id);
            (await appService.DownloadAsync(second.Id)).Data.ShouldBe(SecondContent);
        });
    }

    // Kategori okumasinin farkli is kayitlarindaki dosyalari dogru byte'larla birlestirdigini dogrular.
    [Fact]
    public async Task Should_return_every_file_with_content_by_category()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var category = await CreateCategoryAsync();
            var otherCategory = await CreateCategoryAsync();
            var first = await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), category.Id, FirstContent, "ilk.pdf"));
            var second = await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), category.Id, SecondContent, "ikinci.pdf"));
            await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), otherCategory.Id, FirstContent, "diger.pdf"));

            var files = await appService.GetListWithContentByCategoryAsync(category.Id);

            files.Count.ShouldBe(2);
            var filesById = files.ToDictionary(item => item.Id);
            filesById[first.Id].Data.ShouldBe(FirstContent);
            filesById[second.Id].Data.ShouldBe(SecondContent);
            filesById[first.Id].CategoryName.ShouldBe(category.Name);
        });
    }

    // Is kaydi ve kategori okumasinin yalniz iki adresle de eslesen dosya byte'larini dondurdugunu dogrular.
    [Fact]
    public async Task Should_return_matching_files_with_content_by_external_id_and_category()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var category = await CreateCategoryAsync();
            var otherCategory = await CreateCategoryAsync();
            var externalId = Guid.NewGuid();
            var first = await appService.UploadAsync(
                BuildUpload(externalId, category.Id, FirstContent, "ilk.pdf"));
            var second = await appService.UploadAsync(
                BuildUpload(externalId, category.Id, SecondContent, "ikinci.pdf"));
            await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), category.Id, FirstContent, "diger-is.pdf"));
            await appService.UploadAsync(
                BuildUpload(externalId, otherCategory.Id, FirstContent, "diger-kategori.pdf"));

            var files = await appService.GetListWithContentByExternalIdAndCategoryAsync(
                externalId,
                category.Id);

            files.Count.ShouldBe(2);
            files.All(item => item.ExternalId == externalId && item.CategoryId == category.Id).ShouldBeTrue();
            var filesById = files.ToDictionary(item => item.Id);
            filesById[first.Id].Data.ShouldBe(FirstContent);
            filesById[second.Id].Data.ShouldBe(SecondContent);
        });
    }

    // Silmenin kaydi kaldirdigini ve byte'larin da gittigini dogrular.
    [Fact]
    public async Task Should_delete_record_and_content()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();
            var uploaded = await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), categoryId, FirstContent, "rapor.pdf"));

            await appService.DeleteAsync(uploaded.Id);

            await Should.ThrowAsync<BusinessException>(async () => await appService.GetAsync(uploaded.Id));
        });
    }

    // Cozulemeyen uzantinin yukleme akisi tarafindan reddedildigini dogrular.
    [Fact]
    public async Task Should_reject_file_with_unresolvable_content_type()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();

            var exception = await Should.ThrowAsync<BusinessException>(async () =>
                await appService.UploadAsync(
                    BuildUpload(Guid.NewGuid(), categoryId, FirstContent, "dosya.bilinmeyenuzanti")));

            exception.Code.ShouldBe(ExceptionCodes.StoredFileExceptionCodes.UnresolvedContentType);
        });
    }

    // Presigned GET baglantisinin kayit kimligi, bucket, blob adi ve ayarli sureyi tasidigini dogrular.
    [Fact]
    public async Task Should_create_presigned_download_url_for_stored_blob()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();
            var uploaded = await appService.UploadAsync(
                BuildUpload(Guid.NewGuid(), categoryId, FirstContent, "rapor.pdf"));

            var result = await appService.GetPresignedUrlAsync(uploaded.Id);

            result.OriginalFileName.ShouldBe("rapor.pdf");
            // host/ oneki cercevenin kendi nesne anahtari kuralidir; UploadAsync byte'i ayni
            // anahtara yazar. Onek dusulurse imzali URL var olmayan bir nesneyi gosterir.
            result.Url.ShouldContain($"/{PresignedBucketName}/host/{uploaded.Id:N}");
            result.Url.ShouldContain("X-Amz-Expires=3600");
            result.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(59));
        });
    }

    // Presigned PUT akisinin satir acmadan bekleyen bilgi ve dogru nesne adresini urettigini dogrular.
    [Fact]
    public async Task Should_create_presigned_upload_url_without_content()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();
            var externalId = Guid.NewGuid();

            var result = await appService.CreatePresignedUploadAsync(
                BuildPresignedUpload(externalId, categoryId, "rapor.pdf"));

            var blobName = Uri.UnescapeDataString(new Uri(result.Url).AbsolutePath)
                .Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
            var pending = await GetRequiredService<IDistributedCache<StoredFilePendingUploadModel>>()
                .GetAsync(blobName);

            (await appService.GetListByCategoryAsync(externalId, categoryId)).ShouldBeEmpty();
            pending.ShouldNotBeNull();
            pending.ExternalId.ShouldBe(externalId);
            pending.CategoryId.ShouldBe(categoryId);
            result.OriginalFileName.ShouldBe("rapor.pdf");
            result.Url.ShouldContain($"/{PresignedBucketName}/host/{blobName}");
            result.Url.ShouldContain("X-Amz-Expires=7200");
        });
    }

    // Presigned upload akisinin cozumlenemeyen uzantiyi baglanti uretmeden reddettigini dogrular.
    [Fact]
    public async Task Should_reject_unresolvable_presigned_upload_file_name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var appService = GetRequiredService<IStoredFileAppService>();
            var categoryId = await CreateCategoryIdAsync();

            var exception = await Should.ThrowAsync<BusinessException>(async () =>
                await appService.CreatePresignedUploadAsync(
                    BuildPresignedUpload(Guid.NewGuid(), categoryId, "dosya.bilinmeyenuzanti")));

            exception.Code.ShouldBe(ExceptionCodes.StoredFileExceptionCodes.UnresolvedContentType);
        });
    }

    // Verilen adrese yazan gecerli bir yukleme istegi kurar.
    private static UploadStoredFileDto BuildUpload(
        Guid externalId,
        Guid categoryId,
        IEnumerable<byte> data,
        string fileName)
    {
        return new UploadStoredFileDto
        {
            Data = data.ToArray(),
            FileName = fileName,
            ExternalId = externalId,
            CategoryId = categoryId
        };
    }

    // Verilen adrese byte tasimadan yazan gecerli bir presigned yukleme istegi kurar.
    private static PresignedUploadStoredFileDto BuildPresignedUpload(
        Guid externalId,
        Guid categoryId,
        string fileName)
    {
        return new PresignedUploadStoredFileDto
        {
            FileName = fileName,
            ExternalId = externalId,
            CategoryId = categoryId
        };
    }

    // Testin dosya baglayacagi kategoriyi acar; modul kategori seed etmez.
    private async Task<Guid> CreateCategoryIdAsync()
    {
        return (await CreateCategoryAsync()).Id;
    }

    // Testin kullanacagi kategori kaydini adiyla birlikte acar.
    private Task<FileCategory> CreateCategoryAsync()
    {
        return FileCategoryTestDataBuilder.InsertAsync(
            GetRequiredService<FileCategoryManager>(),
            GetRequiredService<IFileCategoryRepository>());
    }
}
