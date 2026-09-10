namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Dosya yukleme istegini tanimlar.
/// </summary>
// islevi: Byte'lari, ozgun dosya adini ve kategori adresini yukleme akisina tasir.
// sistemdeki gorevi: Public yukleme sozlesmesidir. Ham byte alir, IFormFile almaz; modul
// ASP.NET'e bagimli degildir. Kapsam ve depolama alanlari cagirandan alinmaz, sunucu doldurur.
// mirastan gelen: StoredFileUploadInputDtoBase -> ExternalId, CategoryId, FileName.
// burada yazilan: yalnizca icerik.
public sealed class UploadStoredFileDto : StoredFileUploadInputDtoBase
{
    public byte[] Data { get; set; } = default!;
}
