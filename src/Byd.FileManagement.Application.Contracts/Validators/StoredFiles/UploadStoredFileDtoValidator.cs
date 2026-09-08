using System;
using FluentValidation;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.FileManagement.Consts;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Dtos.StoredFiles;

namespace Byd.FileManagement.Validators.StoredFiles;

/// <summary>
/// Dosya yukleme isteginin sekil kurallarini dogrular.
/// </summary>
// islevi: Bos icerigi, bos dosya adini ve eksik kategori adresini depoya gitmeden reddeder.
// sistemdeki gorevi: Yukleme ucunun public girdi validator'idir.
public class UploadStoredFileDtoValidator : AbstractValidator<UploadStoredFileDto>
{
    // Yukleme isteginin zorunluluk ve uzunluk kurallarini tanimlar.
    public UploadStoredFileDtoValidator()
    {
        // Cascade.Stop: veri bos oldugunda boyut kurali calismaz, aksi halde bos diziyi olcmeye kalkar.
        RuleFor(input => input.Data)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(GeneralExceptionCodes.RequiredValueMissing)
            .Must(data => data.Length <= StoredFileConsts.MaxDataLength)
            .WithErrorCode(StoredFileExceptionCodes.DataTooLarge);
        RuleFor(input => input.FileName)
            .NotEmpty().WithErrorCode(GeneralExceptionCodes.RequiredValueMissing)
            .MaximumLength(StoredFileConsts.MaxOriginalFileNameLength)
            .WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.ExternalId)
            .NotEqual(Guid.Empty).WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
        RuleFor(input => input.CategoryId)
            .NotEqual(Guid.Empty).WithErrorCode(GeneralExceptionCodes.RequiredValueMissing);
    }
}
