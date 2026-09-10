using System;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Sureli dogrudan depolama baglantisini tanimlar.
/// </summary>
// islevi: Dosya adini, imzali URL'yi ve mutlak bitis zamanini istemciye tasir.
// sistemdeki gorevi: Presigned indirme ve yukleme endpointlerinin ortak response sozlesmesidir.
// Kayit kimligi tasinmaz: dosyalar is kaydi ve kategori uzerinden adreslenir, dosya adi da
// liste donen uclarda baglantilari ayirt etmeye yeter.
public class PresignedUrlDto
{
    public string OriginalFileName { get; set; } = default!;
    public string Url { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
