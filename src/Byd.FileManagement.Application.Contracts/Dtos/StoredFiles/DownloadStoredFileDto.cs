namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Dosya indirme cevabini tanimlar.
/// </summary>
// islevi: Depodan okunan byte'lari, gosterilecek dosya adini ve content type'i istemciye tasir.
// sistemdeki gorevi: Public indirme sozlesmesidir; alan adi tasidigi degeri dogru anlatir.
public class DownloadStoredFileDto
{
    public byte[] Data { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
}
