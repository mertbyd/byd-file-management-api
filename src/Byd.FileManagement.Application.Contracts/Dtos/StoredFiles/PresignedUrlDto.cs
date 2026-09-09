using System;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Sureli dogrudan depolama baglantisini tanimlar.
/// </summary>
// islevi: Kayit kimligini, imzali URL'yi ve mutlak bitis zamanini istemciye tasir.
// sistemdeki gorevi: Presigned indirme ve yukleme endpointlerinin ortak response sozlesmesidir.
public class PresignedUrlDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
