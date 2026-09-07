using FluentValidation;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.CoreExtensions.Querying;
using Byd.FileManagement.Dtos.StoredFiles;

namespace Byd.FileManagement.Validators.StoredFiles;

/// <summary>
/// Saklanan dosya liste isteginin sayfalama sinirlarini dogrular.
/// </summary>
// islevi: Sayfa numarasi ile sayfa boyutunu desteklenen DataTable araliginda tutar.
// sistemdeki gorevi: Dinamik sorgu repository'sine yalnizca sinirlari belli liste isteklerinin girmesini saglar.
public class StoredFileListInputValidator : AbstractValidator<StoredFileListInput>
{
    // Saklanan dosya liste isteginin sayfalama kurallarini tanimlar.
    public StoredFileListInputValidator()
    {
        RuleFor(input => input.Page)
            .GreaterThanOrEqualTo(DataTablePaging.FirstPage)
            .WithErrorCode(GeneralExceptionCodes.InvalidPage);
        RuleFor(input => input.PageSize)
            .InclusiveBetween(1, DataTablePaging.MaxPageSize)
            .WithErrorCode(GeneralExceptionCodes.InvalidPageSize);
    }
}
