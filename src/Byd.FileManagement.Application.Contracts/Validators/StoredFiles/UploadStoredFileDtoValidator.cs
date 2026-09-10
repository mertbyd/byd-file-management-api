using FluentValidation;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.ExceptionCodes;

namespace Byd.FileManagement.Validators.StoredFiles;

/// <summary>
/// Dosya yukleme isteginin sekil kurallarini dogrular.
/// </summary>
// islevi: Bos icerigi, bos dosya adini ve eksik kategori adresini depoya gitmeden reddeder.
// sistemdeki gorevi: Yukleme ucunun public girdi validator'idir.
// mirastan gelen: StoredFileUploadInputDtoValidatorBase -> ExternalId, CategoryId ve FileName kurallari.
// burada yazilan: byte icerigin zorunluluk kurali.
public sealed class UploadStoredFileDtoValidator
    : StoredFileUploadInputDtoValidatorBase<UploadStoredFileDto>
{
    // Yukleme isteginin zorunluluk kurallarini tanimlar.
    public UploadStoredFileDtoValidator()
    {
        RuleFor(input => input.Data)
            .NotEmpty().WithErrorCode(StoredFileExceptionCodes.RequiredValueMissing);
    }
}
