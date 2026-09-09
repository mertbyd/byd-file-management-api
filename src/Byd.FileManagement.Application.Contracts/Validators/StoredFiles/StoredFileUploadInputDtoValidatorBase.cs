using System;
using FluentValidation;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.ExceptionCodes;

namespace Byd.FileManagement.Validators.StoredFiles;

/// <summary>
/// Byte tasiyan ve presigned yukleme isteklerinin ortak sekil kurallarini tanimlar.
/// </summary>
// islevi: Is kaydi, kategori ve dosya adi zorunluluklarini iki yukleme yolu icin tek kez tanimlar.
// sistemdeki gorevi: Concrete yukleme validator'larinin bounded hata kodu kullanan ortak tabanidir.
public abstract class StoredFileUploadInputDtoValidatorBase<TDto> : AbstractValidator<TDto>
    where TDto : StoredFileUploadInputDtoBase
{
    protected StoredFileUploadInputDtoValidatorBase()
    {
        RuleFor(input => input.ExternalId)
            .NotEqual(Guid.Empty).WithErrorCode(StoredFileExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.CategoryId)
            .NotEqual(Guid.Empty).WithErrorCode(StoredFileExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.FileName)
            .NotEmpty().WithErrorCode(StoredFileExceptionCodes.RequiredValueMissing)
            .MaximumLength(StoredFileConsts.MaxOriginalFileNameLength)
            .WithErrorCode(StoredFileExceptionCodes.FileNameTooLong);
    }
}
