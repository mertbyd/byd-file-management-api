using Byd.FileManagement.Dtos.StoredFiles;

namespace Byd.FileManagement.Validators.StoredFiles;

/// <summary>
/// Presigned yukleme istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: PresignedUploadStoredFileDto icin byte boyutu kurali olmadan FluentValidation'i etkinlestirir.
// sistemdeki gorevi: Presigned upload endpointinin public girdi validator'idir.
// mirastan gelen: StoredFileUploadInputDtoValidatorBase -> ExternalId, CategoryId ve FileName kurallari.
// burada yazilan: yok.
public sealed class PresignedUploadStoredFileDtoValidator
    : StoredFileUploadInputDtoValidatorBase<PresignedUploadStoredFileDto>
{
}
