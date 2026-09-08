namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Dosya indirme cevabini tanimlar.
/// </summary>
// islevi: Depodan okunan byte'lari, gosterilecek dosya adini ve icerik turunu istemciye tasir.
// sistemdeki gorevi: Public indirme sozlesmesidir; alan adi tasidigi degeri dogru anlatir.
public class DownloadStoredFileDto
{
    public byte[] Data { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;

    // Icerik turu veritabaninda tutulmaz; okuma aninda dosya adindan uretilir.
    // Yukleme aninda Manager turun cozulebildigini dogruladigi icin bu deger her zaman uretilebilir.
    public string ContentType { get; set; } = default!;
}
