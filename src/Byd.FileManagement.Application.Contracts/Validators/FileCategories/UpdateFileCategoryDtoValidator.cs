using Byd.FileManagement.Dtos.FileCategories;

namespace Byd.FileManagement.Validators.FileCategories;

/// <summary>
/// FileCategory update istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: UpdateFileCategoryDto icin FluentValidation kurallarini etkinlestirir.
// sistemdeki gorevi: Update endpointinin public girdi validator'idir.
public sealed class UpdateFileCategoryDtoValidator
    : FileCategoryInputDtoValidatorBase<UpdateFileCategoryDto>
{
}
