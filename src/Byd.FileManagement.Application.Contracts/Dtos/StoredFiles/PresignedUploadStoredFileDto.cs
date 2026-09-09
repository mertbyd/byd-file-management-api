namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Byte tasimayan presigned dosya yukleme istegini tanimlar.
/// </summary>
// islevi: Metadata kaydini acmak ve PUT baglantisi uretmek icin ortak yukleme alanlarini tasir.
// sistemdeki gorevi: Kapsam ve depolama alanlarini sunucuda birakan public presigned upload sozlesmesidir.
// mirastan gelen: StoredFileUploadInputDtoBase -> ExternalId, CategoryId, FileName.
// burada yazilan: yok.
public sealed class PresignedUploadStoredFileDto : StoredFileUploadInputDtoBase
{
}
