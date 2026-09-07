using System.Linq;
using Byd.FileManagement.Dtos.FileCategories;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Validators.FileCategories;
using Shouldly;
using Xunit;

namespace Byd.FileManagement.Application.FileCategories;

/// <summary>
/// FileCategory public input validator sozlesmesini dogrular.
/// </summary>
// islevi: Create ve update girdilerinin kararli zorunluluk ve uzunluk hata kodlarini urettigini kanitlar.
// sistemdeki gorevi: Controller girdilerinin repository'ye ulasmadan sekil dogrulamasindan gecmesini koruyan testtir.
public class FileCategoryValidatorTests
    : FileManagementApplicationTestBase<FileManagementApplicationTestModule>
{
    // Create girdisindeki bos adin zorunluluk hata koduyla reddedildigini dogrular.
    [Fact]
    public void Should_reject_empty_create_name()
    {
        var result = new CreateFileCategoryDtoValidator().Validate(
            new CreateFileCategoryDto { Name = string.Empty });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorCode == FileCategoryExceptionCodes.NameRequired);
    }

    // Update girdisindeki uzun adin uzunluk hata koduyla reddedildigini dogrular.
    [Fact]
    public void Should_reject_overlong_update_name()
    {
        var result = new UpdateFileCategoryDtoValidator().Validate(
            new UpdateFileCategoryDto { Name = string.Concat(Enumerable.Repeat("a", 129)) });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorCode == FileCategoryExceptionCodes.NameTooLong);
    }
}
