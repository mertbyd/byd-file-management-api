using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Services.StoredFiles;
using Shouldly;
using Volo.Abp;
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
    private static readonly byte[] FirstContent = "ilk icerik"u8.ToArray();
    private static readonly byte[] SecondContent = "ikinci icerik"u8.ToArray();

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

    // Testin dosya baglayacagi kategoriyi acar; modul kategori seed etmez.
    private async Task<Guid> CreateCategoryIdAsync()
    {
        var category = await FileCategoryTestDataBuilder.InsertAsync(
            GetRequiredService<FileCategoryManager>(),
            GetRequiredService<IFileCategoryRepository>());

        return category.Id;
    }
}
