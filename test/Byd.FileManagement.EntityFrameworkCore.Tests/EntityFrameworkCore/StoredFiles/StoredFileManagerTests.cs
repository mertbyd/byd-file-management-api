using System;
using System.Threading;
using System.Threading.Tasks;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.FileManagement.Consts;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Managers.StoredFiles;
using Byd.FileManagement.Models.StoredFiles;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// StoredFileManager is kurallarini dogrular.
/// </summary>
// islevi: Yazma on kosulunu, dosya adi normalizasyonunu, modul adinin host beyanindan
// gelmesini, content type cozumlemesini ve blob adi uretimini kanitlar.
// sistemdeki gorevi: StoredFile kararlarinin domain sahibinde kaldigini koruyan testtir.
public class StoredFileManagerTests : FileManagementEntityFrameworkCoreTestBase
{
    // Create isleminin dosya adini trim ettigini dogrular.
    [Fact]
    public async Task Should_normalize_file_name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<StoredFileManager>();
            var model = await BuildModelAsync();
            model.FileName = "  belge.pdf  ";

            var entity = await manager.CreateAsync(model, CancellationToken.None);

            entity.OriginalFileName.ShouldBe("belge.pdf");
        });
    }

    // Modul adinin cagrandan degil host beyanindan yazildigini dogrular.
    [Fact]
    public async Task Should_write_module_name_from_host_declaration()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<StoredFileManager>();

            var entity = await manager.CreateAsync(
                await BuildModelAsync(),
                CancellationToken.None);

            entity.ModuleName.ShouldBe("FileManagement");
        });
    }

    // Blob adinin uretildigini ve orijinal dosya adini tasimadigini dogrular.
    [Fact]
    public async Task Should_generate_opaque_blob_name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<StoredFileManager>();

            var entity = await manager.CreateAsync(
                await BuildModelAsync(),
                CancellationToken.None);

            entity.BlobName.ShouldBe(entity.Id.ToString("N"));
            entity.BlobName.ShouldNotContain("belge");
        });
    }

    // Var olmayan kategori slotuna yazmanin reddedildigini dogrular.
    [Fact]
    public async Task Should_reject_unknown_category()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<StoredFileManager>();
            var model = StoredFileTestDataBuilder.Build(Guid.NewGuid(), Guid.NewGuid());

            var exception = await Should.ThrowAsync<BusinessException>(
                async () => await manager.CreateAsync(model, CancellationToken.None));

            exception.Code.ShouldBe(GeneralExceptionCodes.EntityNotFound);
        });
    }

    // Bos dosya adinin create akisi tarafindan reddedildigini dogrular.
    [Fact]
    public async Task Should_reject_blank_file_name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<StoredFileManager>();
            var model = await BuildModelAsync();
            model.FileName = "   ";

            var exception = await Should.ThrowAsync<BusinessException>(
                async () => await manager.CreateAsync(model, CancellationToken.None));

            exception.Code.ShouldBe(GeneralExceptionCodes.RequiredValueMissing);
        });
    }

    // Icerik tipi cozulemeyen dosya adinin yazma aninda reddedildigini dogrular.
    // Tip kayitta tutulmadigi icin bu kontrol kacarsa dosya sonradan dogru tiple sunulamaz.
    [Fact]
    public async Task Should_reject_file_name_with_unresolvable_content_type()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<StoredFileManager>();
            var model = await BuildModelAsync();
            model.FileName = "dosya.bilinmeyenuzanti";

            var exception = await Should.ThrowAsync<BusinessException>(
                async () => await manager.CreateAsync(model, CancellationToken.None));

            exception.Code.ShouldBe(ExceptionCodes.StoredFileExceptionCodes.UnresolvedContentType);
        });
    }

    // Once kendi kategorisini acar, sonra o kategoriye yazan gecerli bir create modeli kurar.
    private async Task<StoredFileCreateModel> BuildModelAsync()
    {
        var category = await FileCategoryTestDataBuilder.InsertAsync(
            GetRequiredService<FileCategoryManager>(),
            GetRequiredService<IFileCategoryRepository>());

        return StoredFileTestDataBuilder.Build(Guid.NewGuid(), category.Id);
    }
}
