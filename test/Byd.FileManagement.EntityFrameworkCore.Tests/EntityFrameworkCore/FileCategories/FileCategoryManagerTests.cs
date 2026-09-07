using Byd.CoreExtensions.ExceptionCodes;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Models.FileCategories;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Byd.FileManagement.EntityFrameworkCore.FileCategories;

/// <summary>
/// FileCategoryManager is kurallarini dogrular.
/// </summary>
// islevi: Ad normalizasyonu ile zorunlu ad guard'inin Manager tarafindan uygulandigini kanitlar.
// sistemdeki gorevi: FileCategory data-shell mutasyonlarinin domain sahibinde kaldigini koruyan testtir.
public class FileCategoryManagerTests : FileManagementEntityFrameworkCoreTestBase
{
    // Create ve update islemlerinin kategori adini trim ettigini dogrular.
    [Fact]
    public void Should_normalize_create_and_update_names()
    {
        var manager = GetRequiredService<FileCategoryManager>();
        var entity = manager.Create(new FileCategoryCreateModel
        {
            Name = "  Sözleşme  ",
            Code = 10
        });

        entity.Name.ShouldBe("Sözleşme");
        manager.Update(entity, new FileCategoryUpdateModel
        {
            Name = "  Rapor  ",
            Code = 20
        });
        entity.Name.ShouldBe("Rapor");
        entity.Code.ShouldBe(20);
    }

    // Bos kategori adinin create ve update akislari tarafindan reddedildigini dogrular.
    [Fact]
    public void Should_reject_blank_names()
    {
        var manager = GetRequiredService<FileCategoryManager>();
        var createException = Should.Throw<BusinessException>(() =>
            manager.Create(new FileCategoryCreateModel { Name = "   " }));
        createException.Code.ShouldBe(GeneralExceptionCodes.RequiredValueMissing);

        var entity = manager.Create(new FileCategoryCreateModel { Name = "Geçerli" });
        var updateException = Should.Throw<BusinessException>(() =>
            manager.Update(entity, new FileCategoryUpdateModel { Name = " " }));
        updateException.Code.ShouldBe(GeneralExceptionCodes.RequiredValueMissing);
    }
}
