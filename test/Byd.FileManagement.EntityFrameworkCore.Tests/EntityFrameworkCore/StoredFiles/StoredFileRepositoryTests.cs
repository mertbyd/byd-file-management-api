using System;
using System.Threading;
using System.Threading.Tasks;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.EntityFrameworkCore.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Interfaces.StoredFiles;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Managers.StoredFiles;
using Byd.FileManagement.Mappers.StoredFiles;
using Shouldly;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// StoredFile okuma sorgularini dogrular.
/// </summary>
// islevi: Toplu is kaydi okumasinin tek sorguda dogru kumeyi dondurdugunu, bir kategorinin
// birden fazla dosya tasiyabildigini, onaysiz kaydin okumada gorunmedigini ve okumalarin tenant
// sinirini gecmedigini kanitlar.
// sistemdeki gorevi: (ExternalId, CategoryId) adreslemesinin cok dosyali sozlesmesini ve
// StoredFile'in IMultiTenant kapsamini koruyan testtir.
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
            files[0].Category.Name.ShouldBe(category.Name);
        });
    }

    // Bir is kaydinin ayni kategoride birden fazla dosya tasiyabildigini dogrular.
    // Kategori bir belge turudur: bir isin birden cok is dosyasi olabilir.
    [Fact]
    public async Task Should_return_every_file_of_a_category()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IStoredFileRepository>();
            var category = await CreateCategoryAsync();
            var externalId = Guid.NewGuid();
            var first = await InsertFileAsync(externalId, category.Id);
            var second = await InsertFileAsync(externalId, category.Id);

            var files = await repository.GetListByCategoryAsync(
                externalId,
                category.Id,
                CancellationToken.None);

            files.Count.ShouldBe(2);
            files.ShouldContain(item => item.Id == first.Id);
            files.ShouldContain(item => item.Id == second.Id);
            files[0].Category.Name.ShouldBe(category.Name);
            new StoredFileMapper().Map(files[0]).CategoryName.ShouldBe(category.Name);
        });
    }

    // Kategori kimligi sorgusunun farkli is kayitlarindaki dosyalari getirip diger kategoriyi elemesini dogrular.
    [Fact]
    public async Task Should_return_only_files_of_requested_category()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IStoredFileRepository>();
            var manager = GetRequiredService<StoredFileManager>();
            var category = await CreateCategoryAsync();
            var otherCategory = await CreateCategoryAsync();
            var first = await InsertFileAsync(Guid.NewGuid(), category.Id);
            var second = await InsertFileAsync(Guid.NewGuid(), category.Id);
            var other = await InsertFileAsync(Guid.NewGuid(), otherCategory.Id);

            var files = await repository.GetListByCategoryIdAsync(category.Id, CancellationToken.None);

            files.Count.ShouldBe(2);
            files.ShouldContain(item => item.Id == first.Id);
            files.ShouldContain(item => item.Id == second.Id);
            files.ShouldNotContain(item => item.Id == other.Id);
        });
    }

    // Dosyasi olmayan kategori adresinin bos liste dondurdugunu dogrular.
    [Fact]
    public async Task Should_return_empty_list_for_category_without_files()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IStoredFileRepository>();
            var category = await CreateCategoryAsync();

            var files = await repository.GetListByCategoryAsync(
                Guid.NewGuid(),
                category.Id,
                CancellationToken.None);

            files.ShouldBeEmpty();
        });
    }

    // Bir tenant'in dosyasinin baska bir tenant'in okumasinda gorunmedigini dogrular.
    // StoredFile IMultiTenant oldugu icin kosulu ABP global filtresi ekler; bu test o filtrenin
    // gercekten kuruldugunu kanitlar, cunku adresi bilen herkes aksi halde dosyayi okurdu.
    [Fact]
    public async Task Should_not_return_file_of_another_tenant()
    {
        var currentTenant = GetRequiredService<ICurrentTenant>();
        var owner = Guid.NewGuid();
        var stranger = Guid.NewGuid();
        var externalId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var repository = GetRequiredService<IStoredFileRepository>();
            var category = await CreateCategoryAsync();
            using (currentTenant.Change(owner))
            {
                await InsertFileAsync(externalId, category.Id);
            }

            using (currentTenant.Change(stranger))
            {
                var files = await repository.GetListByCategoryAsync(
                    externalId,
                    category.Id,
                    CancellationToken.None);
                var categoryFiles = await repository.GetListByCategoryIdAsync(
                    category.Id,
                    CancellationToken.None);

                files.ShouldBeEmpty();
                categoryFiles.ShouldBeEmpty();
            }

            using (currentTenant.Change(owner))
            {
                var files = await repository.GetListByCategoryAsync(
                    externalId,
                    category.Id,
                    CancellationToken.None);
                var categoryFiles = await repository.GetListByCategoryIdAsync(
                    category.Id,
                    CancellationToken.None);

                files.Count.ShouldBe(1);
                categoryFiles.Count.ShouldBe(1);
            }
        });
    }


    // Verilen adrese kalici bir dosya kaydi yazar.
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
