using System;
using FluentValidation;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.FileManagement.Dtos.StoredFiles;

namespace Byd.FileManagement.Validators.StoredFiles;

/// <summary>
/// Dosya yukleme isteginin sekil kurallarini dogrular.
/// </summary>
// islevi: Bos icerigi, bos dosya adini ve eksik kategori adresini depoya gitmeden reddeder.
// sistemdeki gorevi: Yukleme ucunun public girdi validator'idir.
public class UploadStoredFileDtoValidator : AbstractValidator<UploadStoredFileDto>
{
    // Yukleme isteginin zorunluluk kurallarini tanimlar.
    public UploadStoredFileDtoValidator()
    {
        RuleFor(input => input.Data)
            .NotEmpty().WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.FileName)
            .NotEmpty().WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.ExternalId)
            .NotEqual(Guid.Empty).WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.CategoryId)
            .NotEqual(Guid.Empty).WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
    }
}
