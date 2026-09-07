namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Dosya indirme cevabini tanimlar.
/// </summary>
// islevi: Depodan okunan byte'lari ve gosterilecek dosya adini istemciye tasir.
// sistemdeki gorevi: Public indirme sozlesmesidir; alan adi tasidigi degeri dogru anlatir.
public class DownloadStoredFileDto
{
    public byte[] Data { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
}
