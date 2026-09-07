using System.Collections.Generic;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Models.StoredFiles;
using Riok.Mapperly.Abstractions;

namespace Byd.FileManagement.Mappers.StoredFiles;

/// <summary>
/// Saklanan dosya entity ve DTO eslemelerini tanimlar.
/// </summary>
// islevi: Tekil ve liste repository ciktilarini Mapperly tarafindan uretilen kodla donusturur.
// sistemdeki gorevi: Dosya metadata okuma katmanlari arasindaki alan tasimanin tek sahibidir.
[Mapper]
public partial class StoredFileMapper
{
    // Tek saklanan dosya entity'sini public cevap sozlesmesine esler.
    public partial StoredFileDto Map(StoredFile source);

    // Saklanan dosya entity listesini public cevap listesine esler.
    public partial List<StoredFileDto> Map(List<StoredFile> source);

    // Yukleme istegini Manager'in bekledigi domain create modeline esler.
    public partial StoredFileCreateModel Map(UploadStoredFileDto source);

    // Kayit alanlarini depodan okunan byte'larla birlestirip indirme sozlesmesine esler.
    public partial DownloadStoredFileDto Map(StoredFile source, byte[] data);
}
