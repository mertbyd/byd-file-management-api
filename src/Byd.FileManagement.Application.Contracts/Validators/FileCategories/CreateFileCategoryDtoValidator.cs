using Byd.FileManagement.Dtos.FileCategories;

namespace Byd.FileManagement.Validators.FileCategories;

/// <summary>
/// FileCategory create istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: CreateFileCategoryDto icin FluentValidation kurallarini etkinlestirir.
// sistemdeki gorevi: Create endpointinin public girdi validator'idir.
public sealed class CreateFileCategoryDtoValidator
    : FileCategoryInputDtoValidatorBase<CreateFileCategoryDto>
{
}
