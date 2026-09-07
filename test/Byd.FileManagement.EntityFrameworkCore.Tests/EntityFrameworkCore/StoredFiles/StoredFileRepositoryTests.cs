using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Managers.StoredFiles;
using Shouldly;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// StoredFile slot adresleme davranislarini dogrular.
/// </summary>
// islevi: Toplu is kaydi okumasinin tek sorguda dogru kumeyi dondurdugunu, slot adresinin tek dosya
// verdigini ve ayni slota ikinci kaydin veritabani tarafindan reddedildigini kanitlar.
// sistemdeki gorevi: (ExternalId, CategoryId) adresleme sozlesmesini koruyan testtir.
public class StoredFileRepositoryTests : FileManagementEntityFrameworkCoreTestBase
{
    // Toplu okumanin yalniz istenen is kayitlarinin dosyalarini dondurdugunu dogrular.
    [Fact]
    public async Task Should_return_files_of_requested_external_ids()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IStoredFileRepository>();
            var category = await CreateCategoryAsync();
            var wantedExternalId = Guid.NewGuid();
            var otherExternalId = Guid.NewGuid();
            var wanted = await InsertFileAsync(wantedExternalId, category.Id);
            var other = await InsertFileAsync(otherExternalId, category.Id);

            var files = await repository.GetListByExternalIdsAsync(
                [wantedExternalId],
                CancellationToken.None);

            files.Count.ShouldBe(1);
            files.ShouldContain(item => item.Id == wanted.Id);
            files.ShouldNotContain(item => item.Id == other.Id);
        });
    }

    // Slot adresinin tek dosya dondurdugunu, bos slotta null verdigini dogrular.
    [Fact]
    public async Task Should_return_single_file_for_slot_address()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IStoredFileRepository>();
            var category = await CreateCategoryAsync();
            var externalId = Guid.NewGuid();
            var file = await InsertFileAsync(externalId, category.Id);

            var found = await repository.GetBySlotAsync(externalId, category.Id, CancellationToken.None);
            var empty = await repository.GetBySlotAsync(Guid.NewGuid(), category.Id, CancellationToken.None);

            found.ShouldNotBeNull();
            found.Id.ShouldBe(file.Id);
            empty.ShouldBeNull();
        });
    }

    // Ayni slota ikinci kaydin veritabani tarafindan reddedildigini dogrular.
    [Fact]
    public async Task Should_reject_second_file_in_same_slot()
    {
        var externalId = Guid.NewGuid();

        // Hata unit of work'un disina cikarilir; iceride yakalansaydi basarisiz insert
        // takip edilmeye devam eder ve UOW kapanirken ikinci kez patlardi.
        await Should.ThrowAsync<DbUpdateException>(async () =>
            await WithUnitOfWorkAsync(async () =>
            {
                var category = await CreateCategoryAsync();
                await InsertFileAsync(externalId, category.Id);
                await InsertFileAsync(externalId, category.Id);
            }));
    }

    // Verilen slot adresine kalici bir dosya kaydi yazar.
    private async Task<StoredFile> InsertFileAsync(Guid externalId, Guid categoryId)
    {
        var manager = GetRequiredService<StoredFileManager>();
        var repository = GetRequiredService<IStoredFileRepository>();
        var file = await manager.CreateAsync(
            StoredFileTestDataBuilder.Build(externalId, categoryId),
            CancellationToken.None);
        await repository.InsertAsync(file, autoSave: true, cancellationToken: CancellationToken.None);

        return file;
    }

    // Dosyalarin baglanabilecegi kalici bir kategori kurar; modul kategori seed etmez.
    private Task<FileCategory> CreateCategoryAsync()
        => FileCategoryTestDataBuilder.InsertAsync(
            GetRequiredService<FileCategoryManager>(),
            GetRequiredService<IFileCategoryRepository>());
}
