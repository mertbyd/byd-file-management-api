using FluentValidation;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.FileCategories;
using Byd.FileManagement.ExceptionCodes;

namespace Byd.FileManagement.Validators.FileCategories;

/// <summary>
/// Dosya kategorisi create ve update isteklerinin ortak sekil kurallarini tanimlar.
/// </summary>
// islevi: Kategori adi zorunluluk ve uzunluk hatalarini veritabanina gitmeden reddeder.
// sistemdeki gorevi: Create ve update validator'larinin kararli hata kodu kullanan ortak tabanidir.
public abstract class FileCategoryInputDtoValidatorBase<TDto> : AbstractValidator<TDto>
    where TDto : FileCategoryInputDto
{
    protected FileCategoryInputDtoValidatorBase()
    {
        RuleFor(input => input.Name)
            .NotEmpty().WithErrorCode(FileCategoryExceptionCodes.NameRequired)
            .MaximumLength(FileCategoryConsts.MaxNameLength)
            .WithErrorCode(FileCategoryExceptionCodes.NameTooLong);
    }
}
