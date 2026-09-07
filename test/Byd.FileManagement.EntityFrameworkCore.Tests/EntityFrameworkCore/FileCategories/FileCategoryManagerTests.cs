using System.Threading;
using System.Threading.Tasks;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Models.FileCategories;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.FileCategories;

/// <summary>
/// FileCategoryManager is kurallarini dogrular.
/// </summary>
// islevi: Ad normalizasyonunun, zorunlu ad guard'inin ve ad benzersizliginin Manager tarafindan
// uygulandigini kanitlar.
// sistemdeki gorevi: FileCategory data-shell mutasyonlarinin domain sahibinde kaldigini koruyan testtir.
public class FileCategoryManagerTests : FileManagementEntityFrameworkCoreTestBase
{
    // Create ve update islemlerinin kategori adini trim ettigini dogrular.
    [Fact]
    public async Task Should_normalize_create_and_update_names()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<FileCategoryManager>();
            var entity = await manager.CreateAsync(
                new FileCategoryCreateModel { Name = "  Sozlesme  ", Code = 10 },
                CancellationToken.None);

            entity.Name.ShouldBe("Sozlesme");
            await manager.UpdateAsync(
                entity,
                new FileCategoryUpdateModel { Name = "  Rapor  ", Code = 20 },
                CancellationToken.None);
            entity.Name.ShouldBe("Rapor");
            entity.Code.ShouldBe(20);
        });
    }

    // Bos kategori adinin create ve update akislari tarafindan reddedildigini dogrular.
    [Fact]
    public async Task Should_reject_blank_names()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<FileCategoryManager>();
            var createException = await Should.ThrowAsync<BusinessException>(async () =>
                await manager.CreateAsync(
                    new FileCategoryCreateModel { Name = "   " },
                    CancellationToken.None));
            createException.Code.ShouldBe(GeneralExceptionCodes.RequiredValueMissing);

            var entity = await manager.CreateAsync(
                new FileCategoryCreateModel { Name = "Gecerli" },
                CancellationToken.None);
            var updateException = await Should.ThrowAsync<BusinessException>(async () =>
                await manager.UpdateAsync(
                    entity,
                    new FileCategoryUpdateModel { Name = " " },
                    CancellationToken.None));
            updateException.Code.ShouldBe(GeneralExceptionCodes.RequiredValueMissing);
        });
    }

    // Ayni adla ikinci kategorinin acilmasinin is hatasiyla reddedildigini dogrular.
    // Ad tuketicinin adresleme anahtaridir; kacan bir kopya dosyalari iki kategoriye bolerdi.
    [Fact]
    public async Task Should_reject_duplicate_name()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var manager = GetRequiredService<FileCategoryManager>();
            var repository = GetRequiredService<IFileCategoryRepository>();
            var existing = await manager.CreateAsync(
                new FileCategoryCreateModel { Name = "Is dosyasi" },
                CancellationToken.None);
            await repository.InsertAsync(existing, autoSave: true, cancellationToken: CancellationToken.None);

            var exception = await Should.ThrowAsync<BusinessException>(async () =>
                await manager.CreateAsync(
                    new FileCategoryCreateModel { Name = "  Is dosyasi  " },
                    CancellationToken.None));

            exception.Code.ShouldBe(GeneralExceptionCodes.AlreadyExists);
        });
    }
}
