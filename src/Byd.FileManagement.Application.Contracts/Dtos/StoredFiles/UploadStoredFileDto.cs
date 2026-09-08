using System;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Dosya yukleme istegini tanimlar.
/// </summary>
// islevi: Byte'lari, ozgun dosya adini ve kategori adresini yukleme akisina tasir.
// sistemdeki gorevi: Public yukleme sozlesmesidir. Ham byte alir, IFormFile almaz; modul
// ASP.NET'e bagimli degildir. Kapsam ve depolama alanlari cagirandan alinmaz, sunucu doldurur.
public class UploadStoredFileDto
{
    public Guid ExternalId { get; set; }
    public Guid CategoryId { get; set; }
    public byte[] Data { get; set; } = default!;
    public string FileName { get; set; } = default!;
 
}
